using Carnac.Logic.KeyMonitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carnac.Logic.MouseMonitor {
    public class InterceptMouse: IInterceptMouse {

        public static readonly InterceptMouse Current = new InterceptMouse();
        private readonly IObservable<InterceptKeyEventArgs> keyStream;

        private readonly Channel<InterceptKeyEventArgs> channel =
            Channel.CreateBounded<InterceptKeyEventArgs>(
                new BoundedChannelOptions(64) {
                    FullMode = BoundedChannelFullMode.DropOldest,
                    SingleWriter = true,
                    SingleReader = true
                });

        public static readonly List<Keys> MouseKeys = new List<Keys>()
        {
            Keys.LButton,
            Keys.MButton,
            Keys.RButton,
            Keys.XButton1,
            Keys.XButton2,
            Keys.VolumeUp,
            Keys.VolumeDown
        };

        // Must be stored as a field to prevent GC of the unmanaged callback
        private Win32Methods.LowLevelMouseProc mouseCallback;

        private InterceptMouse() {
            keyStream = Observable.Create<InterceptKeyEventArgs>(observer => {
                CancellationTokenSource cts = new();
                Task readerTask = Task.Run(async () => {
                    await foreach (InterceptKeyEventArgs item in channel.Reader.ReadAllAsync(cts.Token)) {
                        observer.OnNext(item);
                    }
                }, cts.Token);

                IntPtr hookId = IntPtr.Zero;
                mouseCallback = (nCode, wParam, lParam) => {
                    if (nCode >= 0) {
                        InterceptKeyEventArgs eventArgs = CreateMouseEventArgs(wParam, lParam);
                        if (eventArgs != null) {
                            channel.Writer.TryWrite(eventArgs);
                        }
                    }
                    return Win32Methods.CallNextHookEx(hookId, nCode, wParam, lParam);
                };

                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule) {
                    hookId = Win32Methods.SetWindowsHookExMouse(
                        Win32Methods.WH_MOUSE_LL,
                        mouseCallback,
                        Win32Methods.GetModuleHandle(curModule.ModuleName),
                        0);
                }
                Debug.Write("Subscribed to mouse");

                return Disposable.Create(() => {
                    Debug.Write("Unsubscribed from mouse");
                    _ = Win32Methods.UnhookWindowsHookEx(hookId);
                    mouseCallback = null;
                    cts.Cancel();
                    cts.Dispose();
                });
            })
            .Publish().RefCount();
        }

        private static InterceptKeyEventArgs CreateMouseEventArgs(IntPtr wParam, IntPtr lParam) {
            int msg = (int)wParam;
            bool alt = (Control.ModifierKeys & Keys.Alt) != 0;
            bool control = (Control.ModifierKeys & Keys.Control) != 0;
            bool shift = (Control.ModifierKeys & Keys.Shift) != 0;

            Keys key = msg switch {
                Win32Methods.WM_LBUTTONDOWN => Keys.LButton,
                Win32Methods.WM_RBUTTONDOWN => Keys.RButton,
                Win32Methods.WM_MBUTTONDOWN => Keys.MButton,
                Win32Methods.WM_XBUTTONDOWN => GetXButton(lParam),
                Win32Methods.WM_MOUSEWHEEL => GetWheelKey(lParam),
                _ => Keys.None
            };

            return key == Keys.None ? null : new InterceptKeyEventArgs(key, KeyDirection.Down, alt, control, shift);
        }

        private static Keys GetXButton(IntPtr lParam) {
            Win32Methods.MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<Win32Methods.MSLLHOOKSTRUCT>(lParam);
            int xButton = (hookStruct.mouseData >> 16) & 0xFFFF;
            return xButton == 1 ? Keys.XButton1 : Keys.XButton2;
        }

        private static Keys GetWheelKey(IntPtr lParam) {
            Win32Methods.MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<Win32Methods.MSLLHOOKSTRUCT>(lParam);
            short delta = (short)((hookStruct.mouseData >> 16) & 0xFFFF);
            return delta > 0 ? Keys.VolumeUp : Keys.VolumeDown;
        }

        public IObservable<InterceptKeyEventArgs> GetKeyStream() {
            return keyStream;
        }
    }
}

