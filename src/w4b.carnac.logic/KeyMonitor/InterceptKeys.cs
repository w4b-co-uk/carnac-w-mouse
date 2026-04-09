using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carnac.Logic.KeyMonitor {
    public class InterceptKeys: IInterceptKeys {
        public static readonly InterceptKeys Current = new InterceptKeys();
        private readonly IObservable<InterceptKeyEventArgs> keyStream;

        private readonly Channel<InterceptKeyEventArgs> channel =
            Channel.CreateBounded<InterceptKeyEventArgs>(
                new BoundedChannelOptions(128) {
                    FullMode = BoundedChannelFullMode.DropOldest,
                    SingleWriter = true,
                    SingleReader = true
                });

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private Win32Methods.LowLevelKeyboardProc callback;

        private InterceptKeys() {
            keyStream = Observable.Create<InterceptKeyEventArgs>(observer => {
                Debug.Write("Subscribed to keys");

                CancellationTokenSource cts = new();
                Task readerTask = Task.Run(async () => {
                    await foreach (InterceptKeyEventArgs item in channel.Reader.ReadAllAsync(cts.Token)) {
                        observer.OnNext(item);
                    }
                }, cts.Token);

                IntPtr hookId = IntPtr.Zero;
                callback = (nCode, wParam, lParam) => {
                    if (nCode >= 0) {
                        InterceptKeyEventArgs eventArgs = CreateEventArgs(wParam, lParam);
                        channel.Writer.TryWrite(eventArgs);
                        if (eventArgs.Handled) {
                            return (IntPtr)1;
                        }
                    }
                    return Win32Methods.CallNextHookEx(hookId, nCode, wParam, lParam);
                };
                hookId = SetHook(callback);

                return Disposable.Create(() => {
                    Debug.Write("Unsubscribed from keys");
                    _ = Win32Methods.UnhookWindowsHookEx(hookId);
                    callback = null;
                    cts.Cancel();
                    cts.Dispose();
                });
            })
            .Publish().RefCount();
        }

        public IObservable<InterceptKeyEventArgs> GetKeyStream() {
            return keyStream;
        }

        private static InterceptKeyEventArgs CreateEventArgs(IntPtr wParam, IntPtr lParam) {
            bool alt = (Control.ModifierKeys & Keys.Alt) != 0;
            bool control = (Control.ModifierKeys & Keys.Control) != 0;
            bool shift = (Control.ModifierKeys & Keys.Shift) != 0;
            bool keyDown = wParam == (IntPtr)Win32Methods.WM_KEYDOWN;
            bool keyUp = wParam == (IntPtr)Win32Methods.WM_KEYUP;
            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            //http://msdn.microsoft.com/en-us/library/windows/desktop/ms646286(v=vs.85).aspx
            if (key != Keys.RMenu && key != Keys.LMenu && wParam == (IntPtr)Win32Methods.WM_SYSKEYDOWN) {
                alt = true;
                keyDown = true;
            }
            if (key != Keys.RMenu && key != Keys.LMenu && wParam == (IntPtr)Win32Methods.WM_SYSKEYUP) {
                alt = true;
                keyUp = true;
            }
            if (wParam == (IntPtr)Win32Methods.WM_SYSKEYDOWN && key == Keys.LMenu) {
                keyDown = true;
            }

            return new InterceptKeyEventArgs(
                key,
                keyDown ?
                KeyDirection.Down : keyUp
                ? KeyDirection.Up : KeyDirection.Unknown,
                alt, control, shift);
        }

        private static IntPtr SetHook(Win32Methods.LowLevelKeyboardProc proc) {
            // NOTE: This requires FullTrust to use the Process class.
            //       There don't seem to be alternatives to achieving this in
            //       MediumTrust environment which is fine because that's a
            //       concept that has long been obsoleted. But just a warning
            //       if you ever try and run Carnac in that sort of way.
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return Win32Methods.SetWindowsHookEx(Win32Methods.WH_KEYBOARD_LL, proc, Win32Methods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }
    }
}