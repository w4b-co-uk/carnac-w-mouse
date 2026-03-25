# Carnac w/ Mouse

**A real-time keystroke and mouse click visualizer for Windows.**

> 🌐 [Leer en español](README_ES.md)

Carnac w/ Mouse displays an always-on-top transparent overlay showing every key press and mouse click as it happens. It is ideal for **live presentations**, **screencasts**, **tutorials**, and **demos** where your audience needs to see what you are typing or clicking.

![Windows](https://img.shields.io/badge/platform-Windows-blue)
![.NET 10](https://img.shields.io/badge/.NET-10.0-purple)
![WPF](https://img.shields.io/badge/UI-WPF-green)
![License](https://img.shields.io/badge/license-MS--PL-orange)

## Features

- **Keystroke visualization** — Every key press appears in a floating overlay with configurable size, color, opacity, and fade delay.
- **Mouse click indicators** — Animated expanding circles for left, right, middle, and extra buttons, each with its own color.
- **Scroll wheel display** — Optionally shows scroll up/down events.
- **Multi-monitor support** — Choose which screen displays the overlay and position it in any corner with pixel-level offsets.
- **Shortcut detection** — Recognizes keyboard shortcuts from built-in keymaps (VS Code, Visual Studio, Chrome, ReSharper, NCrunch) and displays their names.
- **International keyboard support** — Automatically detects your Windows keyboard layout and displays the correct characters, including accented characters (á, é, ñ, ü, etc.) and dead key sequences.
- **Password / silent mode** — Press `Ctrl+Alt+P` to temporarily hide all keystrokes (for entering passwords or sensitive data). Press again to resume.
- **Process filtering** — Optionally limit visualization to specific applications using regex patterns.
- **Application icon display** — Show the icon of the active application next to keystrokes.
- **Fully customizable** — 26+ settings for colors, sizes, positions, animations, and behavior.

## Requirements

- Windows 10 or later
- [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) (or .NET 10 SDK for building from source)

## Quick Start

### Option A: Run from published binary

1. Download the latest release (or build it yourself — see below).
2. Run `w4b.carnac.exe`.
3. An icon appears in the system tray — left-click it to open preferences.
4. Start typing or clicking anywhere; the overlay shows your keystrokes in real time.

### Option B: Build from source

```powershell
# Ensure .NET 10 SDK is first in PATH
$env:PATH = "C:\Program Files\dotnet;" + $env:PATH

# Restore NuGet packages
dotnet restore src/w4b-carnac.sln -p:RestoreConfigFile="NuGet.Config"

# Build in Debug mode
dotnet build src/w4b-carnac.sln --no-restore

# Run the application
dotnet run --project src/w4b.carnac/w4b.carnac.csproj

# Or publish a single-file Release executable
dotnet publish src/w4b.carnac/w4b.carnac.csproj -c Release
```

The published binary is at `src/w4b.carnac/bin/Release/net10.0-windows/publish/w4b.carnac.exe`.

## Usage

| Action | How |
|--------|-----|
| Open preferences | Left-click the tray icon |
| Exit | Right-click the tray icon → **Exit** |
| Toggle silent mode | `Ctrl+Alt+P` |

For a full walkthrough, see the **[User Manual (English)](docs/USER_MANUAL.md)** or the **[Manual de Usuario (Español)](docs/MANUAL_USUARIO.md)**.

## Configuration

All settings are accessible from the **Preferences** window (left-click the tray icon):

| Tab | Settings |
|-----|----------|
| **General** | Screen selection, overlay position (corner + offsets) |
| **Keyboard** | Font size, color, background, opacity, fade delay, shortcuts-only mode, modifier-only mode, process filter |
| **Mouse** | Click colors (per button), indicator size, animation scale, border, opacity, fade delay, show/hide clicks and scroll |

## Project Structure

| Project | Description |
|---------|-------------|
| `w4b.carnac` | WPF application — UI, dependency injection, tray icon |
| `w4b.carnac.logic` | Domain layer — keyboard/mouse hooks, message pipeline, models |
| `w4b.carnac.tests` | Unit tests (xUnit + NSubstitute + Shouldly) |

For architecture details, see [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## History & Acknowledgments

This project is a fork chain preserving the work of the original authors:

1. **[Code52/carnac](https://github.com/Code52/carnac)** — Original Carnac keystroke utility.
2. **[bfritscher/carnac](https://github.com/bfritscher/carnac)** — Boris Fritscher's fork adding mouse click highlights.
3. **[w4b-co-uk/carnac-w-mouse](https://github.com/w4b-co-uk/carnac-w-mouse)** — Updated to .NET 8 with namespace cleanup and multi-monitor work. *(credits to the w4b team)*
4. **[OscarTinajero117/carnac-w-mouse](https://github.com/OscarTinajero117/carnac-w-mouse)** — Modernized to .NET 10, CommunityToolkit.Mvvm source generators, Serilog structured logging, improved international keyboard support (dead key fix), and comprehensive bilingual documentation.

## License

[Microsoft Public License (MS-PL)](LICENSE.md)

