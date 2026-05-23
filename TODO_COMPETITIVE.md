# Hiyakasudere — What's Needed to Be Competitive on MS Store

## Current State (v2.0 — Avalonia Edition)

The app has been fully migrated from .NET MAUI Blazor to **Avalonia UI 11.2** with ReactiveUI MVVM.
It builds for **Windows, Linux, and macOS** from a single codebase.

Working features: multi-source browsing (Konachan, Safebooru confirmed working), favorites (SQLite), search history, infinite scroll infrastructure, image viewer with keyboard nav, API key support for Gelbooru/Rule34.

---

## Critical Issues to Fix Before Release

### UI Bugs (Must Fix)

| Issue | Description | Priority |
|-------|-------------|----------|
| **Row height bug** | `WrapPanel` sets row height to tallest item. Need true masonry panel (custom `Panel` subclass or column-based layout) | **P0** |
| **No window title bar** | `ExtendClientAreaToDecorationsHint="True"` removes native bar but no custom drag handle was added. Window can't be moved. Min/Max/Close overlap content | **P0** |
| **Sidebar active state** | Currently no visual indicator of which tab is selected. Need to bind `Classes.active` to `ActiveNav` property | **P1** |
| **No tag display** | Tags aren't shown anywhere in browse view or image viewer. Need tag chips in viewer toolbar or as tooltip | **P1** |

### API Issues

| Issue | Description | Priority |
|-------|-------------|----------|
| **Yande.re blocked** | Some ISPs block yande.re. Need timeout + graceful "source unavailable" message, possibly auto-fallback | **P1** |
| **Gelbooru/Rule34 require API keys** | Users must register accounts to use these sources. Need clear onboarding UX in Settings | **P1** |
| **Safebooru empty fields** | API returns empty strings for score/dimensions. Already handled with TryParse but thumbnails may show "0" | **P2** |

---

## Features Needed for Competitive MS Store App

### Must-Have (v2.1)

- [ ] **True masonry/waterfall layout** — Custom panel that distributes items across columns by shortest-column-first algorithm. Fixed column width (200-240px), variable height preserving aspect ratio
- [ ] **Custom title bar** — Drag handle area + app title + minimize/maximize/close buttons styled to match dark theme
- [ ] **Sidebar active indicator** — Left border accent or background highlight on current nav item
- [ ] **Tag display** — Show tags as colored chips in image viewer. Clickable to search
- [ ] **Tag category colors** — Artist (red), Character (green), Copyright (purple), General (blue), Meta (orange)
- [ ] **Image save confirmation** — Toast notification when image is saved successfully
- [ ] **Source status indicator** — Show which sources are reachable on startup (green/red dots in settings)
- [ ] **Loading skeleton** — Show placeholder cards while images load instead of empty space

### Should-Have (v2.2)

- [ ] **Batch download** — Select multiple images, download all with customizable filename pattern (`{source}_{id}_{artist}.{ext}`)
- [ ] **Download queue** — Background download manager with progress
- [ ] **Slideshow mode** — Auto-advance through images with configurable interval
- [ ] **Related posts** — Show parent/child posts if available
- [ ] **Export/Import** — Backup and restore favorites + settings as JSON
- [ ] **Tag blacklist UI** — Visual management of blacklisted tags with add/remove
- [ ] **Image info panel** — Slide-out panel showing full metadata (dimensions, file size, source URL, all tags, date)
- [ ] **Right-click context menu** — Copy URL, Open in browser, Add to favorites, Save, Copy tags

### Nice-to-Have (v2.3+)

- [ ] **Dark/Light/System theme toggle**
- [ ] **Custom accent color picker**
- [ ] **Video/GIF/WebM support** — Detect animated posts, use MediaPlayer control
- [ ] **Multi-source search** — Query all sources simultaneously, merge and deduplicate results
- [ ] **Tag suggestions** — "You might also like" based on frequently searched tags
- [ ] **Collections** — Group favorites into named folders/albums
- [ ] **NSFW blur** — Show blurred thumbnails for questionable/explicit with click-to-reveal
- [ ] **Proxy/VPN support** — Allow users to configure HTTP proxy for blocked sources
- [ ] **Auto-update** — Check for new versions on GitHub releases

---

## Performance Optimizations Needed

- [ ] **Virtualized list** — Replace `ItemsControl` + `WrapPanel` with virtualized panel that only renders visible items. Critical for 100+ posts
- [ ] **Image memory management** — Unload off-screen images, keep only visible + buffer
- [ ] **Response caching** — Cache API responses for 5 minutes to avoid re-fetching on back navigation
- [ ] **Thumbnail disk cache** — AsyncImageLoader already caches to memory; add disk persistence
- [ ] **Debounced search** — Don't fire autocomplete on every keystroke; wait 300ms after last input
- [ ] **Parallel page loading** — Pre-fetch next page while user is viewing current

---

## MS Store Specific Requirements

- [ ] **Privacy Policy** — Required for store listing. Document what data is collected (none, just local SQLite)
- [ ] **Store screenshots** — 5+ high-quality screenshots showing browse, viewer, favorites, settings
- [ ] **Store description** — Feature list, supported sources, keyboard shortcuts
- [ ] **Age rating** — Content is user-generated and potentially NSFW. Need appropriate IARC rating
- [ ] **MSIX package** — Build pipeline: `dotnet publish` → `makeappx` → `signtool` → upload
- [ ] **Version bumping** — Increment version in csproj for each store submission
- [ ] **Crash reporting** — Consider adding Sentry or AppCenter for crash analytics
- [ ] **Telemetry opt-in** — Basic usage stats (which sources are popular) for prioritizing development

---

## Competitor Gap Analysis

| Feature | Boorusama | Imgbrd-Grabber | **Hiyakasudere** |
|---------|-----------|----------------|------------------|
| Sources | 12+ | 100+ | 5 (3 working without auth) |
| Tag colors | Yes | Yes | **No** |
| Batch download | Yes | Yes | **No** |
| Infinite scroll | Yes | No | **Partially** (needs virtualization) |
| Masonry grid | Yes | No | **Broken** (WrapPanel issue) |
| Favorites | Yes | Yes | **Yes** |
| Search history | Yes | No | **Yes** |
| Dark theme | Yes | Yes | **Yes** |
| Cross-platform | Android only | Win/Mac/Linux | **Win/Mac/Linux** |
| Video support | Yes | Yes | **No** |
| Gallery/slideshow | Yes | No | **Partially** (viewer exists) |
| Keyboard shortcuts | N/A | Yes | **Yes** |
| MS Store | No | No | **Yes** (unique advantage!) |

### Our Unique Advantages:
1. **Only booru client on MS Store** — zero competition in that channel
2. **Native cross-platform** — Not Electron, not WebView. Actual Skia rendering = fast + low memory
3. **Modern .NET 8** — Latest platform, best performance
4. **Dark-first design** — Matches what anime/art community expects

---

## Recommended Priority Order

1. Fix window title bar (P0 — app is unusable without it)
2. Fix masonry layout (P0 — visual quality)
3. Add sidebar active state (P1 — basic UX)
4. Add tag display in viewer (P1 — core feature)
5. Virtualize the image list (performance)
6. Add loading skeletons (perceived performance)
7. Batch download (differentiator)
8. Tag colors (polish)
9. MS Store screenshots + listing
10. Publish v2.1

---

## Linux Build

Already fully supported:
```bash
cd Hiyakasudere/Hiyakasudere
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish/linux
```

For macOS:
```bash
dotnet publish -c Release -r osx-x64 --self-contained -o ./publish/osx
dotnet publish -c Release -r osx-arm64 --self-contained -o ./publish/osx-arm
```

---

## Tech Stack Summary

- **UI Framework:** Avalonia UI 11.2.6
- **Architecture:** MVVM (ReactiveUI)
- **Database:** SQLite (EF Core 8)
- **Image Loading:** AsyncImageLoader.Avalonia
- **HTTP:** Shared HttpClient with SocketsHttpHandler (15s timeout, connection pooling)
- **Target:** .NET 8 (LTS)
- **Platforms:** Windows 10+, Linux (x64), macOS (x64/arm64)
