# Hiyakasudere v2.0 - Avalonia Edition

## Overview

**Hiyakasudere** is a cross-platform native desktop application for browsing anime/artwork from multiple imageboard (booru) APIs. Built with **Avalonia UI** for true cross-platform support on Windows, Linux, and macOS.

Available on the [Microsoft Store](https://apps.microsoft.com/store/detail/hiyakasudere/9MZ99G4SQ4ZD).

---

## Architecture

- **Framework:** Avalonia UI 11.2 + ReactiveUI (MVVM)
- **Target:** .NET 8 — Windows, Linux, macOS
- **Database:** SQLite via Entity Framework Core
- **Image Loading:** AsyncImageLoader.Avalonia
- **Theme:** Custom dark Fluent theme with accent colors

### Project Structure
```
Hiyakasudere/
├── Hiyakasudere.sln
├── ROADMAP.md
├── README.md
└── Hiyakasudere/
    ├── Program.cs                 # Entry point
    ├── App.axaml(.cs)             # DI container, app bootstrap
    ├── Hiyakasudere.csproj        # Project file (net8.0)
    ├── Styles/AppStyles.axaml     # Modern dark theme
    ├── Views/                     # AXAML UI views
    │   ├── MainWindow.axaml       # Shell: sidebar + content
    │   ├── BrowseView.axaml       # Image grid + search
    │   ├── FavoritesView.axaml    # Saved posts
    │   └── SettingsView.axaml     # Configuration
    ├── ViewModels/                # MVVM logic (ReactiveUI)
    ├── Data/
    │   ├── ExternalAPI/           # Booru API clients
    │   │   ├── Yandere/
    │   │   ├── Safebooru/
    │   │   ├── Konachan/
    │   │   ├── Gelbooru/
    │   │   └── Rule34/
    │   └── Internal/
    │       ├── Config/            # App settings persistence
    │       ├── Database/          # SQLite (Favorites, Search History)
    │       ├── Data/Post/         # Internal post model + translation
    │       ├── Functionality/     # Image utilities
    │       └── MultiplatformInterfaces/  # Cross-platform file I/O
    └── Packaging/                 # MS Store MSIX manifest & info
```

---

## Supported Sources

- [Yande.re](https://yande.re/) — JSON API
- [Safebooru](https://safebooru.org/) — XML API
- [Konachan](https://konachan.com/) — JSON API
- [Gelbooru](https://gelbooru.com/) — XML API
- [Rule34](https://rule34.xxx) — XML API

---

## Features

### Implemented (v2.0)
- [x] Multi-source image browsing (5 booru sites)
- [x] Source-specific tag autocomplete
- [x] Local favorites system (SQLite)
- [x] Search history with quick-access chips
- [x] Modern dark UI with Fluent design
- [x] Cross-platform (Windows/Linux/macOS)
- [x] Async image loading with caching
- [x] NSFW content filtering
- [x] Tag blacklisting
- [x] Configurable posts per page
- [x] Image save to Pictures folder
- [x] Pagination with prev/next controls
- [x] Error handling with user-facing messages
- [x] MS Store publishing support (MSIX)

### Planned (v2.x)
- [ ] Batch download with filename patterns
- [ ] Tag category highlighting (artist/character/copyright colors)
- [ ] Slideshow/gallery mode
- [ ] Video/GIF/WebM post support
- [ ] Keyboard shortcuts
- [ ] Dark/Light theme toggle
- [ ] Image zoom/pan in viewer
- [ ] Export/Import favorites
- [ ] Download progress indicator

---

## Building

### Prerequisites
- .NET 8 SDK

### Run (any platform)
```bash
cd Hiyakasudere
dotnet run
```

### Publish
```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained -o ./publish/win

# Linux
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish/linux

# macOS (Intel)
dotnet publish -c Release -r osx-x64 --self-contained -o ./publish/osx

# macOS (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained -o ./publish/osx-arm
```

### MS Store (MSIX)
See `Hiyakasudere/Packaging/README.md` for MSIX packaging instructions.

---

## Migration Notes (v1.x MAUI -> v2.0 Avalonia)

The app was migrated from .NET MAUI Blazor Hybrid to Avalonia UI for:
- **Linux support** — MAUI has no Linux target
- **Smaller footprint** — No WebView overhead, native Skia rendering
- **Better maintenance** — Avalonia is commercially backed with active development
- **Single codebase** — One project builds for all platforms (no conditional compilation)

All data layer code (API services, models, database) was preserved unchanged.
The UI was rebuilt from scratch in AXAML with a modern design system.

MS Store identity preserved in `Packaging/Package.appxmanifest`.

---

## License

MIT
