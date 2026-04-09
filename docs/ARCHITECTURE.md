# Carnac-w-Mouse — Architecture Guide

> A keystroke & mouse click visualizer for Windows.  
> Target: **.NET 10 LTS** · WPF · System.Reactive

---

## High-Level Data Flow

```
┌──────────────────────────────────────────────────────┐
│                   Win32 Layer                        │
│  SetWindowsHookEx (WH_KEYBOARD_LL) · WH_MOUSE_LL     │
└──────────────┬──────────────────────┬────────────────┘
               │     Channel<T>       │
               │   (bounded queue)    │
               ▼                      ▼
┌──────────────────────┐  ┌───────────────────────┐
│   InterceptKeys      │  │   InterceptMouse      │
│   (IObservable<>)    │  │   (IObservable<>)     │
└──────────────┬───────┘  └──────────┬────────────┘
               └──────────┬──────────┘
                          ▼
              ┌─────────────────────────┐
              │     KeyProvider         │
              │  Merge · Filter · Map   │
              │  → IObservable<KeyPress>|
              └───────────┬─────────────┘
                          ▼
              ┌────────────────────────┐
              │   MessageProvider      │
              │  Scan(ShortcutAccum.)  │
              │  → IObservable<Message>|
              └───────────┬────────────┘
                          ▼
              ┌───────────────────────┐
              │    KeysController     │
              │  Add · Fade · Remove  │
              │  ObservableCollection │
              └───────────┬───────────┘
                          ▼
              ┌───────────────────────┐
              │   KeyShowView (WPF)   │
              │  Transparent Overlay  │
              │  + Mouse Animations   │
              └───────────────────────┘
```

---

## Project Structure

```
src/
├── w4b.carnac/               # UI Layer (WPF Application)
│   ├── App.xaml.cs           # Entry point, bootstrapping
│   ├── CarnacTrayIcon.cs     # System tray icon (WinForms NotifyIcon)
│   ├── UI/
│   │   ├── KeyShowView       # Transparent overlay window
│   │   ├── KeyShowViewModel  # Manages keystroke collection
│   │   ├── PreferencesView   # Settings window (MahApps.Metro)
│   │   └── PreferencesViewModel
│   ├── Utilities/
│   │   ├── ConcurrencyService  # Rx schedulers
│   │   ├── Loc                 # i18n singleton (ResourceManager + INotifyPropertyChanged)
│   │   ├── ProcessUtilities    # Single-instance mutex
│   │   └── PlacementMarginConverter
│   ├── Properties/           # .resx resource files (Strings.resx, Strings.es.resx, Strings.pt-BR.resx)
│   ├── Resources/            # Icons, fonts (Entypo)
│   └── Themes/               # WPF control templates
│
├── w4b.carnac.logic/         # Domain + Infrastructure
│   ├── KeyMonitor/
│   │   ├── InterceptKeys     # Win32 keyboard hook → Channel → Observable
│   │   ├── InterceptKeyEventArgs  # Raw event data
│   │   └── DesktopLockEventService
│   ├── MouseMonitor/
│   │   ├── InterceptMouse    # Native WH_MOUSE_LL hook → Channel → Observable
│   │   └── IInterceptMouse   # DI interface for mouse hook
│   ├── Models/
│   │   ├── KeyPress          # Enriched key event (process, icon, inputs)
│   │   ├── Message           # Display unit (merging, formatting)
│   │   └── PopupSettings     # All user preferences (~40 properties)
│   ├── Settings/
│   │   ├── ISettingsProvider  # Settings interface (sync + async)
│   │   └── JsonSettingsProvider # JSON-backed persistence (System.Text.Json)
│   ├── Keymaps/              # YAML shortcut definitions (built-in)
│   │   ├── visual-studio.yml
│   │   ├── vscode.yml
│   │   ├── chrome.yml
│   │   └── ...
│   ├── Native/               # Win32 structs (DEVMODE, DISPLAY_DEVICE)
│   ├── Enums/                # NotificationPlacement
│   ├── Internal/             # FixedQueue<T>
│   ├── KeyProvider.cs        # Core: merge hooks → filter → enrich
│   ├── MessageProvider.cs    # Shortcut detection + message merging
│   ├── ShortcutProvider.cs   # YAML keymap loader
│   ├── ShortcutAccumulator.cs # State machine for shortcut detection
│   ├── KeysController.cs     # Manages message lifecycle (add/fade/remove)
│   ├── PasswordModeService.cs # Ctrl+Alt+P toggle to hide keystrokes
│   ├── ScreenManager.cs      # Multi-monitor detection
│   ├── ReplaceKey.cs         # Locale-aware key-to-string translation
│   └── Win32Methods.cs       # P/Invoke declarations
│
├── w4b.carnac.tests/         # Unit Tests (xUnit + NSubstitute)
│   ├── KeyProviderTests.cs   # Modifier handling, filtering
│   ├── KeysControllerFacts.cs # Message lifecycle timing
│   ├── MessageFacts.cs       # Grouping, repetition formatting
│   ├── MessageProviderFacts.cs # Shortcut recognition
│   ├── KeyStreams.cs          # Test data factory
│   └── ViewModels/           # ViewModel initialization tests
│
└── w4b.key.stream.capture/   # Developer Utility
    └── MainWindow.xaml.cs    # Captures keys → generates C# test code
```

---

## Key Concepts for Beginners

### Reactive Extensions (Rx) — `System.Reactive`
Think of Rx as **streams of events** (like JavaScript Observables or Dart Streams).

- `IObservable<T>` = a stream that pushes events to subscribers
- `.Where()` = filter events (like `.where()` in Dart)
- `.Select()` = transform events (like `.map()` in JS)
- `.Scan()` = accumulate state over time (like `.fold()` / `reduce()`)
- `.Merge()` = combine multiple streams into one
- `.ObserveOn()` = switch to a specific thread (critical for UI updates)

**Example flow:**
```csharp
keyboardStream.Merge(mouseStream)   // Combine both inputs
    .Where(k => k.KeyDirection == Down)  // Only key-down events
    .Select(ToCarnacKeyPress)            // Enrich with process info
    .Scan(accumulator, ProcessKey)       // Detect shortcuts (stateful)
    .Where(c => c.HasCompletedValue)     // Only when ready
    .SelectMany(c => c.GetMessages())    // Expand to Messages
```

### WPF Data Binding
Like Flutter's `setState()` + `Obx()`, but declarative in XAML:
- `{Binding PropertyName}` in XAML ↔ `Obx(() => widget)` in Flutter
- `INotifyPropertyChanged` ↔ `ValueNotifier` / `ChangeNotifier` in Flutter
- `ObservableCollection<T>` = a list that notifies the UI when items are added/removed

### CommunityToolkit.Mvvm (Source Generators)
Replaces Fody IL weaving with compile-time source generators:
- `[ObservableProperty]` on a `private` field → generates a `Public` property with `OnPropertyChanged()`
- `[NotifyPropertyChangedFor(nameof(DerivedProp))]` → cascading notifications
- `ObservableObject` base class → implements `INotifyPropertyChanged`
- Class must be `partial` to allow source generation

### Dependency Injection (Microsoft.Extensions.DependencyInjection)
All services are registered in `App.xaml.cs` using `Host.CreateDefaultBuilder()`:
- `AddSingleton<IService, Implementation>()` → one instance for the entire app lifetime
- `AddTransient<Service>()` → new instance each time it's requested
- `GetRequiredService<T>()` → resolve a service from the container

### Logging (Serilog)
Structured logging to rolling files in `%LOCALAPPDATA%/Carnac/logs/`:
- `Log.Information("message")` for info-level events
- `ILogger<T>` can be injected via DI for class-specific logging
- Rolling daily files, 7-day retention

### Localization (i18n)
Multi-language UI via `.resx` resource files + a `Loc` singleton:
- `Properties/Strings.resx` — English (default)
- `Properties/Strings.es.resx` — Spanish
- `Properties/Strings.pt-BR.resx` — Portuguese (Brazil)
- `Utilities/Loc.cs` — Wraps `ResourceManager`, exposes string properties, implements `INotifyPropertyChanged` so XAML bindings update live when `SwitchCulture()` is called.
- Language preference stored in `PopupSettings.Language` and applied on startup via `CultureInfo.CurrentUICulture`.

---

## Dependencies

| Package | Purpose | Status |
|---------|---------|--------|
| System.Reactive 6.0 | Event stream processing | ✅ Keep |
| System.Threading.Channels | Hook callback decoupling | ✅ Added |
| MahApps.Metro 2.4.10 | Modern WPF controls | ✅ Keep |
| YamlDotNet 13.1.1 | YAML keymap parser | ✅ Update to 16.x |
| CommunityToolkit.Mvvm 8.4.1 | MVVM source generators | ✅ Added |
| Microsoft.Extensions.Hosting 10.0.5 | DI + Host | ✅ Added |
| Serilog.Extensions.Hosting 10.0.0 | Structured logging | ✅ Added |
| Velopack 0.0.1298 | Installer framework (update hooks) | ✅ Added |
| xUnit + NSubstitute + Shouldly | Unit testing | ✅ Keep, updated |

### Removed
- ~~Costura.Fody~~ → `PublishSingleFile` native .NET
- ~~Squirrel.Windows~~ → Velopack
- ~~DeltaCompressionDotNet~~ → Squirrel dependency
- ~~Mono.Cecil~~ → Squirrel dependency
- ~~Splat~~ → Squirrel dependency
- ~~Fody~~ → CommunityToolkit.Mvvm source generators
- ~~PropertyChanged.Fody~~ → `[ObservableProperty]` attribute
- ~~Microsoft.CSharp~~ → included in .NET 10 SDK
- ~~SettingsProviderNet~~ → `JsonSettingsProvider` (System.Text.Json)
- ~~MouseKeyHook~~ → Native Win32 `WH_MOUSE_LL` P/Invoke in `InterceptMouse`

---

## Build & Run

```bash
# Restore with local NuGet config (bypasses VS offline config permissions)
dotnet restore src/w4b-carnac.sln -p:RestoreConfigFile=NuGet.Config

# Build
dotnet build src/w4b-carnac.sln --no-restore

# Test
dotnet test src/w4b.carnac.tests/w4b.carnac.tests.csproj --no-build

# Publish single-file exe
dotnet publish src/w4b.carnac/w4b.carnac.csproj -c Release
```
