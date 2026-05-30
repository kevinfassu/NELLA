# NEXXUS — Claude Code Build Prompt v2.0
## A Place, Not a Page · Nella's Artist Universe
**Project:** MyBlazorApp (.NET 9 / Blazor Server)  
**Stack:** Blazor Server · C# · JavaScript · CSS  
**Payments:** Stripe Checkout + Webhooks  
**Prepared for:** Claude Code (lead build AI)  
**Author:** Nella · Version 2.0

---

## 00 — How to Use This Document

Read the **entire document** before writing a single line. Do not skip sections. Do not infer — if it is not written here, ask before building.

### Sacred Rules (Non-Negotiable)
- One word is banned in all copy: the r-word commonly paired with "discipline" or "control." Use instead: discipline, edit, selection, taste.
- **Never re-order the song catalog:** JEJE · DANDA KIDI · NO VISA · LUKAKU · OMG · LEFT ON READ · QUEEN
- **Brand accent colors only:** Cowrie Cream `#F4E8D0` and Gold `#C9A96A`. No purple, no neon.
- **Always spell Fèfè** with the grave accent on the first e.
- No emoji in product copy. Use typography for emphasis.
- Sacred phrases (do not paraphrase): *"A place, not a page."* · *"AI as instrument. Nella as composer."*
- User photos **NEVER** persisted. PII **NEVER** sold. AI output **NEVER** attributed to a real person without opt-in.

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

## 02 — The Full Vision (Read This First)

NEXXUS is an immersive single-page web experience that doubles as Nella's artist universe. The visitor crosses a threshold, lands in an atrium, and chooses between rooms. Every room reinforces a single brand idea: **Nella's IP is a world you can walk around in.**

### The Rooms
1. **7 Song-Worlds** — JEJE, DANDA KIDI, NO VISA, LUKAKU, OMG, LEFT ON READ, QUEEN. Each is a unique immersive environment with its own canvas FX, color palette, and audio.
2. **Fashion Room** — AI try-on. Fan uploads their photo, AI dresses them in Nella's wardrobe. Privacy-first: photos held in RAM only, never persisted.
3. **Rest House** — Fan guestbook / community wall. Fans leave messages, read others.
4. **Language Room** — Full experience in English, French, Pidgin, and Fèfè.
5. **Moonbeam City** — A 3D city rendered in three.js representing Nella's world.
6. **NELLA NEXUS Hub** — Streaming links, fan signup, donation/music unlock. This is a room, not the gateway.

### The Navigation Flow
```
LANDING (threshold)
  — nella-bg.mp4 video background (N lettermark)
  — intro.mp3 starts playing automatically on page load
  — centered "NELLA NEXUS" button
  — click → zoom animation → navigate to /atrium
  — audio continues into atrium

ATRIUM (room selector)
  — bluish-silver matrix rain background
  — grid of room cards: 7 song-worlds + Fashion + Rest House + Language + City + Nexus Hub
  — fan chooses a room → enters it
  — audio stops when entering any room/world

WORLD / ROOM PAGES
  — each has its own background, color palette, canvas FX
  — HUD with back button → returns to Atrium
  — world-specific audio (future phase)
```

### What /nella-nexus Becomes
The `/nella-nexus` route stays but it is now **one of the rooms** accessible from the Atrium — not the main gateway. It contains: streaming links (Spotify, Apple Music), fan signup form, donation/music unlock flow, and the photo carousel.

---

## 03 — Current State of the Project

**Do not scaffold a new project. Do not change the project name.** Read what exists before building anything.

### Files That Exist and Work — Do Not Break
- `App.razor` — global shell with Space Grotesk font, video preload, script references
- `MainLayout.razor` + `MainLayout.razor.css` — stripped layout (no sidebar, no top-row)
- `Landing.razor` + `Landing.razor.cs` + `landing.razor.css` — the threshold/portal page
- `NellaNexus.razor` + `NellaNexus.razor.cs` + `NellaNexus.razor.css` — the nexus hub room
- `Program.cs` — Stripe Checkout + Webhook API with rate limiting and HMAC verification
- `appsettings.json` — Stripe config
- `wwwroot/js/landing.js` — `nellaLanding.init()` function
- `wwwroot/videos/nella-bg.mp4` — the N lettermark video
- `wwwroot/audio/intro.mp3` — portal ambient music
- `wwwroot/audio/OMG.wav` — the unlockable track
- `wwwroot/Images/` — 4 artist photos for the nexus page carousel

### Changes Required to Existing Files

**Landing.razor.cs** — Change audio to start on page load (not on button click). Stop audio when entering a world (handled in world pages). Keep the zoom animation on button click.

**Landing.razor.cs updated audio logic:**
- On `OnAfterRenderAsync` firstRender: start `intro.mp3` automatically
- Remove audio start from `EnterNexus()` — it already plays
- Keep the zoom + navigate to `/atrium` (not `/nella-nexus`)
- `EnterNexus()` navigates to `/atrium` not `/nella-nexus`

**App.razor** — Add `<script src="js/worlds.js"></script>` to body.

---

## 04 — Full Project File Structure

```
MyBlazorApp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor              [EXISTS — no sidebar]
│   │   └── MainLayout.razor.css          [EXISTS]
│   └── Pages/
│       ├── Landing.razor                 [EXISTS — update audio + nav target]
│       ├── Landing.razor.cs              [EXISTS — update audio + nav target]
│       ├── landing.razor.css             [EXISTS]
│       ├── NellaNexus.razor              [EXISTS — now a room, not gateway]
│       ├── NellaNexus.razor.cs           [EXISTS]
│       ├── NellaNexus.razor.css          [EXISTS]
│       ├── Atrium.razor                  [BUILD — main hub/room selector]
│       ├── Atrium.razor.cs               [BUILD]
│       ├── Atrium.razor.css              [BUILD]
│       ├── World.razor                   [BUILD — /world/{slug}]
│       ├── World.razor.cs                [BUILD]
│       ├── World.razor.css               [BUILD]
│       ├── Fashion.razor                 [BUILD — /fashion]
│       ├── Fashion.razor.cs              [BUILD]
│       ├── Fashion.razor.css             [BUILD]
│       ├── RestHouse.razor               [BUILD — /rest-house]
│       ├── RestHouse.razor.cs            [BUILD]
│       ├── RestHouse.razor.css           [BUILD]
│       ├── LanguageRoom.razor            [BUILD — /language/{lang}]
│       ├── LanguageRoom.razor.cs         [BUILD]
│       ├── LanguageRoom.razor.css        [BUILD]
│       ├── City.razor                    [BUILD — /city, three.js]
│       ├── City.razor.cs                 [BUILD]
│       └── City.razor.css               [BUILD]
├── App.razor                             [EXISTS — add worlds.js script]
├── Routes.razor                          [EXISTS]
├── _Imports.razor                        [EXISTS]
├── Program.cs                            [EXISTS — Stripe API]
├── appsettings.json                      [EXISTS]
└── wwwroot/
    ├── app.css                           [EXISTS]
    ├── favicon.png                       [EXISTS]
    ├── js/
    │   ├── landing.js                   [EXISTS]
    │   └── worlds.js                    [BUILD — canvas FX per world]
    ├── videos/
    │   └── nella-bg.mp4                 [EXISTS]
    ├── audio/
    │   ├── intro.mp3                    [EXISTS — ambient portal music]
    │   └── OMG.wav                      [EXISTS — unlockable track]
    └── Images/
        └── nella-photo-[1-4].jpg        [EXISTS — nexus carousel]
```

---

## 05 — Architecture & Coding Rules

### Blazor Patterns
- `@rendermode InteractiveServer` on every page component
- Never put `@code` blocks in `.razor` files — all C# in `.razor.cs`
- All members bound in markup (`@bind`, `@onclick`, `@if` conditions) must be `public` in code-behind
- `ElementReference` fields must be `public` for `@ref` to work
- Use `JS.InvokeVoidAsync("eval", ...)` for inline JS execution
- Always use `document.getElementById(id)` in JS — never pass `ElementReference` parameters (unreliable across render cycles)
- Canvas animations store their RAF handle on `window._xxxRaf` so they can be cancelled in `DisposeAsync()`
- Timers (`System.Timers.Timer`) must be disposed in `DisposeAsync()` via `IAsyncDisposable`

### CSS Rules
- Scoped CSS (`.razor.css`) for page-specific styles
- Global styles in `wwwroot/app.css` only
- Brand colors: `--color-gold: #C9A96A` · `--color-cream: #F4E8D0` · Background: `#000000`
- Font: Space Grotesk (loaded via Google Fonts in App.razor). Fallback: Helvetica Neue, Arial
- All immersive pages: `position: fixed; inset: 0; overflow-y: auto; background: #000`
- No Bootstrap layout classes on immersive pages

### JavaScript Rules
- All JS in `wwwroot/js/` as plain `.js` files. Never inline in `.razor`
- Canvas init: always set `canvas.width/height` (attributes) AND `canvas.style.width/height`
- Always attach `window.addEventListener("resize", ...)` for canvas elements
- Global handles: `window._nellaAudio`, `window._matrixRaf`, `window._worldRaf`, `window._fashionRaf`

### Security Rules
- Stripe Secret Key: env var `STRIPE_SECRET_KEY` first, then `appsettings.json Stripe:SecretKey`
- Stripe Webhook: verify `Stripe-Signature` with HMAC-SHA256. Reject events > 5 minutes old
- `/api/verify-payment` must check PaymentStore AND call Stripe API — `?paid=true` alone NEVER unlocks
- Rate limiting: 5 req/min on `/api/create-checkout`, 30 req/min on webhook
- HTTPS enforced in production
- **Fashion Room:** photos held in RAM only. No disk write. No log lines containing image bytes.

---

## 06 — Updated Landing Page (Changes Required)

### What Changes
1. `intro.mp3` starts automatically on page load (not on button click)
2. `EnterNexus()` navigates to `/atrium` instead of `/nella-nexus`
3. Audio continues playing into the Atrium
4. Audio stops when user enters a world (handled in World.razor.cs)

### Updated Landing.razor.cs
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
            // Stop audio if user navigated back from a world
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

            // Start intro audio automatically on page load
            try
            {
                await JS.InvokeVoidAsync("eval", @"
                    (function() {
                        if (window._nellaAudio) return;
                        var a = new Audio('audio/intro.mp3');
                        a.volume = 0.8;
                        a.play().catch(function() {
                            // Autoplay blocked — wait for first user interaction
                            document.addEventListener('click', function startAudio() {
                                a.play();
                                document.removeEventListener('click', startAudio);
                            }, { once: true });
                        });
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
        }
    }

    public async Task EnterNexus()
    {
        if (IsZooming) return;
        IsZooming = true;
        StateHasChanged();

        // Zoom animation plays, audio continues
        await Task.Delay(1200);
        Nav.NavigateTo("/atrium"); // Goes to Atrium, not /nella-nexus
    }
}
```

---

## 07 — The Atrium (Main Hub)

Route: `/atrium`. The room selector — the heart of NEXXUS. Fans arrive here from the landing page and choose where to go.

### Room Cards (in order)
| # | Route | Label | Description |
|---|-------|-------|-------------|
| 01-07 | /world/{slug} | Song titles | The 7 song-worlds |
| 08 | /fashion | FASHION ROOM | AI try-on · Nella's wardrobe |
| 09 | /rest-house | REST HOUSE | Fan guestbook · Community |
| 10 | /language/en | LANGUAGE ROOM | EN · FR · Pidgin · Fèfè |
| 11 | /city | MOONBEAM CITY | 3D city · Nella's world |
| 12 | /nella-nexus | NELLA NEXUS | Stream · Support · Join |

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

        <section class="rooms-section">
            <p class="section-label">// SONG WORLDS //</p>
            <div class="worlds-grid">
                @foreach (var world in SongWorlds)
                {
                    <div class="room-card world-card fade-up" style="--delay: @(world.Index * 80)ms"
                         @onclick="() => EnterRoom(world.Route)">
                        <span class="card-num">@world.Number</span>
                        <h2 class="card-title">@world.Title</h2>
                        <p class="card-desc">@world.Mood</p>
                        <span class="card-enter">ENTER →</span>
                    </div>
                }
            </div>
        </section>

        <section class="rooms-section">
            <p class="section-label">// OTHER ROOMS //</p>
            <div class="rooms-grid">
                @foreach (var room in OtherRooms)
                {
                    <div class="room-card fade-up" style="--delay: @(room.Index * 100)ms"
                         @onclick="() => EnterRoom(room.Route)">
                        <span class="card-icon">@room.Icon</span>
                        <h2 class="card-title">@room.Title</h2>
                        <p class="card-desc">@room.Description</p>
                        <span class="card-enter">ENTER →</span>
                    </div>
                }
            </div>
        </section>
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

    public record RoomCard(int Index, string Number, string Route, string Title, string Mood, string Icon = "");

    public List<RoomCard> SongWorlds { get; } = new() {
        new(0,  "01", "/world/jeje",         "JEJE",         "Warm, celebratory, Cameroonian roots"),
        new(1,  "02", "/world/danda-kidi",   "DANDA KIDI",   "Percussive energy, fierce, rhythmic"),
        new(2,  "03", "/world/no-visa",      "NO VISA",      "Love across borders, Afrobeats + diaspora"),
        new(3,  "04", "/world/lukaku",       "LUKAKU",       "Self-worth anthem, gold, unstoppable"),
        new(4,  "05", "/world/omg",          "OMG",          "Electrifying, surprise, peak energy"),
        new(5,  "06", "/world/left-on-read", "LEFT ON READ", "Digital longing, melancholic, intimate"),
        new(6,  "07", "/world/queen",        "QUEEN",        "The closer. Regal, definitive, triumphant"),
    };

    public List<RoomCard> OtherRooms { get; } = new() {
        new(0, "", "/fashion",    "FASHION ROOM",   "AI try-on · Nella's wardrobe",     "◈"),
        new(1, "", "/rest-house", "REST HOUSE",     "Fan guestbook · Community wall",   "◉"),
        new(2, "", "/language/en","LANGUAGE ROOM",  "EN · FR · Pidgin · Fèfè",          "◎"),
        new(3, "", "/city",       "MOONBEAM CITY",  "3D city · Nella's world",          "◬"),
        new(4, "", "/nella-nexus","NELLA NEXUS",    "Stream · Support · Join the inner circle", "◆"),
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

    public void EnterRoom(string route) => Nav.NavigateTo(route);

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

.atrium-main { position: relative; z-index: 10; min-height: 100vh; display: flex; flex-direction: column; align-items: center; padding: 48px 24px 64px; gap: 48px; }

.atrium-header { text-align: center; }
.atrium-title { font-family: 'Space Grotesk', sans-serif; font-size: 2.5rem; font-weight: 700; letter-spacing: 6px; margin: 0; background: linear-gradient(135deg, #ffd700, #00e5ff, #ff00ff); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }
.atrium-sub { font-family: 'Space Grotesk', monospace; font-size: 0.72rem; letter-spacing: 3px; color: rgba(255,255,255,0.35); margin: 8px 0 0; }

.rooms-section { width: 100%; max-width: 1000px; display: flex; flex-direction: column; gap: 16px; }
.section-label { font-family: 'Space Grotesk', monospace; font-size: 0.68rem; letter-spacing: 3px; color: rgba(201,169,106,0.5); margin: 0; }

.worlds-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 14px; }
.rooms-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 14px; }

.room-card {
    padding: 24px 20px;
    background: rgba(0,0,0,0.72);
    backdrop-filter: blur(14px);
    -webkit-backdrop-filter: blur(14px);
    border: 1px solid rgba(201,169,106,0.15);
    border-radius: 18px;
    cursor: pointer;
    display: flex; flex-direction: column; gap: 6px;
    transition: transform 250ms ease, border-color 250ms ease, box-shadow 250ms ease;
}
.room-card:hover {
    transform: translateY(-3px) scale(1.02);
    border-color: rgba(201,169,106,0.55);
    box-shadow: 0 0 28px rgba(201,169,106,0.1), 0 12px 32px rgba(0,0,0,0.4);
}
.card-num { font-family: 'Space Grotesk', monospace; font-size: 0.68rem; color: rgba(201,169,106,0.5); font-weight: 600; }
.card-icon { font-size: 1.2rem; color: #C9A96A; }
.card-title { font-family: 'Space Grotesk', sans-serif; font-size: 1.2rem; font-weight: 700; color: #fff; margin: 0; letter-spacing: 1.5px; }
.card-desc { font-family: 'Space Grotesk', sans-serif; font-size: 0.78rem; color: rgba(255,255,255,0.45); margin: 0; line-height: 1.5; }
.card-enter { font-family: 'Space Grotesk', sans-serif; font-size: 0.72rem; font-weight: 600; color: #C9A96A; letter-spacing: 1px; margin-top: 6px; }

.fade-up { opacity: 0; transform: translateY(18px); transition: opacity 0.7s ease var(--delay, 0ms), transform 0.7s cubic-bezier(0.23,1,0.32,1) var(--delay, 0ms); }
.atrium-root.entered .fade-up { opacity: 1; transform: translateY(0); }

@media (max-width: 540px) {
    .atrium-title { font-size: 1.8rem; }
    .worlds-grid, .rooms-grid { grid-template-columns: 1fr 1fr; }
}
@media (max-width: 380px) {
    .worlds-grid, .rooms-grid { grid-template-columns: 1fr; }
}
```

---

## 08 — Song-World Pages (/world/{slug})

One `World.razor` handles all 7 slugs. Audio stops when entering. Each world has its own color and particle FX.

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
        ["jeje"]         = new("jeje",         "JEJE",         "Warm · Celebratory",      "Rooted in Cameroonian tradition, JEJE is a homecoming. The rhythm of Makossa carried into new worlds.",          "#C9A96A"),
        ["danda-kidi"]   = new("danda-kidi",   "DANDA KIDI",   "Percussive · Fierce",     "Raw energy. Bikutsi percussion transformed into something that hits harder than any drop.",                       "#E07B39"),
        ["no-visa"]      = new("no-visa",      "NO VISA",      "Love Without Borders",    "Love has no checkpoint. Open borders. No inspection. Afrobeats and Afrofusion in one breath.",                   "#5BA4CF"),
        ["lukaku"]       = new("lukaku",       "LUKAKU",       "Self-Worth · Unstoppable","I no fi chase. I no be Lukaku. The anthem of a woman who knows her worth.",                                      "#C9A96A"),
        ["omg"]          = new("omg",          "OMG",          "Electrifying · Peak",     "Peak frequency. The moment everything ignites.",                                                                  "#A78BFA"),
        ["left-on-read"] = new("left-on-read", "LEFT ON READ", "Digital Longing",         "The silence after the message. Melancholic, intimate, undeniably human.",                                        "#94A3B8"),
        ["queen"]        = new("queen",        "QUEEN",        "Regal · Triumphant",      "The closer. Every world was leading here. Nella as composer. The orchestra rests.",                              "#FFD700"),
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
            // Stop the intro audio when entering a world
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
.hud-title { font-family: 'Space Grotesk', sans-serif; font-size: 0.85rem; font-weight: 700; letter-spacing: 3px; color: rgba(255,255,255,0.4); }

.world-main { position: relative; z-index: 10; min-height: 100vh; display: flex; flex-direction: column; align-items: center; justify-content: center; padding: 80px 24px 40px; text-align: center; gap: 16px; }
.world-display-title { font-family: 'Space Grotesk', sans-serif; font-size: clamp(3rem, 10vw, 7rem); font-weight: 700; letter-spacing: 6px; color: var(--world-color); margin: 0; text-shadow: 0 0 40px var(--world-color); }
.world-mood-text { font-family: 'Space Grotesk', monospace; font-size: 0.85rem; letter-spacing: 3px; color: rgba(255,255,255,0.4); margin: 0; text-transform: uppercase; }
.world-desc { font-family: 'Space Grotesk', sans-serif; font-size: 1rem; color: rgba(255,255,255,0.6); margin: 0; line-height: 1.8; max-width: 480px; }

.fade-up { opacity: 0; transform: translateY(20px); transition: opacity 0.9s ease 0.2s, transform 0.9s cubic-bezier(0.23,1,0.32,1) 0.2s; }
.world-root.entered .fade-up { opacity: 1; transform: translateY(0); }
```

### wwwroot/js/worlds.js
```javascript
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
                if (p.x > w) p.x = 0; if (p.x < 0) p.x = w;
                if (p.y > h) p.y = 0; if (p.y < 0) p.y = h;
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

## 09 — Fashion Room (/fashion)

### Overview
AI try-on room. Fan uploads their photo + selects a garment from Nella's wardrobe. AI composites the result. **Privacy is sacred: photos held in RAM only, never written to disk, never logged.**

### Modes
- **Ghost-mannequin** — garment only on invisible body. Default gallery view.
- **Flat-lay** — garment styled on a surface. Share card fallback.
- **With-subject** — fan's photo + garment composed. The marquee feature.

### Fashion.razor
```razor
@page "/fashion"
@rendermode InteractiveServer

<HeadContent><PageTitle>Fashion Room — NELLA NEXUS</PageTitle></HeadContent>

<div class="fashion-root @(HasEntered ? "entered" : "")">
    <div class="fashion-scrim"></div>
    <div class="world-hud">
        <button class="hud-back" @onclick="GoBack">← ATRIUM</button>
        <span class="hud-title">FASHION ROOM</span>
    </div>
    <main class="fashion-main fade-up">
        <h1 class="fashion-title">NELLA'S WARDROBE</h1>
        <p class="fashion-sub">// SELECT A PIECE · WEAR THE FREQUENCY //</p>

        @if (ShowResult)
        {
            <div class="tryon-result fade-up-fast">
                <img src="@ResultImageUrl" class="result-img" alt="Try-on result" />
                <button class="fashion-btn" @onclick="ResetTryOn">TRY ANOTHER</button>
            </div>
        }
        else if (ShowUpload)
        {
            <div class="upload-section fade-up-fast">
                <p class="upload-label">Upload your photo to wear <strong>@SelectedItem?.Name</strong></p>
                <label class="upload-zone">
                    <input type="file" accept="image/*" @onchange="HandlePhotoUpload" class="file-input" />
                    <span>TAP TO UPLOAD PHOTO</span>
                </label>
                @if (IsProcessing)
                {
                    <p class="processing-label">// COMPOSITING THE FREQUENCY //</p>
                }
                <button class="fashion-btn-ghost" @onclick="ResetTryOn">CANCEL</button>
            </div>
        }
        else
        {
            <div class="wardrobe-grid">
                @foreach (var item in WardrobeItems)
                {
                    <div class="wardrobe-card @(SelectedItem?.Id == item.Id ? "selected" : "")"
                         @onclick="() => SelectItem(item)">
                        <div class="wardrobe-img-wrap">
                            <img src="@item.ImageUrl" alt="@item.Name" class="wardrobe-img" />
                        </div>
                        <p class="wardrobe-name">@item.Name</p>
                        <button class="try-btn">TRY ON →</button>
                    </div>
                }
            </div>
        }

        <p class="privacy-note">// Your photo is never stored. Privacy is sacred. //</p>
    </main>
</div>
```

### Fashion.razor.cs
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace MyBlazorApp.Components.Pages;

public partial class Fashion : ComponentBase
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IHttpClientFactory HttpFactory { get; set; } = default!;

    public bool HasEntered { get; set; }
    public bool ShowUpload { get; set; }
    public bool ShowResult { get; set; }
    public bool IsProcessing { get; set; }
    public string? ResultImageUrl { get; set; }

    public WardrobeItem? SelectedItem { get; set; }

    public record WardrobeItem(string Id, string Name, string ImageUrl);

    // Populate with actual wardrobe assets when available
    public List<WardrobeItem> WardrobeItems { get; } = new() {
        new("001", "The Gold Drape",     "images/wardrobe/item-001.jpg"),
        new("002", "The Cream Corset",   "images/wardrobe/item-002.jpg"),
        new("003", "The Bamiléké Wrap",  "images/wardrobe/item-003.jpg"),
        new("004", "The Nexus Jacket",   "images/wardrobe/item-004.jpg"),
        new("005", "The Ritual Gown",    "images/wardrobe/item-005.jpg"),
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Stop intro audio
            try { await JS.InvokeVoidAsync("eval", "if(window._nellaAudio){window._nellaAudio.pause();window._nellaAudio=null;}"); }
            catch { }
            HasEntered = true;
            StateHasChanged();
        }
    }

    public void SelectItem(WardrobeItem item)
    {
        SelectedItem = item;
        ShowUpload = true;
    }

    public async Task HandlePhotoUpload(ChangeEventArgs e)
    {
        // NOTE: Photo is held in memory only — never written to disk
        // Future: send to /api/tryon endpoint with garment ID
        // For now: show placeholder result
        IsProcessing = true;
        StateHasChanged();
        await Task.Delay(2000); // Simulate processing
        ResultImageUrl = SelectedItem?.ImageUrl; // Replace with actual AI result URL
        IsProcessing = false;
        ShowUpload = false;
        ShowResult = true;
        StateHasChanged();
    }

    public void ResetTryOn()
    {
        ShowUpload = false;
        ShowResult = false;
        SelectedItem = null;
        ResultImageUrl = null;
    }

    public void GoBack() => Nav.NavigateTo("/atrium");
}
```

---

## 10 — Rest House (/rest-house)

Fan guestbook and community wall. Fans leave messages and read others.

### RestHouse.razor
```razor
@page "/rest-house"
@rendermode InteractiveServer

<HeadContent><PageTitle>Rest House — NELLA NEXUS</PageTitle></HeadContent>

<div class="resthouse-root @(HasEntered ? "entered" : "")">
    <canvas id="restHouseCanvas" class="resthouse-canvas"></canvas>
    <div class="resthouse-scrim"></div>
    <div class="world-hud">
        <button class="hud-back" @onclick="GoBack">← ATRIUM</button>
        <span class="hud-title">REST HOUSE</span>
    </div>
    <main class="resthouse-main fade-up">
        <h1 class="resthouse-title">THE GUESTBOOK</h1>
        <p class="resthouse-sub">// LEAVE YOUR FREQUENCY HERE //</p>

        <div class="guestbook-form">
            <input type="text" placeholder="Your name" @bind="GuestName" @bind:event="oninput" class="form-input" maxlength="50" />
            <textarea placeholder="Leave a message for Nella..." @bind="GuestMessage" @bind:event="oninput" class="form-textarea" maxlength="280"></textarea>
            <button class="submit-btn" @onclick="SubmitEntry" disabled="@(IsSubmitting || string.IsNullOrWhiteSpace(GuestMessage))">
                @(IsSubmitting ? "TRANSMITTING…" : "LEAVE YOUR MARK →")
            </button>
        </div>

        <div class="entries-list">
            @foreach (var entry in Entries)
            {
                <div class="entry-card">
                    <span class="entry-name">@entry.Name</span>
                    <p class="entry-message">@entry.Message</p>
                    <span class="entry-time">@entry.TimeAgo</span>
                </div>
            }
        </div>
    </main>
</div>
```

### RestHouse.razor.cs
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyBlazorApp.Components.Pages;

public partial class RestHouse : ComponentBase, IAsyncDisposable
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    public bool HasEntered { get; set; }
    public bool IsSubmitting { get; set; }
    public string GuestName { get; set; } = "";
    public string GuestMessage { get; set; } = "";

    public record GuestEntry(string Name, string Message, string TimeAgo);

    // In-memory for now — replace with database in production
    public List<GuestEntry> Entries { get; set; } = new() {
        new("Amara", "The frequency found me. JEJE changed everything.", "2 days ago"),
        new("Kemi", "237 in the building. Proud of you Nella.", "3 days ago"),
        new("Marcus", "NO VISA been on repeat for a week straight.", "5 days ago"),
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try { await JS.InvokeVoidAsync("eval", "if(window._nellaAudio){window._nellaAudio.pause();window._nellaAudio=null;}"); }
            catch { }
            HasEntered = true;
            StateHasChanged();
        }
    }

    public async Task SubmitEntry()
    {
        if (IsSubmitting || string.IsNullOrWhiteSpace(GuestMessage)) return;
        IsSubmitting = true;
        StateHasChanged();

        await Task.Delay(800);

        var name = string.IsNullOrWhiteSpace(GuestName) ? "Anonymous" : GuestName.Trim();
        Entries.Insert(0, new GuestEntry(name, GuestMessage.Trim(), "Just now"));

        GuestName = "";
        GuestMessage = "";
        IsSubmitting = false;
        StateHasChanged();
    }

    public void GoBack() => Nav.NavigateTo("/atrium");

    public async ValueTask DisposeAsync()
    {
        try { await JS.InvokeVoidAsync("eval", "if(window._restRaf)cancelAnimationFrame(window._restRaf);"); }
        catch { }
    }
}
```

---

## 11 — Language Room (/language/{lang})

Full NEXXUS experience in four languages. **Language order is fixed. Always spell Fèfè with the grave accent.**

### Supported Languages (exact order)
1. `en` — English
2. `fr` — French  
3. `ln` — Pidgin
4. `fefe` — Fèfè

### LanguageRoom.razor
```razor
@page "/language/{Lang}"
@rendermode InteractiveServer

<HeadContent><PageTitle>Language Room — NELLA NEXUS</PageTitle></HeadContent>

<div class="lang-root @(HasEntered ? "entered" : "")">
    <div class="lang-scrim"></div>
    <div class="world-hud">
        <button class="hud-back" @onclick="GoBack">← ATRIUM</button>
        <span class="hud-title">LANGUAGE ROOM</span>
    </div>
    <main class="lang-main fade-up">
        <h1 class="lang-title">@CurrentContent?.Greeting</h1>
        <p class="lang-sub">@CurrentContent?.Tagline</p>

        <div class="lang-selector">
            @foreach (var lang in Languages)
            {
                <button class="lang-btn @(Lang == lang.Code ? "active" : "")"
                        @onclick="() => SwitchLanguage(lang.Code)">
                    @lang.Label
                </button>
            }
        </div>

        <p class="lang-body">@CurrentContent?.Body</p>
    </main>
</div>
```

### LanguageRoom.razor.cs
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyBlazorApp.Components.Pages;

public partial class LanguageRoom : ComponentBase
{
    [Parameter] public string Lang { get; set; } = "en";
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    public bool HasEntered { get; set; }

    public record LangOption(string Code, string Label);
    public record LangContent(string Greeting, string Tagline, string Body);

    // Language order is FIXED — do not reorder
    public List<LangOption> Languages { get; } = new() {
        new("en",   "English"),
        new("fr",   "Français"),
        new("ln",   "Pidgin"),
        new("fefe", "Fèfè"),
    };

    private static readonly Dictionary<string, LangContent> Content = new() {
        ["en"]   = new("Welcome to the Nexus.", "// THE FREQUENCY IS UNIVERSAL //", "Nella's world speaks every language. Choose yours and step inside the frequency."),
        ["fr"]   = new("Bienvenue au Nexus.", "// LA FRÉQUENCE EST UNIVERSELLE //", "Le monde de Nella parle toutes les langues. Choisissez la vôtre et entrez dans la fréquence."),
        ["ln"]   = new("Welcome for Nexus.", "// DI FREQUENCY NA FOR EVRIBODI //", "Nella im world sabi all language. Choose your own make you enter inside di frequency."),
        ["fefe"] = new("Bɔ́ŋ Nexus.", "// FÉ FRÉQUENCE Á YƐLƐ MBɄ̌ //", "Nella ɔ chem á yɛ ndɔ mɔ mbʉ̌. Líʔ yɛ lɛ ɔ chíŋ á mbʉ̌ tɔ."),
    };

    public LangContent? CurrentContent => Content.TryGetValue(Lang, out var c) ? c : Content["en"];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try { await JS.InvokeVoidAsync("eval", "if(window._nellaAudio){window._nellaAudio.pause();window._nellaAudio=null;}"); }
            catch { }
            HasEntered = true;
            StateHasChanged();
        }
    }

    public void SwitchLanguage(string code) => Nav.NavigateTo($"/language/{code}");
    public void GoBack() => Nav.NavigateTo("/atrium");
}
```

---

## 12 — Moonbeam City (/city)

A 3D city environment rendered in three.js. Represents Nella's world literally — a place fans can walk around in.

**If three.js is out of scope for initial build:** replace with a 2D illustrated placeholder + CSS parallax effect. Keep the route and page shell — just swap the implementation.

### City.razor
```razor
@page "/city"
@rendermode InteractiveServer

<HeadContent><PageTitle>Moonbeam City — NELLA NEXUS</PageTitle></HeadContent>

<div class="city-root @(HasEntered ? "entered" : "")">
    <canvas id="cityCanvas" class="city-canvas"></canvas>
    <div class="world-hud">
        <button class="hud-back" @onclick="GoBack">← ATRIUM</button>
        <span class="hud-title">MOONBEAM CITY</span>
    </div>
    <main class="city-main fade-up">
        <h1 class="city-title">MOONBEAM CITY</h1>
        <p class="city-sub">// NELLA'S WORLD · WALK AROUND IN IT //</p>
        <p class="city-coming">Three.js city — coming soon.</p>
    </main>
</div>
```

### City.razor.cs
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyBlazorApp.Components.Pages;

public partial class City : ComponentBase, IAsyncDisposable
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    public bool HasEntered { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try { await JS.InvokeVoidAsync("eval", "if(window._nellaAudio){window._nellaAudio.pause();window._nellaAudio=null;}"); }
            catch { }
            // Future: initialize three.js scene here
            // try { await JS.InvokeVoidAsync("nellaCityInit", "cityCanvas"); } catch { }
            HasEntered = true;
            StateHasChanged();
        }
    }

    public void GoBack() => Nav.NavigateTo("/atrium");

    public async ValueTask DisposeAsync()
    {
        try { await JS.InvokeVoidAsync("eval", "if(window._cityRaf)cancelAnimationFrame(window._cityRaf);"); }
        catch { }
    }
}
```

---

## 13 — NELLA NEXUS Hub (/nella-nexus)

This page is now a room accessed from the Atrium. It is no longer the gateway. All existing code stays — streaming links, photo carousel, fan signup form, donation/music unlock flow.

**No changes needed to NellaNexus.razor, NellaNexus.razor.cs, or NellaNexus.razor.css.**

The only addition: stop the intro audio on page load (same pattern as all other rooms).

Add this to `NellaNexus.razor.cs` in `OnAfterRenderAsync` firstRender, before the matrix rain init:

```csharp
// Stop intro audio when entering the Nexus Hub
try { await JS.InvokeVoidAsync("eval", "if(window._nellaAudio){window._nellaAudio.pause();window._nellaAudio=null;}"); }
catch { }
```

---

## 14 — Stripe & Payments (Existing — Do Not Change)

The Stripe integration in `Program.cs` is complete and secure. Do not modify it unless fixing a bug.

### What Exists
- `POST /api/create-checkout` — creates Stripe Checkout session, validates amount ($5–$10,000), rate limited 5/min
- `POST /api/stripe-webhook` — verifies Stripe-Signature with HMAC-SHA256, confirms payment in PaymentStore
- `GET /api/verify-payment?sessionId=xxx` — server-side payment verification before showing tracklist
- `PaymentStore` — in-memory session store with 2-hour auto-cleanup

### Stripe Dashboard Setup (one-time)
1. Stripe Dashboard → Developers → Webhooks → Add Endpoint
2. URL: `https://yourdomain.com/api/stripe-webhook`
3. Event: `checkout.session.completed`
4. Copy `whsec_...` into `appsettings.json → Stripe:WebhookSecret`

### Keys
- **Publishable key** (frontend only): `pk_live_51TQARuJ7NMEnEoS3nrnGVyiMoUZBWQ7kV97gEaw3gHpTfzoK9HtRwGx5KJXaMjxvCAIiZU9SUrPMWz2IVoMf8f1800aFjBgRWg`
- **Secret key**: set in `appsettings.json → Stripe:SecretKey` or env var `STRIPE_SECRET_KEY`
- **Never commit real secret keys to git**

---

## 15 — Brand Design Tokens

| Token | Value | Usage |
|-------|-------|-------|
| `--color-gold` | `#C9A96A` | Primary accent, borders, glows |
| `--color-cream` | `#F4E8D0` | Table headers, subtle fills |
| `--color-black` | `#000000` | Page backgrounds |
| `--color-dark` | `#050508` | Card backgrounds |
| `--font-primary` | Space Grotesk | All UI text |
| `--font-mono` | monospace | Protocol tags, code labels |

> No purple. No neon. No gradients outside the NELLA logo. Gold and cream ONLY for accents.

---

## 16 — Build Order (Recommended)

Build in this order. Each step is deployable and testable before moving to the next.

1. **Update Landing.razor.cs** — auto-start audio, navigate to `/atrium`
2. **Build Atrium** — the hub with all room cards
3. **Update World.razor** — stop audio on enter, back to atrium
4. **Update NellaNexus.razor.cs** — stop audio on enter
5. **Build Fashion.razor** — wardrobe gallery + upload shell (AI integration placeholder)
6. **Build RestHouse.razor** — guestbook form + in-memory entries
7. **Build LanguageRoom.razor** — 4 language toggle
8. **Build City.razor** — placeholder shell (three.js later)
9. **Add worlds.js** to wwwroot/js and reference in App.razor

---

## 17 — Deployment & Local Run

```bash
cd MyBlazorApp
dotnet run
# http://localhost:5070
```

**Stripe test card:** `4242 4242 4242 4242` · any future date · any CVC

---

*"A place, not a page."*  
*"AI as instrument. Nella as composer."*  
*© 2025 NELLA · 237 x HTX · ALL FREQUENCIES RESERVED*
