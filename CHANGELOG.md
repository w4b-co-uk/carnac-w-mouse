# Changelog

All notable changes to Carnac w/ Mouse are documented in this file.

## [3.0.0] — 2026-04-01

Major modernization release: .NET 10, new architecture, multi-language UI, and native mouse hooks.

### Added

- **Updated About tab** — Refreshed project description, contributors (including all fork authors), current component list, and GitHub link. Title now shows "Carnac w/ Mouse".
- **Responsive buttons** — All preference window buttons now auto-size to fit localized text instead of using fixed widths.
- **Minimum window size** — Preferences window enforces a minimum size of 610×875 to prevent layout issues.

- **Multi-language UI** — Interface available in English (default), Spanish, and Portuguese (Brazil). Language selector in General tab; changes apply live.
- **Custom keymaps folder** — Load additional YAML keymap files from a user-specified folder (Keyboard tab → Custom Keymaps).
- **Native mouse hook** — Replaced `MouseKeyHook` library with native Win32 `WH_MOUSE_LL` P/Invoke via `InterceptMouse`.
- **Channel-based decoupling** — `System.Threading.Channels` in `InterceptKeys` and `InterceptMouse` to prevent hook timeout under load.
- **JSON settings** — Replaced `SettingsProviderNet` with `JsonSettingsProvider` using `System.Text.Json`.
- **Velopack installer** — Integrated `Velopack 0.0.1298` for update hooks.
- **CI/CD pipeline** — GitHub Actions workflow (`.github/workflows/ci.yml`) for build, test, and publish on push to `beta`/`main`.
- **Structured logging** — Serilog with rolling daily files in `%LOCALAPPDATA%/Carnac/logs/`.
- **Dependency injection** — `Microsoft.Extensions.Hosting` with `IServiceProvider` composition root in `App.xaml.cs`.
- **MVVM source generators** — `CommunityToolkit.Mvvm 8.4.1` with `[ObservableProperty]` replacing Fody IL weaving.
- **Max messages setting** — Configurable limit for visible keystroke messages (FIFO queue).
- **Bilingual documentation** — Full README, User Manual, and Architecture Guide in English and Spanish.

### Changed

- **Target framework** — Upgraded from .NET 8 to **.NET 10 LTS** (`net10.0-windows`).
- **Key sanitization** — Rewritten `ReplaceKey.Sanitise()` to use shift-state logic instead of P/Invoke `GetKeyboardState`/`ToUnicodeEx`, fixing test reliability and uppercase handling.
- **Modifier filtering** — `KeyProvider` pipeline now filters modifier-only key events (e.g. bare Shift, Ctrl presses) to avoid producing extra `KeyPress` items.

### Removed

- `Fody` / `PropertyChanged.Fody` — replaced by CommunityToolkit.Mvvm source generators.
- `MouseKeyHook` — replaced by native `WH_MOUSE_LL` P/Invoke.
- `SettingsProviderNet` — replaced by `JsonSettingsProvider`.
- `Costura.Fody` — replaced by `PublishSingleFile`.
- `Squirrel.Windows` — replaced by Velopack.
- `Microsoft.CSharp` — included in .NET 10 SDK.

### Fixed

- All 31 unit tests now pass (previously 10 failures due to modifier leaks and P/Invoke key state issues in tests).

---

## [2.x] — Previous Versions

See the [w4b-co-uk/carnac-w-mouse](https://github.com/w4b-co-uk/carnac-w-mouse) repository for .NET 8 era changes and the original [Code52/carnac](https://github.com/Code52/carnac) for the project's early history.
