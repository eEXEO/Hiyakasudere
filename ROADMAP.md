# Hiyakasudere - Analysis & Improvement Roadmap

## Overview

**Hiyakasudere** is a .NET MAUI Blazor Hybrid desktop application — a booru image browser/client for Windows. It lets users browse anime/artwork from multiple imageboard APIs (Yandere, Safebooru, Konachan, Gelbooru, Rule34), search by tags with autocomplete, view full-resolution images in modals, and save them locally.

Published on the Microsoft Store with two branches:
- `main` — uncensored version
- `msstore` — "safe" version with NSFW content filtered

---

## Existing Bugs (Fixed in Phase 1)

| Bug | Location | Severity |
|-----|----------|----------|
| `KonachanPostService.GetKonachanPostCount()` queries yande.re URL instead of konachan.com | `KonachanPostService.cs` | **High** |
| `PostInternal` constructor: `SampleWidth = sampleHeight` (assigns height to width) | `PostInternal.cs` | **Medium** |
| `ConfigDataModel.SelectedSource` has `[Range(1, 2)]` but 5 sources exist | `ConfigDataModel.cs` | **Medium** |
| Tag autocomplete always uses Yandere API regardless of selected source | `PostTranslationService.cs` | **Medium** |
| Gelbooru/Rule34 date parsing uses fragile `IndexOf` string manipulation | `PostTranslationService.cs` | **Low-Med** |

---

## Architecture Concerns

1. **`HttpClient` anti-pattern** — Each service creates its own `HttpClient` instance. Should use `IHttpClientFactory` to prevent socket exhaustion.
2. **Images loaded as base64 strings** — Full images downloaded, converted to base64, then rendered as data URIs. Doubles memory usage.
3. **No error handling UI** — All exceptions swallowed with `Debug.WriteLine`. Users see infinite loading spinners.
4. **Race condition in config loading** — Busy-waits with `Task.Delay(50)`. Should use proper async synchronization.
5. **No caching** — Every page navigation re-downloads all thumbnails and data.
6. **Windows-only despite MAUI** — `IFileManager` only has a Windows implementation.

---

## Feature Roadmap

### Phase 1 — Bug Fixes & Stability (Current)
- [x] Fix all 5 bugs listed above
- [x] Add proper error handling & user-facing error messages
- [x] Fix base64 image loading to use direct URLs for thumbnails

### Phase 2 — Core UX Improvements
- [ ] Local favorites system (SQLite/LiteDB)
- [ ] Image caching layer
- [ ] Source-specific tag autocomplete
- [ ] Dark/Light theme toggle
- [ ] Responsive column count (adapt to window width)
- [ ] Infinite scroll or better pagination (page jump)
- [ ] Download progress indicator

### Phase 3 — Feature Parity with Competitors
- [ ] Batch download with filename patterns
- [ ] Tag category highlighting (artist=red, character=green, copyright=purple, general=blue)
- [ ] Saved searches & search history
- [ ] Slideshow/gallery mode (swipe through images)
- [ ] Video/GIF/WebM post support
- [ ] Keyboard shortcuts (arrows, Escape, S to save)
- [ ] Multi-source simultaneous search
- [ ] Import/Export settings & favorites

### Phase 4 — Architecture Modernization
- [ ] Upgrade to .NET 8+
- [ ] Replace Newtonsoft.Json with System.Text.Json
- [ ] IHttpClientFactory + Microsoft.Extensions.Http.Resilience
- [ ] Strategy/Adapter pattern for booru services (`IBooruAdapter`)
- [ ] Re-enable Android/macOS targets
- [ ] Proper state management (Fluxor or custom state containers)

---

## Competitor Feature Comparison

Features found in leading booru clients (Boorusama, Imgbrd-Grabber, Boorusphere, Flexbooru):

| Feature | Boorusama | Imgbrd-Grabber | Boorusphere | Hiyakasudere |
|---------|-----------|----------------|-------------|--------------|
| Multi-source support | Yes (12+) | Yes (100+) | Yes | Yes (5) |
| Tag autocomplete | Yes | Yes | Yes | Partial* |
| Tag highlighting | Yes | Yes | No | No |
| Favorites/Bookmarks | Yes | Yes | Yes | No |
| Batch download | Yes | Yes | No | No |
| Custom filename patterns | Yes | Yes | No | No |
| Infinite scroll | Yes | No | Yes | No |
| Dark/Light theme | Yes | Yes | Yes | No |
| Video support | Yes | Yes | Yes | No |
| Saved searches | Yes | Yes | No | No |
| Search history | Yes | No | Yes | No |
| Gallery/slideshow mode | Yes | No | Yes | No |
| Gesture support | Yes | N/A | Yes | No |
| Keyboard shortcuts | N/A | Yes | N/A | No |

*Currently only uses Yandere tags regardless of source

---

## Technical Notes

- **Framework:** .NET 6 MAUI Blazor Hybrid (WebView-based UI)
- **Target:** Windows 10.0.19041+
- **UI:** Bootstrap 5 + Blazored.Modal + Blazored.Typeahead
- **Version:** 1.1.1
- **Config storage:** JSON file in Windows Roaming AppData
- **Image save location:** `{Pictures Library}/Hiyakasudere/`

---

## Linux Build Feasibility Analysis

**.NET MAUI does NOT officially support Linux.** Microsoft's supported platforms are Windows, Android, iOS, and macOS only. However, there are viable paths to bring Hiyakasudere to Linux:

### Option 1: Photino.Blazor (Recommended for this project)
- **[Photino.Blazor](https://github.com/tryphotino/photino.Blazor)** is a lightweight framework for building .NET desktop apps using Blazor Web UI that works cross-platform on Windows, Linux, and macOS.
- Since Hiyakasudere already uses Blazor for all UI (Razor components + Bootstrap), the Blazor code could be reused almost entirely.
- Would require creating a separate project (`Hiyakasudere.Linux`) that hosts the same Razor components in a Photino window instead of MAUI's WebView.
- **Effort:** Medium — shared Razor class library + platform-specific host projects.

### Option 2: Avalonia + BlazorWebView
- **[Avalonia UI](https://avaloniaui.net/)** now supports embedding a BlazorWebView, enabling Blazor Hybrid apps on Linux.
- The [Baksteen.Avalonia.Blazor](https://github.com/jpmikkers/Baksteen.Avalonia.Blazor) package provides this capability.
- In March 2026, Avalonia previewed official MAUI support for Linux and WebAssembly, though adoption is still early.
- **Effort:** Medium-High — need to swap the MAUI host for Avalonia while keeping Blazor components.

### Option 3: MauiGtk (Community/Experimental)
- **[maui-linux](https://github.com/MauiGtk/maui-linux)** is a community fork adding GTK-based Linux support to MAUI.
- Experimental and not production-ready. BlazorWebView support is an open issue.
- **Effort:** Low (if it works) but **high risk** of instability.

### Recommended Approach for Hiyakasudere
1. **Extract shared Razor component library** — Move all Pages, Modals, Data services into a shared .NET class library (`Hiyakasudere.Shared`).
2. **Keep MAUI host for Windows** — `Hiyakasudere.Windows` references the shared library.
3. **Create Photino host for Linux** — `Hiyakasudere.Linux` uses Photino.Blazor to host the same Razor components.
4. **Platform abstraction** — `IFileManager` gets a Linux implementation using standard `System.IO` APIs.

This architecture also makes future macOS and even web (WASM) deployment possible.

---

## License

MIT
