using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Text.Json;

namespace MyBlazorApp.Components.Pages;

public partial class NellaNexus : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IHttpClientFactory HttpFactory { get; set; } = default!;

    // Page state
    public bool HasEntered { get; set; }

    // Photo carousel
    public int ActivePhoto { get; set; }
    private System.Timers.Timer? _photoTimer;

    // Form
    public bool IsSubmitting { get; set; }
    public bool IsSubmitted { get; set; }
    public NexusFormModel FormModel { get; set; } = new();

    // Donation
    public bool ShowDonation { get; set; }
    public bool ShowTracklist { get; set; }
    public int SelectedAmount { get; set; } = 5;
    public bool IsCustomAmount { get; set; }
    public int CustomAmountValue { get; set; } = 5;
    public bool IsProcessing { get; set; }
    public int FinalAmount => IsCustomAmount ? Math.Max(5, CustomAmountValue) : SelectedAmount;

    // Audio
    public bool IsPlayingOMG { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Check if returning from Stripe — verify payment server-side
        var uri = new Uri(Nav.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var sessionId = query["session_id"];

        if (!string.IsNullOrEmpty(sessionId))
        {
            try
            {
                var client = HttpFactory.CreateClient();
                var baseUri = Nav.BaseUri.TrimEnd('/');
                var response = await client.GetAsync($"{baseUri}/api/verify-payment?sessionId={sessionId}");

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(body);
                    if (json.RootElement.TryGetProperty("valid", out var valid) && valid.GetBoolean())
                    {
                        ShowTracklist = true;
                        ShowDonation = false;
                    }
                }
            }
            catch { /* Payment verification failed — don't show tracklist */ }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(200);

            // Matrix rain
            try
            {
                await JS.InvokeVoidAsync("eval", @"
                    (function() {
                        var canvas = document.getElementById('matrixCanvas');
                        if (!canvas) return;
                        var ctx = canvas.getContext('2d');
                        var w, h, columns, streams;
                        var fontSize = 11;
                        var chars = 'アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンNELLA0123456789@#$%&=+~^'.split('');
                        function resize() {
                            w=window.innerWidth; h=window.innerHeight;
                            canvas.width=w; canvas.height=h;
                            canvas.style.width=w+'px'; canvas.style.height=h+'px';
                            columns=Math.floor(w/fontSize); streams=[];
                            for(var i=0;i<columns;i++) streams.push({y:Math.floor(Math.random()*h/fontSize),speed:0.3+Math.random()*0.5,len:12+Math.floor(Math.random()*14)});
                            ctx.fillStyle='#000'; ctx.fillRect(0,0,w,h);
                        }
                        resize(); window.addEventListener('resize',resize);
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

            // Photo carousel
            _photoTimer = new System.Timers.Timer(4000);
            _photoTimer.Elapsed += (s, e) =>
            {
                ActivePhoto = (ActivePhoto + 1) % 4;
                InvokeAsync(StateHasChanged);
            };
            _photoTimer.Start();

            HasEntered = true;
            StateHasChanged();
        }
    }

    // Form
    public async Task HandleSubmit()
    {
        if (IsSubmitting || IsSubmitted) return;
        IsSubmitting = true;
        StateHasChanged();
        await Task.Delay(1500);
        IsSubmitting = false;
        IsSubmitted = true;
        StateHasChanged();
    }

    // Donation
    public void ShowDonationPanel()
    {
        ShowDonation = true;
        SelectedAmount = 5;
        IsCustomAmount = false;
    }

    public void SelectAmount(int amount)
    {
        SelectedAmount = amount;
        IsCustomAmount = false;
    }

    public void ShowCustom()
    {
        IsCustomAmount = true;
        CustomAmountValue = 5;
    }

    public async Task ProcessDonation()
    {
        if (IsProcessing || FinalAmount < 5) return;
        IsProcessing = true;
        StateHasChanged();

        try
        {
            await JS.InvokeVoidAsync("eval", $@"
                (async function() {{
                    try {{
                        var res = await fetch('/api/create-checkout', {{
                            method: 'POST',
                            headers: {{ 'Content-Type': 'application/json' }},
                            body: JSON.stringify({{ amount: {FinalAmount} }})
                        }});
                        if (res.status === 429) {{ alert('Too many requests. Please wait a moment.'); return; }}
                        var data = await res.json();
                        if (data.url) {{ window.location.href = data.url; }}
                        else {{ alert('Payment setup failed. Please try again.'); }}
                    }} catch(e) {{ alert('Connection error. Please try again.'); }}
                }})();
            ");
        }
        catch
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    // Audio
    public async Task ToggleOMG()
    {
        IsPlayingOMG = !IsPlayingOMG;
        StateHasChanged();

        if (IsPlayingOMG)
        {
            await JS.InvokeVoidAsync("eval", @"
                if (!window._omgAudio) { window._omgAudio = new Audio('audio/OMG.wav'); }
                window._omgAudio.play();
            ");
        }
        else
        {
            await JS.InvokeVoidAsync("eval", "if(window._omgAudio){window._omgAudio.pause();}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        _photoTimer?.Dispose();
        try
        {
            await JS.InvokeVoidAsync("eval",
                "if(window._matrixRaf){cancelAnimationFrame(window._matrixRaf);}" +
                "if(window._omgAudio){window._omgAudio.pause();window._omgAudio=null;}");
        }
        catch { }
    }

    public class NexusFormModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
    }
}