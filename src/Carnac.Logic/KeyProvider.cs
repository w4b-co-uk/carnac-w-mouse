using Carnac.Logic.KeyMonitor;
using Carnac.Logic.Models;
using Carnac.Logic.MouseMonitor;
using Microsoft.Win32;
using SettingsProviderNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media;

namespace Carnac.Logic {
    public class KeyProvider: IKeyProvider {
        private readonly IInterceptKeys interceptKeysSource;
        private readonly IPasswordModeService passwordModeService;
        private readonly IDesktopLockEventService desktopLockEventService;
        private readonly PopupSettings settings;
        private string currentFilter = null;

        private static readonly IList<Keys> modifierKeys =
            new List<Keys>
                {
                    Keys.LControlKey,
                    Keys.RControlKey,
                    Keys.LShiftKey,
                    Keys.RShiftKey,
                    Keys.LMenu,
                    Keys.RMenu,
                    Keys.ShiftKey,
                    Keys.Shift,
                    Keys.Alt,
                    Keys.LWin,
                    Keys.RWin
                };

        private bool winKeyPressed;

        public KeyProvider(IInterceptKeys interceptKeysSource, IPasswordModeService passwordModeService, IDesktopLockEventService desktopLockEventService, ISettingsProvider settingsProvider) {
            if (settingsProvider == null) {
                throw new ArgumentNullException(nameof(settingsProvider));
            }

            this.interceptKeysSource = interceptKeysSource;
            this.passwordModeService = passwordModeService;
            this.desktopLockEventService = desktopLockEventService;

            settings = settingsProvider.GetSettings<PopupSettings>();
        }

        private bool ShouldFilterProcess(out Regex filterRegex) {
            filterRegex = null;
            if (settings?.ProcessFilterExpression != currentFilter) {
                currentFilter = settings?.ProcessFilterExpression;

                if (!string.IsNullOrEmpty(currentFilter)) {
                    try {
                        filterRegex = new Regex(currentFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                    } catch {
                        filterRegex = null;
                    }
                } else {
                    filterRegex = null;
                }
            }

            return filterRegex != null;
        }

        public IObservable<KeyPress> GetKeyStream() {
            // We are using an observable create to tie the lifetimes of the session switch stream and the keystream
            return Observable.Create<KeyPress>(observer => {
                // When desktop is locked we will not get the keyup, because we track the windows key
                // specially we need to set it to not being pressed anymore
                IDisposable sessionSwitchStreamSubscription = desktopLockEventService.GetSessionSwitchStream()
                .Subscribe(ss => {
                    if (ss.Reason == SessionSwitchReason.SessionLock) {
                        winKeyPressed = false;
                    }
                }, observer.OnError);

                IDisposable keyStreamSubsription = Observable.Merge(
                    new IObservable<InterceptKeyEventArgs>[2] {
                        interceptKeysSource.GetKeyStream(),
                        InterceptMouse.Current.GetKeyStream() })
                    .Select(DetectWindowsKey)
                    .Where(k => k.KeyDirection == KeyDirection.Down)
                    .Select(ToCarnacKeyPress)
                    .Where(keypress => keypress != null)
                    .Where(k => !passwordModeService.CheckPasswordMode(k.InterceptKeyEventArgs))
                    .Subscribe(observer);

                return new CompositeDisposable(sessionSwitchStreamSubscription, keyStreamSubsription);
            });
        }

        private InterceptKeyEventArgs DetectWindowsKey(InterceptKeyEventArgs interceptKeyEventArgs) {
            if (interceptKeyEventArgs.Key == Keys.LWin || interceptKeyEventArgs.Key == Keys.RWin) {
                if (interceptKeyEventArgs.KeyDirection == KeyDirection.Up) {
                    winKeyPressed = false;
                } else if (interceptKeyEventArgs.KeyDirection == KeyDirection.Down) {
                    winKeyPressed = true;
                }
            }

            return interceptKeyEventArgs;
        }

        public static bool IsModifierKeyPress(InterceptKeyEventArgs interceptKeyEventArgs) {
            return modifierKeys.Contains(interceptKeyEventArgs.Key);
        }

        private KeyPress ToCarnacKeyPress(InterceptKeyEventArgs interceptKeyEventArgs) {
            Process process = AssociatedProcessUtilities.GetAssociatedProcess();
            if (process == null) {
                return null;
            }

            // see if this process is one being filtered for
            if (ShouldFilterProcess(out Regex filterRegex) && !filterRegex.IsMatch(process.ProcessName)) {
                return null;
            }

            if (!settings.ShowMouseClickKeys && (interceptKeyEventArgs.Key == Keys.LButton || interceptKeyEventArgs.Key == Keys.MButton || interceptKeyEventArgs.Key == Keys.RButton || interceptKeyEventArgs.Key == Keys.XButton1 || interceptKeyEventArgs.Key == Keys.XButton2)) {
                return null;
            }

            if (!settings.ShowMouseScrollKeys && (interceptKeyEventArgs.Key == Keys.VolumeUp || interceptKeyEventArgs.Key == Keys.VolumeDown)) {
                return null;
            }

            bool isLetter = interceptKeyEventArgs.IsLetter();
            string[] inputs = ToInputs(isLetter, winKeyPressed, interceptKeyEventArgs).ToArray();
            try {
                string processFileName = process.MainModule.FileName;
                ImageSource image = IconUtilities.GetProcessIconAsImageSource(processFileName);
                return new KeyPress(new ProcessInfo(process.ProcessName, image), interceptKeyEventArgs, winKeyPressed, inputs);
            } catch (Exception) {
                return new KeyPress(new ProcessInfo(process.ProcessName), interceptKeyEventArgs, winKeyPressed, inputs);
                ;
            }
        }

        private static IEnumerable<string> ToInputs(bool isLetter, bool isWinKeyPressed, InterceptKeyEventArgs interceptKeyEventArgs) {
            bool controlPressed = interceptKeyEventArgs.ControlPressed;
            bool altPressed = interceptKeyEventArgs.AltPressed;
            bool shiftPressed = interceptKeyEventArgs.ShiftPressed;

            if (IsModifierKeyPress(interceptKeyEventArgs)) {
                controlPressed = false;
                altPressed = false;
                shiftPressed = false;
                isWinKeyPressed = false;
            }

            bool mouseAction = InterceptMouse.MouseKeys.Contains(interceptKeyEventArgs.Key);
            if (controlPressed) {
                yield return "Ctrl";
            }

            if (altPressed) {
                yield return "Alt";
            }

            if (isWinKeyPressed) {
                yield return "Win";
            }

            if (controlPressed || altPressed || mouseAction) {
                //Treat as a shortcut, don't be too smart
                if (shiftPressed) {
                    yield return "Shift";
                }

                yield return interceptKeyEventArgs.Key.SanitiseLower();
            } else {
                yield return interceptKeyEventArgs.Key.Sanitise();
            }
        }
    }
}