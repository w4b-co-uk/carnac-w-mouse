﻿using Carnac.Logic;
using Gma.System.MouseKeyHook;
using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Carnac.UI {
    public partial class KeyShowView: IDisposable {
        private Storyboard sb;
        private IKeyboardMouseEvents m_GlobalHook = null;

        public KeyShowView(KeyShowViewModel keyShowViewModel) {
            DataContext = keyShowViewModel;
            InitializeComponent();
            keyShowViewModel.Settings.PropertyChanged += Settings_PropertyChanged;
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);

            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            Win32Methods.SetWindowExTransparentAndNotInWindowList(hwnd);
            Timer timer = new(100);
            timer.Elapsed +=
                (s, x) => SetWindowPos(hwnd,
                                 HWND.TOPMOST,
                                 0, 0, 0, 0,
                                 (uint)(SWP.NOMOVE | SWP.NOSIZE | SWP.SHOWWINDOW));

            timer.Start();

            KeyShowViewModel vm = (KeyShowViewModel)DataContext;
            Left = vm.Settings.Left;
            vm.Settings.LeftChanged += SettingsLeftChanged;
            Top = vm.Settings.Top;
            vm.Settings.TopChanged += SettingsTopChanged;
            WindowState = WindowState.Maximized;
            if (vm.Settings.ShowMouseClicks) {
                SetupMouseEvents();
            }
        }

        public void Dispose() {
            m_GlobalHook?.Dispose();
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int W, int H, uint uFlags);

        /// <summary>
        /// HWND values for hWndInsertAfter
        /// </summary>
        public static class HWND {
            public static readonly IntPtr
            NOTOPMOST = new(-2),
            BROADCAST = new(0xffff),
            TOPMOST = new(-1),
            TOP = new(0),
            BOTTOM = new(1);
        }

        /// <summary>
        /// SetWindowPos Flags
        /// </summary>
        public static class SWP {
            public static readonly int
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e) {
            sb = FindResource("clickHighlighterStoryboard") as Storyboard;
        }

        private void SettingsLeftChanged(object sender, EventArgs e) {
            WindowState = WindowState.Normal;
            KeyShowViewModel vm = (KeyShowViewModel)DataContext;
            Left = vm.Settings.Left;
            WindowState = WindowState.Maximized;
        }

        private void SettingsTopChanged(object sender, EventArgs e) {
            WindowState = WindowState.Normal;
            KeyShowViewModel vm = (KeyShowViewModel)DataContext;
            Top = vm.Settings.Top;
            WindowState = WindowState.Maximized;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            KeyShowViewModel vm = (KeyShowViewModel)DataContext;
            switch (e.PropertyName) {
                case "ClickFadeDelay":
                    Duration d = TimeSpan.FromMilliseconds(vm.Settings.ClickFadeDelay);
                    foreach (DoubleAnimation da in sb.Children) {
                        da.Duration = d;
                    }
                    break;
                case "ShowMouseClicks":
                    if (vm.Settings.ShowMouseClicks) {
                        SetupMouseEvents();
                    } else {
                        DestroyMouseEvents();
                    }
                    break;
            }
        }

        private void SetupMouseEvents() {
            m_GlobalHook ??= Hook.GlobalEvents();
            m_GlobalHook.MouseDown += OnMouseDown;
            m_GlobalHook.MouseMove += OnMouseMove;
        }

        private void DestroyMouseEvents() {
            if (m_GlobalHook == null) {
                return;
            }
            m_GlobalHook.MouseDown -= OnMouseDown;
            m_GlobalHook.MouseMove -= OnMouseMove;
            m_GlobalHook.Dispose();
            m_GlobalHook = null;
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
            KeyShowViewModel vm = (KeyShowViewModel)DataContext;
            vm.Settings.ClickColor = vm.Settings.LeftClickColor;
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                vm.Settings.ClickColor = vm.Settings.RightClickColor;
            } else if (e.Button == System.Windows.Forms.MouseButtons.Middle) {
                vm.Settings.ClickColor = vm.Settings.ScrollClickColor;
            } else if (e.Button == System.Windows.Forms.MouseButtons.XButton1) {
                vm.Settings.ClickColor = vm.Settings.XButton1ClickColor;
            } else if (e.Button == System.Windows.Forms.MouseButtons.XButton2) {
                vm.Settings.ClickColor = vm.Settings.XButton2ClickColor;
            }
            sb.Begin();
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
            KeyShowViewModel vm = (KeyShowViewModel)DataContext;
            vm.CursorPosition = PointFromScreen(new Point(e.X, e.Y));
        }
    }
}
