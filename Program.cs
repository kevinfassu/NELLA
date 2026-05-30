using MyBlazorApp.Components;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

// ─── Rate Limiting ───
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("checkout", opt =>
    {
        opt.PermitLimit = 5;           // 5 requests
        opt.Window = TimeSpan.FromMinutes(1); // per minute
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("webhook", opt =>
    {
        opt.PermitLimit = 30;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please wait.", token);
    };
});

// ─── In-memory store for verified payments ───
// In production, use a database instead
builder.Services.AddSingleton<PaymentStore>();

var app = builder.Build();

// ─── Force HTTPS in production ───
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseRateLimiter();

// ─── Config helpers ───
string GetStripeSecret() =>
    Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
    ?? app.Configuration["Stripe:SecretKey"]
    ?? throw new InvalidOperationException("Stripe secret key not configured");

string GetWebhookSecret() =>
    Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")
    ?? app.Configuration["Stripe:WebhookSecret"]
    ?? throw new InvalidOperationException("Stripe webhook secret not configured");

string GetTokenSecret() =>
    Environment.GetEnvironmentVariable("NELLA_TOKEN_SECRET")
    ?? app.Configuration["Security:TokenSecret"]
    ?? "CHANGE-THIS-TO-A-RANDOM-64-CHAR-STRING-IN-PRODUCTION";

// ─── Signed token helpers ───
string CreatePaymentToken(string sessionId)
{
    var payload = $"{sessionId}|{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GetTokenSecret()));
    var hash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
    return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}|{hash}"));
}

bool ValidatePaymentToken(string token, PaymentStore store)
{
    try
    {
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var parts = decoded.Split('|');
        if (parts.Length != 3) return false;

        var sessionId = parts[0];
        var timestamp = long.Parse(parts[1]);
        var providedHash = parts[2];

        // Token expires after 1 hour
        var age = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timestamp;
        if (age > 3600) return false;

        // Verify HMAC
        var payload = $"{sessionId}|{timestamp}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GetTokenSecret()));
        var expectedHash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
        if (providedHash != expectedHash) return false;

        // Verify payment was confirmed via webhook
        return store.IsConfirmed(sessionId);
    }
    catch { return false; }
}

// ═══════════════════════════════════════════
// API: Create Stripe Checkout Session
// ═══════════════════════════════════════════
app.MapPost("/api/create-checkout", async (HttpRequest request, PaymentStore store) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    int amount;
    try
    {
        var json = JsonDocument.Parse(body);
        amount = json.RootElement.GetProperty("amount").GetInt32();
    }
    catch { return Results.BadRequest(new { error = "Invalid request" }); }

    if (amount < 5 || amount > 10000)
        return Results.BadRequest(new { error = "Amount must be between $5 and $10,000" });

    var stripeSecret = GetStripeSecret();
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", stripeSecret);

    var origin = $"{request.Scheme}://{request.Host}";

    // Create a pending session record
    var sessionNonce = Guid.NewGuid().ToString("N");

    var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        ["payment_method_types[0]"] = "card",
        ["line_items[0][price_data][currency]"] = "usd",
        ["line_items[0][price_data][product_data][name]"] = "NELLA NEXUS — Donation",
        ["line_items[0][price_data][product_data][description]"] = $"Support Nella — ${amount} donation to unlock exclusive music",
        ["line_items[0][price_data][unit_amount]"] = (amount * 100).ToString(),
        ["line_items[0][quantity]"] = "1",
        ["mode"] = "payment",
        ["success_url"] = $"{origin}/nella-nexus?session_id={{CHECKOUT_SESSION_ID}}",
        ["cancel_url"] = $"{origin}/nella-nexus",
        ["metadata[nonce]"] = sessionNonce,
    });

    var response = await httpClient.PostAsync("https://api.stripe.com/v1/checkout/sessions", formContent);
    var responseBody = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        return Results.Json(new { error = "Payment setup failed" }, statusCode: 500);

    var sessionJson = JsonDocument.Parse(responseBody);
    var checkoutUrl = sessionJson.RootElement.GetProperty("url").GetString();
    var sessionId = sessionJson.RootElement.GetProperty("id").GetString();

    // Track this session as pending
    store.AddPending(sessionId!);

    return Results.Json(new { url = checkoutUrl });

}).RequireRateLimiting("checkout");


// ═══════════════════════════════════════════
// API: Stripe Webhook (payment confirmation)
// ═══════════════════════════════════════════
app.MapPost("/api/stripe-webhook", async (HttpRequest request, PaymentStore store) =>
{
    // Read raw body for signature verification
    using var reader = new StreamReader(request.Body);
    var payload = await reader.ReadToEndAsync();

    // Verify Stripe signature
    var sigHeader = request.Headers["Stripe-Signature"].FirstOrDefault();
    if (string.IsNullOrEmpty(sigHeader))
        return Results.BadRequest("Missing signature");

    var webhookSecret = GetWebhookSecret();

    // Parse signature header
    var sigParts = sigHeader.Split(',')
        .Select(p => p.Split('='))
        .Where(p => p.Length == 2)
        .ToDictionary(p => p[0], p => p[1]);

    if (!sigParts.TryGetValue("t", out var timestamp) || !sigParts.TryGetValue("v1", out var signature))
        return Results.BadRequest("Invalid signature format");

    // Verify timestamp (reject if older than 5 minutes)
    if (long.TryParse(timestamp, out var ts))
    {
        var age = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - ts;
        if (Math.Abs(age) > 300)
            return Results.BadRequest("Timestamp too old");
    }

    // Compute expected signature
    var signedPayload = $"{timestamp}.{payload}";
    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
    var expectedSig = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signedPayload))).ToLowerInvariant();

    if (expectedSig != signature)
        return Results.BadRequest("Invalid signature");

    // Parse the event
    var eventJson = JsonDocument.Parse(payload);
    var eventType = eventJson.RootElement.GetProperty("type").GetString();

    if (eventType == "checkout.session.completed")
    {
        var sessionId = eventJson.RootElement
            .GetProperty("data")
            .GetProperty("object")
            .GetProperty("id")
            .GetString();

        var paymentStatus = eventJson.RootElement
            .GetProperty("data")
            .GetProperty("object")
            .GetProperty("payment_status")
            .GetString();

        if (paymentStatus == "paid" && sessionId != null)
        {
            store.Confirm(sessionId);
        }
    }

    return Results.Ok();

}).RequireRateLimiting("webhook");


// ═══════════════════════════════════════════
// API: Verify payment (called by frontend)
// ═══════════════════════════════════════════
app.MapGet("/api/verify-payment", async (string sessionId, PaymentStore store) =>
{
    if (string.IsNullOrEmpty(sessionId))
        return Results.BadRequest(new { valid = false });

    // First check local store
    if (store.IsConfirmed(sessionId))
        return Results.Json(new { valid = true });

    // If webhook hasn't fired yet, verify directly with Stripe
    var stripeSecret = GetStripeSecret();
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", stripeSecret);

    var response = await httpClient.GetAsync($"https://api.stripe.com/v1/checkout/sessions/{sessionId}");
    if (!response.IsSuccessStatusCode)
        return Results.Json(new { valid = false });

    var body = await response.Content.ReadAsStringAsync();
    var session = JsonDocument.Parse(body);
    var status = session.RootElement.GetProperty("payment_status").GetString();

    if (status == "paid")
    {
        store.Confirm(sessionId);
        return Results.Json(new { valid = true });
    }

    return Results.Json(new { valid = false });

}).RequireRateLimiting("checkout");


app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();


// ═══════════════════════════════════════════
// In-memory payment store
// Replace with a database in production
// ═══════════════════════════════════════════
public class PaymentStore
{
    private readonly ConcurrentDictionary<string, PaymentRecord> _sessions = new();

    public void AddPending(string sessionId)
    {
        _sessions[sessionId] = new PaymentRecord
        {
            Status = "pending",
            CreatedAt = DateTimeOffset.UtcNow
        };
        Cleanup();
    }

    public void Confirm(string sessionId)
    {
        _sessions.AddOrUpdate(sessionId,
            new PaymentRecord { Status = "confirmed", CreatedAt = DateTimeOffset.UtcNow },
            (_, existing) => { existing.Status = "confirmed"; return existing; });
    }

    public bool IsConfirmed(string sessionId)
    {
        return _sessions.TryGetValue(sessionId, out var record) && record.Status == "confirmed";
    }

    // Remove records older than 2 hours
    private void Cleanup()
    {
        var cutoff = DateTimeOffset.UtcNow.AddHours(-2);
        foreach (var kvp in _sessions)
        {
            if (kvp.Value.CreatedAt < cutoff)
                _sessions.TryRemove(kvp.Key, out _);
        }
    }

    private class PaymentRecord
    {
        public string Status { get; set; } = "pending";
        public DateTimeOffset CreatedAt { get; set; }
    }
}