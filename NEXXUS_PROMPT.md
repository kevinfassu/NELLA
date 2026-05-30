# NEXXUS — Claude Code Build Prompt
## A Place, Not a Page · Nella's Artist Universe
**Project:** MyBlazorApp (.NET 9 / Blazor Server)  
**Stack:** Blazor Server · C# · JavaScript · CSS  
**Payments:** Stripe Checkout + Webhooks  
**Prepared for:** Claude Code (lead build AI)  
**Author:** Nella · Version 1.0

---

## 00 — How to Use This Document

Read the entire document before writing a single line. Do not skip sections. Do not infer — if it is not written here, ask before building.

### Sacred Rules
- One word is banned in all copy: the r-word commonly paired with "discipline" or "control." Use: discipline, edit, selection, taste.
- Never re-order the song catalog: JEJE · DANDA KIDI · NO VISA · LUKAKU · OMG · LEFT ON READ · QUEEN.
- Brand accent colors only: Cowrie Cream `#F4E8D0` and Gold `#C9A96A`. No purple, no neon.
- Always spell Fèfè with the grave accent on the first e.
- No emoji in product copy. Use typography for emphasis.
- Sacred phrases (do not paraphrase): "A place, not a page." · "AI as instrument. Nella as composer."

### File Separation Rule — No Exceptions
- `PageName.razor` → HTML markup ONLY. Zero `@code` blocks.
- `PageName.razor.cs` → All C# logic (code-behind partial class).
- `PageName.razor.css` → All scoped CSS for that page.
- `wwwroot/js/` → All JavaScript. Never inline in `.razor` files.

---

## 01 — About Nella (Brand Voice)

Born in Yaoundé, Cameroon. Rooted in Bamiléké heritage. Raised on Makossa, Bikutsi, and Ben Skin. Immigrated to California, earned a PharmD, then returned to music — not gently, but as a reckoning.

Her sound is Afrofusion built from the inside out: Afrobeats, R&B, Hip-Hop, and Amapiano. She performs in English, French, Pidgin, and Fèfè simultaneously — not for effect but because that is who she is.

NEXXUS is not a website. It is her universe. A frequency fans walk into.

| Attribute | Detail |
|-----------|--------|
| Genres | Afrobeats · Afrofusion · R&B · Global Sounds |
| Languages | English · French · Pidgin · Fèfè |
| Origin | Yaoundé, Cameroon · Los Angeles, USA |
| Heritage | Bamiléké · Makossa · Bikutsi · Ben Skin |
| Singles | LUKAKU (2026) · NO VISA (2025) · JEJE (2024) |
| Socials | @nellaleen_ (Instagram · TikTok) |
| Contact | nellasongbird@gmail.com |

---

## 02 — Current State of the Project

The project exists at `MyBlazorApp` (.NET 9, Blazor Server). **Do not scaffold a new project. Do not change the project name.**

### What Already Works — Do Not Break
- `App.razor` — global shell with font preloads and script references
- `Landing.razor` + `Landing.razor.cs` + `landing.razor.css` — the portal entry page
- `NellaNexus.razor` + `NellaNexus.razor.cs` + `NellaNexus.razor.css` — the nexus hub page
- `Program.cs` — includes Stripe Checkout + Webhook API endpoints with rate limiting and HMAC verification
- `wwwroot/js/landing.js` — `nellaLanding.init()` function
- `wwwroot/videos/nella-bg.mp4` — the N lettermark video background
- `wwwroot/audio/intro.mp3` — portal intro music (plays on NELLA NEXUS button click, loops with 6-second gap)
- `wwwroot/audio/OMG.wav` — the unlockable track
- `wwwroot/Images/` — artist photos for the fading carousel on the nexus page

### Landing Page Behavior (Working)
- Full-viewport looping `nella-bg.mp4` video (soundwave N lettermark) on pure black background
- Centered pill button "NELLA NEXUS" with gold border glow on hover
- Protocol tags: "// NEXUS PROTOCOL v2.0 //" and "// ENTER THE FREQUENCY //"
- On click: `intro.mp3` starts → video zooms in → navigates to `/nella-nexus`
- On return to landing: audio stops cleanly

### Nexus Page Behavior (Working)
- Background: bluish-silver matrix rain canvas (11px monospace, katakana + NELLA chars)
- Behind the card: 4 artist photos cycle with 2s cross-fade every 4 seconds, dimmed to 25% opacity
- Card: NELLA gradient logo, hero text, Spotify + Apple Music buttons, sign-up form
- Unlock Music button → donation panel ($5/$10/$20/Other, min $5) → Stripe Checkout
- After Stripe payment: server-side verification via `session_id` → tracklist with OMG track + play/pause
- Audio: `OMG.wav` plays/pauses. `intro.mp3` stops when landing page loads.

---

## 03 — Full Project File Structure

Items marked `[EXISTS]` are already in place. Items marked `[BUILD]` need to be created.

```
MyBlazorApp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor          [EXISTS — stripped, no sidebar]
│   │   └── MainLayout.razor.css      [EXISTS]
│   └── Pages/
│       ├── Landing.razor             [EXISTS]
│       ├── Landing.razor.cs          [EXISTS]
│       ├── landing.razor.css         [EXISTS]
│       ├── NellaNexus.razor          [EXISTS]
│       ├── NellaNexus.razor.cs       [EXISTS]
│       ├── NellaNexus.razor.css      [EXISTS]
│       ├── Atrium.razor              [BUILD — room selector hub]
│       ├── Atrium.razor.cs           [BUILD]
│       ├── Atrium.razor.css          [BUILD]
│       ├── World.razor               [BUILD — dynamic route /world/{slug}]
│       ├── World.razor.cs            [BUILD]
│       ├── World.razor.css           [BUILD]
│       ├── Fashion.razor             [PHASE 2]
│       ├── RestHouse.razor           [PHASE 2]
│       └── LanguageRoom.razor        [PHASE 2]
├── App.razor                         [EXISTS]
├── Routes.razor                      [EXISTS]
├── _Imports.razor                    [EXISTS]
├── Program.cs                        [EXISTS — includes Stripe API]
├── appsettings.json                  [EXISTS — Stripe keys here]
└── wwwroot/
    ├── app.css                       [EXISTS]
    ├── favicon.png                   [EXISTS]
    ├── js/
    │   ├── landing.js               [EXISTS]
    │   └── worlds.js                [BUILD — per-world canvas FX]
    ├── videos/
    │   └── nella-bg.mp4             [EXISTS]
    ├── audio/
    │   ├── intro.mp3                [EXISTS]
    │   └── OMG.wav                  [EXISTS]
    └── Images/
        ├── nella-photo-1.jpg through nella-photo-4.jpg  [EXISTS]
```

---

## 04 — Architecture & Coding Rules

### Blazor Patterns
- `@rendermode InteractiveServer` on every page component
- Never put `@code` blocks in `.razor` files. All C# belongs in `.razor.cs`
- All members that `.razor` markup binds to must be `public` in the code-behind class
- `ElementReference` fields must be `public` for `@ref` to work
- Use `JS.InvokeVoidAsync("eval", ...)` for quick inline JS
- Always use `getElementById(id)` in JS — never `ElementReference` parameters
- Canvas animations store their `requestAnimationFrame` handle on `window._xxxRaf`
- Timers (`System.Timers.Timer`) must be disposed in `DisposeAsync()` via `IAsyncDisposable`

### CSS Rules
- Scoped CSS (`.razor.css`) for page-specific styles
- Global styles in `wwwroot/app.css` only
- Brand colors: `--color-gold: #C9A96A` · `--color-cream: #F4E8D0` · Background: `#000000`
- Font: Space Grotesk (loaded via Google Fonts in App.razor). Fallback: Helvetica Neue, Arial
- All immersive pages: `position: fixed; inset: 0; overflow-y: auto`

### JavaScript Rules
- All JS in `wwwroot/js/` as plain `.js` files. Never inline in `.razor`
- Canvas init: always set both `canvas.width/height` (attributes) AND `canvas.style.width/height`
- Attach `window.addEventListener("resize", ...)` for every canvas
- Store all global handles on `window._` prefixed keys

### Security Rules
- Stripe Secret Key: env var `STRIPE_SECRET_KEY` first, then `appsettings.json`
- Webhook: verify `Stripe-Signature` header with HMAC-SHA256. Reject events older than 5 minutes
- Payment verification: `/api/verify-payment` checks PaymentStore AND calls Stripe API. A bare `?paid=true` URL param NEVER unlocks content
- Rate limiting: 5 req/min on `/api/create-checkout`, 30 req/min on webhook
- HTTPS enforced in production

---

## 05 — Complete Code: Existing Files

### App.razor
```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@400;600;700&display=swap" rel="stylesheet" />
    <link rel="preload" as="video" href="videos/nella-bg.mp4" />
    <link rel="stylesheet" href="@Assets["lib/bootstrap/dist/css/bootstrap.min.css"]" />
    <link rel="stylesheet" href="@Assets["app.css"]" />
    <link rel="stylesheet" href="@Assets["MyBlazorApp.styles.css"]" />
    <ImportMap />
    <link rel="icon" type="image/png" href="favicon.png" />
    <HeadOutlet />
</head>
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
    <script src="js/landing.js"></script>
    <script src="js/worlds.js"></script>
</body>
</html>
```

### Landing.razor
```razor
@page "/"
@rendermode InteractiveServer

<HeadContent>
    <PageTitle>NELLA — The Portal</PageTitle>
</HeadContent>

<div class="portal-root @(IsZooming ? "zooming" : "") @(HasLoaded ? "loaded" : "")">
    <div class="video-wrap" aria-hidden="true">
        <video autoplay loop muted playsinline disablepictureinpicture>
            <source src="videos/nella-bg.mp4" type="video/mp4" />
        </video>
    </div>
    <div class="scrim" aria-hidden="true"></div>
    <div class="scanlines" aria-hidden="true"></div>
    <main class="portal-center">
        <div class="portal-content fade-up">
            <span class="protocol-tag">// NEXUS PROTOCOL v2.0 //</span>
            <button class="nexus-btn" @onclick="EnterNexus" aria-label="Enter Nella Nexus">
                <span class="btn-glow" aria-hidden="true"></span>
                <span class="btn-text">NELLA NEXUS</span>
            </button>
            <span class="protocol-tag sub">// ENTER THE FREQUENCY //</span>
        </div>
    </main>
</div>
```

### Landing.razor.cs
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyBlazorApp.Components.Pages;

public partial class Landing : ComponentBase
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    public bool HasLoaded { get; set; }
    public bool IsZooming { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await JS.InvokeVoidAsync("eval", @"
                    if (window._nellaAudio) {
                        window._nellaAudio.pause();
                        window._nellaAudio.currentTime = 0;
                        window._nellaAudio = null;
                    }
                ");
            }
            catch { }

            try { await JS.InvokeVoidAsync("nellaLanding.init"); } catch { }

            await Task.Delay(100);
            HasLoaded = true;
            StateHasChanged();
        }
    }

    public async Task EnterNexus()
    {
        if (IsZooming) return;
        IsZooming = true;
        StateHasChanged();

        try
        {
            await JS.InvokeVoidAsync("eval", @"
                (function() {
                    var a = new Audio('audio/intro.mp3');
                    a.volume = 1; a.play();
                    window._nellaAudio = a;
                    a.addEventListener('ended', function() {
                        setTimeout(function() {
                            if (window._nellaAudio) {
                                window._nellaAudio.currentTime = 0;
                                window._nellaAudio.play();
                            }
                        }, 6000);
                    });
                })();
            ");
        }
        catch { }

        await Task.Delay(1200);
        Nav.NavigateTo("/nella-nexus");
    }
}
```

### Program.cs
```csharp
using MyBlazorApp.Components;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<PaymentStore>();

builder.Services.AddRateLimiter(options => {
    options.AddFixedWindowLimiter("checkout", opt => {
        opt.PermitLimit = 5; opt.Window = TimeSpan.FromMinutes(1); opt.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("webhook", opt => {
        opt.PermitLimit = 30; opt.Window = TimeSpan.FromMinutes(1); opt.QueueLimit = 0;
    });
    options.OnRejected = async (ctx, token) => {
        ctx.HttpContext.Response.StatusCode = 429;
        await ctx.HttpContext.Response.WriteAsync("Too many requests.", token);
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseRateLimiter();

string GetStripeSecret() =>
    Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
    ?? app.Configuration["Stripe:SecretKey"]
    ?? throw new InvalidOperationException("Stripe secret key not configured");

string GetWebhookSecret() =>
    Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")
    ?? app.Configuration["Stripe:WebhookSecret"]
    ?? throw new InvalidOperationException("Webhook secret not configured");

// POST /api/create-checkout
app.MapPost("/api/create-checkout", async (HttpRequest req, PaymentStore store) => {
    using var reader = new StreamReader(req.Body);
    var body = await reader.ReadToEndAsync();
    int amount;
    try { var j = JsonDocument.Parse(body); amount = j.RootElement.GetProperty("amount").GetInt32(); }
    catch { return Results.BadRequest(new { error = "Invalid request" }); }
    if (amount < 5 || amount > 10000) return Results.BadRequest(new { error = "Amount must be $5-$10,000" });

    using var http = new HttpClient();
    http.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetStripeSecret());

    var origin = $"{req.Scheme}://{req.Host}";
    var form = new FormUrlEncodedContent(new Dictionary<string, string> {
        ["payment_method_types[0]"] = "card",
        ["line_items[0][price_data][currency]"] = "usd",
        ["line_items[0][price_data][product_data][name]"] = "NELLA NEXUS — Donation",
        ["line_items[0][price_data][product_data][description]"] = $"Support Nella — ${amount} donation",
        ["line_items[0][price_data][unit_amount]"] = (amount * 100).ToString(),
        ["line_items[0][quantity]"] = "1",
        ["mode"] = "payment",
        ["success_url"] = $"{origin}/nella-nexus?session_id={{CHECKOUT_SESSION_ID}}",
        ["cancel_url"] = $"{origin}/nella-nexus",
    });

    var res = await http.PostAsync("https://api.stripe.com/v1/checkout/sessions", form);
    var resBody = await res.Content.ReadAsStringAsync();
    if (!res.IsSuccessStatusCode) return Results.Json(new { error = "Stripe error" }, statusCode: 500);

    var session = JsonDocument.Parse(resBody);
    var url = session.RootElement.GetProperty("url").GetString();
    var sid = session.RootElement.GetProperty("id").GetString();
    store.AddPending(sid!);
    return Results.Json(new { url });
}).RequireRateLimiting("checkout");

// POST /api/stripe-webhook
app.MapPost("/api/stripe-webhook", async (HttpRequest req, PaymentStore store) => {
    using var reader = new StreamReader(req.Body);
    var payload = await reader.ReadToEndAsync();
    var sig = req.Headers["Stripe-Signature"].FirstOrDefault();
    if (string.IsNullOrEmpty(sig)) return Results.BadRequest("Missing signature");

    var parts = sig.Split(",").Select(p => p.Split("="))
        .Where(p => p.Length == 2).ToDictionary(p => p[0], p => p[1]);
    if (!parts.TryGetValue("t", out var ts) || !parts.TryGetValue("v1", out var sigV))
        return Results.BadRequest("Invalid signature");

    if (long.TryParse(ts, out var tsLong))
        if (Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - tsLong) > 300)
            return Results.BadRequest("Timestamp expired");

    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GetWebhookSecret()));
    var expected = Convert.ToHexString(
        hmac.ComputeHash(Encoding.UTF8.GetBytes($"{ts}.{payload}"))).ToLowerInvariant();
    if (expected != sigV) return Results.BadRequest("Invalid signature");

    var ev = JsonDocument.Parse(payload);
    if (ev.RootElement.GetProperty("type").GetString() == "checkout.session.completed") {
        var obj = ev.RootElement.GetProperty("data").GetProperty("object");
        var sessionId = obj.GetProperty("id").GetString();
        if (obj.GetProperty("payment_status").GetString() == "paid" && sessionId != null)
            store.Confirm(sessionId);
    }
    return Results.Ok();
}).RequireRateLimiting("webhook");

// GET /api/verify-payment?sessionId=xxx
app.MapGet("/api/verify-payment", async (string sessionId, PaymentStore store) => {
    if (string.IsNullOrEmpty(sessionId)) return Results.BadRequest(new { valid = false });
    if (store.IsConfirmed(sessionId)) return Results.Json(new { valid = true });

    using var http = new HttpClient();
    http.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetStripeSecret());
    var res = await http.GetAsync($"https://api.stripe.com/v1/checkout/sessions/{sessionId}");
    if (!res.IsSuccessStatusCode) return Results.Json(new { valid = false });
    var body = await res.Content.ReadAsStringAsync();
    var s = JsonDocument.Parse(body);
    if (s.RootElement.GetProperty("payment_status").GetString() == "paid") {
        store.Confirm(sessionId);
        return Results.Json(new { valid = true });
    }
    return Results.Json(new { valid = false });
}).RequireRateLimiting("checkout");

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();

public class PaymentStore {
    private readonly ConcurrentDictionary<string, (string status, DateTimeOffset created)> _s = new();
    public void AddPending(string id) { _s[id] = ("pending", DateTimeOffset.UtcNow); Cleanup(); }
    public void Confirm(string id) => _s.AddOrUpdate(id, ("confirmed", DateTimeOffset.UtcNow),
        (_, e) => ("confirmed", e.created));
    public bool IsConfirmed(string id) => _s.TryGetValue(id, out var r) && r.status == "confirmed";
    private void Cleanup() {
        var cut = DateTimeOffset.UtcNow.AddHours(-2);
        foreach (var k in _s.Where(x => x.Value.created < cut).Select(x => x.Key)) _s.TryRemove(k, out _);
    }
}
```

### appsettings.json
```json
{
  "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
  "AllowedHosts": "*",
  "Stripe": {
    "SecretKey": "sk_live_REPLACE_WITH_YOUR_SECRET_KEY",
    "WebhookSecret": "whsec_REPLACE_WITH_YOUR_WEBHOOK_SECRET"
  }
}
```
> **NEVER commit real secret keys to git. Use environment variables in production.**

---

## 06 — Next Build: The Atrium (Room Selector)

Route: `/atrium`. The hub after the Nexus page where fans choose a song-world.

### Visual Design
- Pure black background
- Seven room cards in a grid, one per song-world
- Each card: song title, mood descriptor, "ENTER →" button
- On hover: card glows gold, subtle scale up
- Page entrance: cards fade and slide up staggered (CSS animation-delay using `--delay` CSS var)
- Matrix rain continues as background (reuse existing matrix rain logic with canvas id `atriumMatrix`)

### Song-World Catalog — EXACT ORDER, DO NOT CHANGE

| # | Slug | Title | Mood |
|---|------|-------|------|
| 01 | jeje | JEJE | Warm, celebratory, Cameroonian roots |
| 02 | danda-kidi | DANDA KIDI | Percussive energy, fierce, rhythmic |
| 03 | no-visa | NO VISA | Love across borders, Afrobeats + diaspora |
| 04 | lukaku | LUKAKU | Self-worth anthem, gold, unstoppable |
| 05 | omg | OMG | Electrifying, surprise, peak energy |
| 06 | left-on-read | LEFT ON READ | Digital longing, melancholic, intimate |
| 07 | queen | QUEEN | The closer. Regal, definitive, triumphant |

### Atrium.razor
```razor
@page "/atrium"
@rendermode InteractiveServer

<HeadContent><PageTitle>NELLA NEXUS — Atrium</PageTitle></HeadContent>

<div class="atrium-root @(HasEntered ? "entered" : "")">
    <canvas id="atriumMatrix" class="atrium-matrix"></canvas>
    <div class="atrium-scrim"></div>
    <main class="atrium-main">
        <header class="atrium-header fade-up">
            <h1 class="atrium-title">NELLA NEXUS</h1>
            <p class="atrium-sub">// CHOOSE YOUR WORLD //</p>
        </header>
        <div class="worlds-grid">
            @foreach (var world in Worlds)
            {
                <div class="world-card fade-up" style="--delay: @(world.Index * 80)ms"
                     @onclick="() => EnterWorld(world.Slug)">
                    <span class="world-num">@world.Number</span>
                    <h2 class="world-name">@world.Title</h2>
                    <p class="world-mood">@world.Mood</p>
                    <span class="world-enter">ENTER →</span>
                </div>
            }
        </div>
    </main>
</div>
```

### Atrium.razor.cs
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyBlazorApp.Components.Pages;

public partial class Atrium : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    public bool HasEntered { get; set; }

    public record WorldCard(int Index, string Number, string Slug, string Title, string Mood);

    public List<WorldCard> Worlds { get; } = new() {
        new(0, "01", "jeje",         "JEJE",         "Warm, celebratory, Cameroonian roots"),
        new(1, "02", "danda-kidi",   "DANDA KIDI",   "Percussive energy, fierce, rhythmic"),
        new(2, "03", "no-visa",      "NO VISA",       "Love across borders, Afrobeats + diaspora"),
        new(3, "04", "lukaku",       "LUKAKU",        "Self-worth anthem, gold, unstoppable"),
        new(4, "05", "omg",          "OMG",           "Electrifying, surprise, peak energy"),
        new(5, "06", "left-on-read", "LEFT ON READ",  "Digital longing, melancholic, intimate"),
        new(6, "07", "queen",        "QUEEN",         "The closer. Regal, definitive, triumphant"),
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(200);
            try
            {
                await JS.InvokeVoidAsync("eval", @"
                    (function() {
                        var canvas = document.getElementById('atriumMatrix');
                        if (!canvas) return;
                        var ctx = canvas.getContext('2d');
                        var w, h, columns, streams;
                        var fontSize = 11;
                        var chars = 'アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンNELLA0123456789@#$%&=+~^'.split('');
                        function resize() {
                            w = window.innerWidth; h = window.innerHeight;
                            canvas.width = w; canvas.height = h;
                            canvas.style.width = w+'px'; canvas.style.height = h+'px';
                            columns = Math.floor(w/fontSize); streams = [];
                            for(var i=0;i<columns;i++) streams.push({y:Math.floor(Math.random()*h/fontSize),speed:0.3+Math.random()*0.5,len:12+Math.floor(Math.random()*14)});
                            ctx.fillStyle='#000'; ctx.fillRect(0,0,w,h);
                        }
                        resize(); window.addEventListener('resize', resize);
                        function draw() {
                            ctx.fillStyle='rgba(0,0,0,0.06)'; ctx.fillRect(0,0,w,h);
                            ctx.font=fontSize+'px monospace';
                            for(var i=0;i<columns;i++){
                                var s=streams[i],x=i*fontSize,headY=Math.floor(s.y)*fontSize;
                                for(var t=0;t<s.len;t++){
                                    var cy=headY-t*fontSize;
                                    if(cy<-fontSize||cy>h+fontSize) continue;
                                    var c=chars[Math.floor(Math.random()*chars.length)];
                                    if(t===0) ctx.fillStyle='#eef4ff';
                                    else if(t<3) ctx.fillStyle='rgba(160,190,230,0.85)';
                                    else{var f=1-(t/s.len),f2=f*f; ctx.fillStyle='rgba('+(Math.floor(100*f2+60))+','+(Math.floor(130*f2+70))+','+(Math.floor(190*f2+50))+','+(0.2+f2*0.6).toFixed(2)+')';}
                                    ctx.fillText(c,x,cy);
                                }
                                s.y+=s.speed;
                                if((s.y-s.len)*fontSize>h){s.y=Math.floor(Math.random()*-20);s.speed=0.3+Math.random()*0.5;s.len=12+Math.floor(Math.random()*14);}
                            }
                            window._matrixRaf=requestAnimationFrame(draw);
                        }
                        draw();
                    })();
                ");
            }
            catch { }

            HasEntered = true;
            StateHasChanged();
        }
    }

    public void EnterWorld(string slug) => Nav.NavigateTo($"/world/{slug}");

    public async ValueTask DisposeAsync()
    {
        try { await JS.InvokeVoidAsync("eval", "if(window._matrixRaf)cancelAnimationFrame(window._matrixRaf);"); }
        catch { }
    }
}
```

### Atrium.razor.css
```css
.atrium-root { position: fixed; inset: 0; overflow: hidden; overflow-y: auto; background: #000; }
.atrium-matrix { position: fixed; top: 0; left: 0; width: 100%; height: 100%; z-index: 0; display: block; }
.atrium-scrim { position: fixed; inset: 0; z-index: 1; background: radial-gradient(ellipse at center, rgba(0,0,0,0.1) 0%, rgba(0,0,0,0.5) 100%); pointer-events: none; }
.atrium-main { position: relative; z-index: 10; min-height: 100vh; display: flex; flex-direction: column; align-items: center; padding: 48px 24px; gap: 40px; }

.atrium-header { text-align: center; }
.atrium-title { font-family: 'Space Grotesk', sans-serif; font-size: 2.5rem; font-weight: 700; letter-spacing: 6px; margin: 0; background: linear-gradient(135deg, #ffd700, #00e5ff, #ff00ff); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }
.atrium-sub { font-family: 'Space Grotesk', monospace; font-size: 0.72rem; letter-spacing: 3px; color: rgba(255,255,255,0.35); margin: 8px 0 0; }

.worlds-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 16px; width: 100%; max-width: 960px; }

.world-card {
    padding: 28px 24px;
    background: rgba(0,0,0,0.7);
    backdrop-filter: blur(14px);
    -webkit-backdrop-filter: blur(14px);
    border: 1px solid rgba(201,169,106,0.15);
    border-radius: 20px;
    cursor: pointer;
    display: flex; flex-direction: column; gap: 8px;
    transition: transform 250ms ease, border-color 250ms ease, box-shadow 250ms ease;
}
.world-card:hover {
    transform: translateY(-3px) scale(1.02);
    border-color: rgba(201,169,106,0.55);
    box-shadow: 0 0 30px rgba(201,169,106,0.12), 0 12px 32px rgba(0,0,0,0.4);
}
.world-num { font-family: 'Space Grotesk', monospace; font-size: 0.7rem; color: rgba(201,169,106,0.5); font-weight: 600; }
.world-name { font-family: 'Space Grotesk', sans-serif; font-size: 1.4rem; font-weight: 700; color: #fff; margin: 0; letter-spacing: 2px; }
.world-mood { font-family: 'Space Grotesk', sans-serif; font-size: 0.82rem; color: rgba(255,255,255,0.5); margin: 0; line-height: 1.5; }
.world-enter { font-family: 'Space Grotesk', sans-serif; font-size: 0.75rem; font-weight: 600; color: #C9A96A; letter-spacing: 1px; margin-top: 8px; }

.fade-up { opacity: 0; transform: translateY(20px); transition: opacity 0.7s ease var(--delay, 0ms), transform 0.7s cubic-bezier(0.23,1,0.32,1) var(--delay, 0ms); }
.atrium-root.entered .fade-up { opacity: 1; transform: translateY(0); }

@media (max-width: 540px) {
    .atrium-title { font-size: 1.8rem; }
    .worlds-grid { grid-template-columns: 1fr; }
}
```

---

## 07 — Song-World Pages (/world/{slug})

A single `World.razor` handles all 7 slugs via a dynamic route parameter.

### World.razor
```razor
@page "/world/{Slug}"
@rendermode InteractiveServer

<HeadContent><PageTitle>@CurrentWorld?.Title — NELLA NEXUS</PageTitle></HeadContent>

<div class="world-root @(HasEntered ? "entered" : "")" style="--world-color: @(CurrentWorld?.Color ?? "#C9A96A")">
    <canvas id="worldCanvas" class="world-canvas"></canvas>
    <div class="world-scrim"></div>
    <div class="world-hud">
        <button class="hud-back" @onclick="GoBack" aria-label="Back to Atrium">← ATRIUM</button>
        <span class="hud-title">@CurrentWorld?.Title</span>
    </div>
    <main class="world-main fade-up">
        <h1 class="world-display-title">@CurrentWorld?.Title</h1>
        <p class="world-mood-text">@CurrentWorld?.Mood</p>
        <p class="world-desc">@CurrentWorld?.Description</p>
    </main>
</div>
```

### World.razor.cs
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyBlazorApp.Components.Pages;

public partial class World : ComponentBase, IAsyncDisposable
{
    [Parameter] public string Slug { get; set; } = "";
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    public bool HasEntered { get; set; }
    public WorldData? CurrentWorld { get; set; }

    public record WorldData(string Slug, string Title, string Mood, string Description, string Color);

    private static readonly Dictionary<string, WorldData> WorldMap = new() {
        ["jeje"]         = new("jeje", "JEJE", "Warm · Celebratory", "Rooted in Cameroonian tradition, JEJE is a homecoming. The rhythm of Makossa carried into new worlds.", "#C9A96A"),
        ["danda-kidi"]   = new("danda-kidi", "DANDA KIDI", "Percussive · Fierce", "Raw energy. Bikutsi percussion transformed into something that hits harder than any drop.", "#E07B39"),
        ["no-visa"]      = new("no-visa", "NO VISA", "Love Without Borders", "Love has no checkpoint. Open borders. No inspection. Afrobeats and Afrofusion in one breath.", "#5BA4CF"),
        ["lukaku"]       = new("lukaku", "LUKAKU", "Self-Worth · Unstoppable", "I no fi chase. I no be Lukaku. The anthem of a woman who knows her worth.", "#C9A96A"),
        ["omg"]          = new("omg", "OMG", "Electrifying · Peak Energy", "Peak frequency. The moment everything ignites.", "#A78BFA"),
        ["left-on-read"] = new("left-on-read", "LEFT ON READ", "Digital Longing · Intimate", "The silence after the message. Melancholic, intimate, undeniably human.", "#94A3B8"),
        ["queen"]        = new("queen", "QUEEN", "Regal · Triumphant", "The closer. Every world was leading here. Nella as composer. The orchestra rests.", "#FFD700"),
    };

    protected override void OnParametersSet()
    {
        CurrentWorld = WorldMap.TryGetValue(Slug, out var w) ? w : null;
        if (CurrentWorld == null) Nav.NavigateTo("/atrium");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(150);
            try
            {
                await JS.InvokeVoidAsync("eval",
                    $"typeof nellaWorlds !== 'undefined' && nellaWorlds.init('worldCanvas', '{Slug}');");
            }
            catch { }
            HasEntered = true;
            StateHasChanged();
        }
    }

    public void GoBack() => Nav.NavigateTo("/atrium");

    public async ValueTask DisposeAsync()
    {
        try { await JS.InvokeVoidAsync("eval", "if(window._worldRaf)cancelAnimationFrame(window._worldRaf);"); }
        catch { }
    }
}
```

### World.razor.css
```css
.world-root { position: fixed; inset: 0; overflow: hidden; background: #000; }
.world-canvas { position: absolute; top: 0; left: 0; width: 100%; height: 100%; z-index: 0; display: block; }
.world-scrim { position: absolute; inset: 0; z-index: 1; background: radial-gradient(ellipse at center, rgba(0,0,0,0.2) 0%, rgba(0,0,0,0.7) 100%); pointer-events: none; }

.world-hud { position: fixed; top: 0; left: 0; right: 0; z-index: 20; display: flex; align-items: center; justify-content: space-between; padding: 20px 28px; }
.hud-back { background: none; border: 1px solid rgba(201,169,106,0.3); border-radius: 999px; color: rgba(255,255,255,0.7); font-family: 'Space Grotesk', sans-serif; font-size: 0.78rem; font-weight: 600; letter-spacing: 1px; padding: 8px 18px; cursor: pointer; transition: all 200ms ease; }
.hud-back:hover { border-color: #C9A96A; color: #fff; }
.hud-title { font-family: 'Space Grotesk', sans-serif; font-size: 0.85rem; font-weight: 700; letter-spacing: 3px; color: rgba(255,255,255,0.5); }

.world-main { position: relative; z-index: 10; min-height: 100vh; display: flex; flex-direction: column; align-items: center; justify-content: center; padding: 80px 24px 40px; text-align: center; gap: 16px; }
.world-display-title { font-family: 'Space Grotesk', sans-serif; font-size: clamp(3rem, 10vw, 7rem); font-weight: 700; letter-spacing: 6px; color: var(--world-color); margin: 0; text-shadow: 0 0 40px var(--world-color); }
.world-mood-text { font-family: 'Space Grotesk', monospace; font-size: 0.85rem; letter-spacing: 3px; color: rgba(255,255,255,0.45); margin: 0; text-transform: uppercase; }
.world-desc { font-family: 'Space Grotesk', sans-serif; font-size: 1rem; color: rgba(255,255,255,0.65); margin: 0; line-height: 1.8; max-width: 480px; }

.fade-up { opacity: 0; transform: translateY(20px); transition: opacity 0.9s ease 0.2s, transform 0.9s cubic-bezier(0.23,1,0.32,1) 0.2s; }
.world-root.entered .fade-up { opacity: 1; transform: translateY(0); }
```

### wwwroot/js/worlds.js
```javascript
// wwwroot/js/worlds.js
window.nellaWorlds = {
    init: function(canvasId, slug) {
        var canvas = document.getElementById(canvasId);
        if (!canvas) return;
        var ctx = canvas.getContext('2d');
        var w, h;

        function resize() {
            w = canvas.width = window.innerWidth;
            h = canvas.height = window.innerHeight;
            canvas.style.width = w + 'px';
            canvas.style.height = h + 'px';
        }
        resize();
        window.addEventListener('resize', resize);

        var colors = {
            'jeje':         '#C9A96A',
            'danda-kidi':   '#E07B39',
            'no-visa':      '#5BA4CF',
            'lukaku':       '#C9A96A',
            'omg':          '#A78BFA',
            'left-on-read': '#94A3B8',
            'queen':        '#FFD700'
        };
        var color = colors[slug] || '#C9A96A';

        var particles = [];
        for (var i = 0; i < 100; i++) {
            particles.push({
                x: Math.random() * 1000,
                y: Math.random() * 1000,
                r: 0.8 + Math.random() * 2.5,
                vx: (Math.random() - 0.5) * 0.5,
                vy: (Math.random() - 0.5) * 0.5,
                alpha: 0.15 + Math.random() * 0.6
            });
        }

        function draw() {
            ctx.clearRect(0, 0, w, h);
            for (var i = 0; i < particles.length; i++) {
                var p = particles[i];
                // Wrap to current canvas size
                if (p.x > w) p.x = 0;
                if (p.x < 0) p.x = w;
                if (p.y > h) p.y = 0;
                if (p.y < 0) p.y = h;
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
                ctx.fillStyle = color;
                ctx.globalAlpha = p.alpha;
                ctx.fill();
                p.x += p.vx;
                p.y += p.vy;
            }
            ctx.globalAlpha = 1;
            window._worldRaf = requestAnimationFrame(draw);
        }
        draw();
    }
};
```

---

## 08 — Phase 2 Features (Do Not Build Yet)

These features are planned but not in scope for the initial build. Do not make architectural decisions that break them.

### Fashion Room (/fashion)
- AI try-on with 5 wardrobe ghost-mannequin renders
- Fan uploads photo + selects garment → AI composites them wearing it
- **SACRED: User photos NEVER persisted. No disk write. No logs containing image bytes.**
- Three modes: ghost-mannequin, flat-lay, with-subject

### Rest House (/rest-house)
- Fan guestbook — write and read entries
- Abuse + spam moderation queue
- WhatsApp channel webhook for drop notifications

### Language Room (/language/{lang})
- Content in: English · French · Pidgin · Fèfè
- Language order is fixed — never reorder alphabetically
- Always spell Fèfè with the grave accent

### Moonbeam City
- Three.js 3D city environment representing Nella's world
- If timeline tight: replace with 2D illustrated still + CSS parallax

---

## 09 — Brand Design Tokens

| Token | Value | Usage |
|-------|-------|-------|
| `--color-gold` | `#C9A96A` | Primary accent, borders, glows |
| `--color-cream` | `#F4E8D0` | Table headers, subtle fills |
| `--color-black` | `#000000` | Page backgrounds |
| `--color-dark` | `#050508` | Card backgrounds |
| `--font-primary` | Space Grotesk | All UI text |
| `--font-mono` | monospace / Courier New | Protocol tags, code |

> No purple. No neon green. No gradients outside the NELLA logo. Gold and cream ONLY for accents.

---

## 10 — Deployment & Setup Checklist

### Required Before Running
- Set Stripe Secret Key in `appsettings.json` → `Stripe:SecretKey` OR env var `STRIPE_SECRET_KEY`
- Set Stripe Webhook Secret in `appsettings.json` → `Stripe:WebhookSecret` OR env var `STRIPE_WEBHOOK_SECRET`
- Stripe Publishable Key (frontend only): `pk_live_51TQARuJ7NMEnEoS3nrnGVyiMoUZBWQ7kV97gEaw3gHpTfzoK9HtRwGx5KJXaMjxvCAIiZU9SUrPMWz2IVoMf8f1800aFjBgRWg`

### Stripe Webhook Setup
1. Stripe Dashboard → Developers → Webhooks → Add Endpoint
2. URL: `https://yourdomain.com/api/stripe-webhook`
3. Event: `checkout.session.completed`
4. Copy `whsec_...` signing secret into config

### Local Run
```
cd MyBlazorApp
dotnet run
# http://localhost:5070
```

### Stripe Test Card
- Card: `4242 4242 4242 4242`
- Date: any future date
- CVC: any 3 digits

---

*"A place, not a page."*  
*"AI as instrument. Nella as composer."*  
*© 2025 NELLA · 237 x HTX · ALL FREQUENCIES RESERVED*
