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
            // Stop audio if user came back from Nexus
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

            try { await JS.InvokeVoidAsync("nellaLanding.init"); }
            catch { }

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

        // Start audio on user gesture (required by browsers)
        try
        {
            await JS.InvokeVoidAsync("eval", @"
                (function() {
                    var a = new Audio('audio/intro.mp3');
                    a.volume = 1;
                    a.play();
                    window._nellaAudio = a;
                    window._nellaAudioGap = 6000;
                    a.addEventListener('ended', function() {
                        setTimeout(function() {
                            if (window._nellaAudio) {
                                window._nellaAudio.currentTime = 0;
                                window._nellaAudio.play();
                            }
                        }, window._nellaAudioGap);
                    });
                })();
            ");
        }
        catch { }

        await Task.Delay(1200);
        Nav.NavigateTo("/nella-nexus");
    }
}