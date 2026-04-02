# AGENTS.md — Carnac-w-Mouse

Instructions for AI coding agents working on this repository.

## Project Overview

Carnac-w-mouse is a **keystroke and mouse click visualizer** for Windows.
It renders an always-on-top transparent WPF overlay showing pressed keys and mouse clicks in real time.

- **Language**: C# 13
- **Framework**: .NET 10 LTS (`net10.0-windows`)
- **UI**: WPF (required — only .NET UI tech supporting transparent click-through overlays on Windows)
- **Branch**: `beta` (default)

## Solution & Projects

Solution file: `src/w4b-carnac.sln`

| Project | Path | Role |
|---------|------|------|
| `w4b.carnac` | `src/w4b.carnac/` | WPF application — UI, DI composition root, tray icon |
| `w4b.carnac.logic` | `src/w4b.carnac.logic/` | Domain layer — key/mouse hooks, message pipeline, models |
| `w4b.carnac.tests` | `src/w4b.carnac.tests/` | Unit tests (xUnit + NSubstitute + Shouldly) |
| `w4b.key.stream.capture` | `src/w4b.key.stream.capture/` | Dev utility — captures key streams for test data |

## Build Commands

```powershell
# IMPORTANT: ensure .NET 10 SDK is first in PATH
$env:PATH = "C:\Program Files\dotnet;" + $env:PATH

# Restore (uses local NuGet.Config to bypass VS offline config permissions)
dotnet restore src/w4b-carnac.sln -p:RestoreConfigFile="NuGet.Config"

# Build
dotnet build src/w4b-carnac.sln --no-restore --nologo

# Test
dotnet test src/w4b.carnac.tests/w4b.carnac.tests.csproj --no-build --nologo

# Publish single-file exe
dotnet publish src/w4b.carnac/w4b.carnac.csproj -c Release
```

### NuGet Restore Workaround

The machine has a locked VS offline NuGet config at `C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config` that causes permission errors.
Always pass `-p:RestoreConfigFile="NuGet.Config"` (pointing to the repo root `NuGet.Config`) to bypass it.

## Architecture

See `docs/ARCHITECTURE.md` for the full data flow diagram and dependency map.

### Key Patterns

1. **Dependency Injection** — `Microsoft.Extensions.Hosting` + `IServiceProvider`. All services registered in `App.xaml.cs` via `Host.CreateDefaultBuilder().ConfigureServices(...)`.

2. **MVVM with Source Generators** — `CommunityToolkit.Mvvm` 8.4.1. ViewModels and models use `[ObservableProperty]` on `private` fields (generates public properties with change notification). Classes using it must be `partial`. Base class: `NotifyPropertyChanged` (thin wrapper over `ObservableObject`).

3. **Reactive Streams** — `System.Reactive` 6.0. The entire key/mouse event pipeline is `IObservable<T>` chains. Key operators: `Merge`, `Where`, `Select`, `Scan`, `SelectMany`, `ObserveOn`.

4. **Win32 Interop** — Keyboard hooks via `SetWindowsHookEx` (P/Invoke in `InterceptKeys`). Mouse hooks via native `WH_MOUSE_LL` P/Invoke in `InterceptMouse`. Window transparency via `SetWindowExTransparent` in `Win32Methods`.

5. **Structured Logging** — Serilog writing to `%LOCALAPPDATA%/Carnac/logs/carnac-{date}.log` (rolling daily, 7-day retention). Access via `Serilog.Log` static or `ILogger<T>` from DI.

6. **Channel-Based Decoupling** — `System.Threading.Channels` in both `InterceptKeys` and `InterceptMouse` decouple Win32 hook callbacks from the Rx observable pipeline, preventing hook-timeout issues under load.

### Service Registration (DI)

All services are registered as **singletons** in `App.xaml.cs`, except:
- `PreferencesViewModel` → **transient** (new instance per preferences window)
- `InterceptKeys.Current` → registered as an **existing instance** (static singleton)
- `InterceptMouse.Current` → registered as an **existing instance** (static singleton)

### Interface → Implementation Map

| Interface | Implementation | Project |
|-----------|---------------|---------|
| `IKeyProvider` | `KeyProvider` | logic |
| `IMessageProvider` | `MessageProvider` | logic |
| `IShortcutProvider` | `ShortcutProvider` | logic |
| `IPasswordModeService` | `PasswordModeService` | logic |
| `IDesktopLockEventService` | `DesktopLockEventService` | logic |
| `IScreenManager` | `ScreenManager` | logic |
| `IInterceptKeys` | `InterceptKeys` (static singleton) | logic |
| `IInterceptMouse` | `InterceptMouse` (static singleton) | logic |
| `IConcurrencyService` | `ConcurrencyService` | carnac (Utilities/) |
| `ISettingsProvider` | `JsonSettingsProvider` | logic (Settings/) |

## Code Conventions

- **Namespaces**: `Carnac.Logic` (logic layer), `Carnac.UI` (views/viewmodels), `Carnac.Utilities`, `Carnac.logic` (models — lowercase, legacy).
- **File naming**: PascalCase matching class names. Interfaces prefixed with `I`.
- **XAML**: Views in `src/w4b.carnac/UI/`. Themes in `src/w4b.carnac/Themes/`.
- **Keymaps**: YAML files in `src/w4b.carnac.logic/Keymaps/` — copied to output on build. Users can also specify a custom keymaps folder in preferences.
- **Tests**: xUnit `[Fact]` attributes. Use `NSubstitute` for mocks (`Substitute.For<IFoo>()`). Use `Shouldly` for assertions (`value.ShouldBe(expected)`). Use `Microsoft.Reactive.Testing` for Rx stream testing.

## Important Warnings

- **CA1416 warnings (~365)**: Expected. WPF APIs are Windows-only and the project targets `net10.0-windows`. These are informational.
- **Do NOT add `Fody` or `PropertyChanged.Fody`**: These were removed and replaced by CommunityToolkit.Mvvm source generators. There are no `FodyWeavers.xml` files in the repo.
- **Do NOT add `Microsoft.CSharp`**: It's included in the .NET 10 SDK; explicit references are unnecessary.
- **Do NOT add `Costura.Fody` or `Squirrel.Windows`**: Removed. Use `PublishSingleFile` for single-file publishing. Velopack is the replacement installer framework.
- **Do NOT add `MouseKeyHook`**: Removed. Mouse hooks now use native Win32 `WH_MOUSE_LL` P/Invoke.
- **Do NOT add `SettingsProviderNet`**: Removed. Settings are handled by `JsonSettingsProvider` using `System.Text.Json`.

## Completed Modernization

| Phase | Goal | Status |
|-------|------|--------|
| 4 | Replace `SettingsProviderNet` with `JsonSettingsProvider` (System.Text.Json) | ✅ Done |
| 5 | Performance: `System.Threading.Channels`, async settings, thread-safe utilities | ✅ Done |
| 6 | Replace `MouseKeyHook` with native Win32 `WH_MOUSE_LL` P/Invoke | ✅ Done |
| 7 | Plugin system — custom keymaps folder in preferences | ✅ Done |
| 8 | Velopack installer integration | ✅ Done |
| 9 | CI/CD pipeline (GitHub Actions) | ✅ Done |
