# Carnac w/ Mouse — User Manual

> 🌐 [Leer en español](MANUAL_USUARIO.md)

## Table of Contents

1. [Introduction](#introduction)
2. [Installation](#installation)
3. [First Launch](#first-launch)
4. [The System Tray Icon](#the-system-tray-icon)
5. [Preferences Window](#preferences-window)
   - [General Tab](#general-tab)
   - [Keyboard Tab](#keyboard-tab)
   - [Mouse Tab](#mouse-tab)
   - [About Tab](#about-tab)
6. [Silent Mode (Password Protection)](#silent-mode-password-protection)
7. [International Keyboard Support](#international-keyboard-support)
8. [Shortcut Recognition](#shortcut-recognition)
9. [Log Files](#log-files)
10. [Troubleshooting](#troubleshooting)

---

## Introduction

Carnac w/ Mouse is a **real-time keystroke and mouse click visualizer** for Windows. It shows a transparent always-on-top overlay displaying every key you press and every mouse button you click, making it perfect for:

- **Live presentations** — Your audience can see keyboard shortcuts as you use them.
- **Screencasts & tutorials** — Viewers can follow along with your exact key presses.
- **Learning keyboard shortcuts** — Carnac detects known shortcuts and shows their names.
- **Demos** — Show exactly what you're doing without verbal explanation.

---

## Installation

### Prerequisites

- **Operating System**: Windows 10 or later.
- **Runtime**: [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).

### Steps

1. Download the latest release of `w4b.carnac.exe` (single-file executable).
2. Place it anywhere on your computer (e.g., `C:\Tools\Carnac\`).
3. Double-click `w4b.carnac.exe` to launch.

> **Tip**: Right-click the executable → **Create shortcut** and place it in your Startup folder (`shell:startup`) to launch Carnac automatically with Windows.

---

## First Launch

When you first run Carnac:

1. The application starts **minimized** — there is no main window.
2. A small icon appears in the **system tray** (bottom-right of your taskbar).
3. A transparent overlay is active on one of your screens.
4. Start typing or clicking anywhere — your keystrokes and mouse clicks appear on the overlay immediately.

---

## The System Tray Icon

The system tray icon is your primary way to interact with Carnac:

| Action | Result |
|--------|--------|
| **Left-click** | Opens the Preferences window |
| **Right-click** | Shows context menu with **Exit** option |

---

## Preferences Window

Left-click the tray icon to open the Preferences window. It has four tabs:

### General Tab

Controls **where** the overlay appears.

| Setting | Description |
|---------|-------------|
| **Screen selector** | Visual representation of all connected monitors. Click one to choose which screen shows the overlay. |
| **Notification placement** | Choose a corner: Top-Left, Top-Right, Bottom-Left, or Bottom-Right. |
| **Top Offset** | Distance in pixels from the top edge of the screen. |
| **Bottom Offset** | Distance in pixels from the bottom edge of the screen. |
| **Left Offset** | Distance in pixels from the left edge of the screen. |
| **Right Offset** | Distance in pixels from the right edge of the screen. |
| **Language** | Select the UI language: English (default), Español, or Português (Brasil). Changes apply immediately to the Preferences window. |

### Keyboard Tab

Controls how **keystrokes** appear in the overlay.

| Setting | Description | Default |
|---------|-------------|---------|
| **Popup Text Width** | Maximum pixel width of the keystroke display area. | 350 |
| **Popup Opacity** | Transparency of the overlay text. 0 = invisible, 1 = fully opaque. | 0.5 |
| **Popup Fade Delay** | Seconds before the keystroke fades away. | 5 |
| **Font Size** | Text size in pixels (range: 8–48). | 40 |
| **Font Colour** | Text color (any Windows color name, e.g., "White", "Cyan", "Yellow"). | White |
| **Background Color** | Background color behind the text. | Black |
| **Shortcuts Only** | When enabled, only displays key combinations found in the built-in keymap files. Regular typing is hidden. | Off |
| **Custom Keymaps** | Path to an optional folder containing additional `.yml` keymap files. Use the Browse button to select a folder. Applied on restart. | Empty |
| **Only Keys with Modifiers** | When enabled, only shows key combos that include Ctrl, Alt, Shift, or Win. Regular typing is hidden. | Off |
| **Show Space as ␣** | Displays the space key as the Unicode open-box symbol instead of a blank space. | Off |
| **Show Application Icon** | Displays the icon of the currently active application next to the keystroke. | Off |
| **Process Filter** | A regex pattern to limit keystroke display to specific applications. Leave empty to show all. Example: `chrome|firefox` to only show keystrokes in browsers. | Empty |

### Mouse Tab

Controls how **mouse clicks** appear on the overlay.

| Setting | Description | Default |
|---------|-------------|---------|
| **Show Mouse Clicks** | Toggle mouse click visualization on/off. | On |
| **Show Clicks as Keys** | Render mouse click names (e.g., "LButton") in the keystroke overlay area. | On |
| **Show Scroll as Keys** | Display scroll wheel events in the overlay. | On |
| **Mouse Key Size** | Size of the click indicator circle in pixels (8–300). | 40 |
| **Start Scale** | Initial size multiplier of the click animation. | 1 |
| **Stop Scale** | Final size multiplier of the click animation (creates expanding effect). | 4 |
| **Circle Fade Delay** | Milliseconds before the click indicator disappears (100–5000). | 3700 |
| **Start Border** | Initial border width of the indicator circle. | 1 |
| **Start Opacity** | Initial transparency of the indicator (0–1). | 0.8 |
| **Stop Border** | Final border width of the indicator. | 2 |
| **Stop Opacity** | Final transparency of the indicator (0 = fully faded out). | 0 |
| **Left Click Color** | Color of the left mouse button indicator. | OrangeRed |
| **Right Click Color** | Color of the right mouse button indicator. | RoyalBlue |
| **Scroll Click Color** | Color of the scroll wheel indicator. | Gold |
| **XButton1 Click Color** | Color of mouse button 4 (side button) indicator. | Peru |
| **XButton2 Click Color** | Color of mouse button 5 (side button) indicator. | Plum |

### About Tab

Shows credits, component versions, and links to the project repository.

---

## Silent Mode (Password Protection)

When you need to type a password or other sensitive information:

1. Press **`Ctrl+Alt+P`** — the overlay stops showing keystrokes and mouse clicks.
2. Type your password or sensitive data normally.
3. Press **`Ctrl+Alt+P`** again — the overlay resumes normal operation.

> **Important**: Silent mode is a toggle. You must press the shortcut a second time to re-enable keystroke display.

---

## International Keyboard Support

Carnac automatically detects your active Windows keyboard layout and displays the correct characters for your locale. This means:

- **Spanish keyboards**: `ñ`, accented vowels (`á`, `é`, `í`, `ó`, `ú`), and `ü` display correctly.
- **French AZERTY**: Characters map to their correct physical positions.
- **German QWERTZ**: Umlauts (`ä`, `ö`, `ü`) and `ß` display correctly.
- **Any other Windows keyboard layout**: Characters are resolved through the Windows `ToUnicodeEx` API, so any layout supported by Windows will work.

### Dead Keys (Accent Marks)

Dead keys are keys that don't produce a character immediately but modify the next key press (e.g., pressing `´` then `a` produces `á` on Spanish keyboards).

Carnac handles dead keys by:
1. Displaying the accent mark when the dead key is pressed.
2. Displaying the next character as normal.
3. Preserving the dead key state so the target application still receives the correct combined character.

---

## Shortcut Recognition

Carnac includes built-in keymap files for popular applications:

| Keymap | Application |
|--------|-------------|
| `chrome.yml` | Google Chrome |
| `visual-studio.yml` | Visual Studio IDE |
| `vscode.yml` | Visual Studio Code |
| `resharper.yml` | ReSharper |
| `ncrunch.yml` | NCrunch |

When you press a key combination that matches a shortcut in these files and the corresponding application is in focus, Carnac displays the **shortcut name** alongside the keys.

These keymap files are located in `Keymaps/` next to the executable and use YAML format. You can edit them or add new ones.

---

## Log Files

Carnac writes diagnostic logs to:

```
%LOCALAPPDATA%\Carnac\logs\carnac-YYYYMMDD.log
```

Logs rotate daily and are retained for 7 days. Use these files for troubleshooting problems.

---

## Troubleshooting

### The overlay does not appear

- Make sure Carnac is running (check the system tray for the icon).
- Open Preferences → General tab and verify the correct monitor is selected.
- Try changing the notification placement to a different corner.

### Keys are not displaying

- Ensure silent mode is not active (press `Ctrl+Alt+P` to toggle).
- Check if "Shortcuts Only" or "Only Keys with Modifiers" is enabled in the Keyboard tab.
- Check the Process Filter — if it has a value, Carnac only shows keys from matching applications.

### Accented characters don't display correctly

- Verify your Windows keyboard layout is set to your language (e.g., "Spanish (Spain)" or "Spanish (Latin America)").
- Switch to your desired layout using `Win+Space` in Windows.
- Carnac uses your active keyboard layout; changing it takes effect immediately.

### The overlay blocks mouse clicks

- The overlay is designed to be click-through — clicks pass through to applications underneath. If this isn't working, try restarting Carnac.

### Mouse click indicators don't appear

- Open Preferences → Mouse tab and ensure "Show Mouse Clicks" is enabled.
- Check if the circle fade delay is very short (increase it to see the effect).
- Verify that start opacity is above 0.

## Debug

```powershell
$env:PATH = "C:\Program Files\dotnet;" + $env:PATH
dotnet run --project src/w4b.carnac/w4b.carnac.csproj
```

## Release

To build a release version of Carnac:

```powershell
# 1. Ensure .NET 10 SDK is first in PATH
$env:PATH = "C:\Program Files\dotnet;" + $env:PATH

# 2. Restore NuGet packages (uses local NuGet.Config to bypass VS offline config)
dotnet restore src/w4b-carnac.sln -p:RestoreConfigFile="NuGet.Config"

# 3. Build in Release mode
dotnet build src/w4b-carnac.sln -c Release --no-restore

# 4. Run tests to verify everything passes
dotnet test src/w4b.carnac.tests/w4b.carnac.tests.csproj -c Release --no-build --nologo

# 5. Publish single-file executable
dotnet publish src/w4b.carnac/w4b.carnac.csproj -c Release --no-restore
```

The output is a single-file executable at:

```
src/w4b.carnac/bin/Release/net10.0-windows/publish/w4b.carnac.exe
```

This file is framework-dependent (requires .NET 10 Desktop Runtime on the target machine). To create a fully self-contained executable:

```powershell
dotnet publish src/w4b.carnac/w4b.carnac.csproj -c Release --self-contained true -r win-x64
```

The self-contained output is at `src/w4b.carnac/bin/Release/net10.0-windows/win-x64/publish/w4b.carnac.exe`.