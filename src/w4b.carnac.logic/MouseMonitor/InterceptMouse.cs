﻿using Carnac.Logic.KeyMonitor;
using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Carnac.Logic.MouseMonitor {
    public class InterceptMouse: IInterceptKeys {

        public static readonly InterceptMouse Current = new InterceptMouse();
        private readonly IKeyboardMouseEvents m_GlobalHook = Hook.GlobalEvents();
        private readonly IObservable<InterceptKeyEventArgs> keyStream;
        private IObserver<InterceptKeyEventArgs> observer;
        private readonly KeysConverter kc = new KeysConverter();

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

        private InterceptMouse() {
            keyStream = Observable.Create<InterceptKeyEventArgs>(observer => {
                this.observer = observer;
                m_GlobalHook.MouseClick += OnMouseClick;
                m_GlobalHook.MouseDoubleClick += OnMouseDoubleClick;
                m_GlobalHook.MouseWheel += HookManager_MouseWheel;
                Debug.Write("Subscribed to mouse");

                return Disposable.Create(() => {
                    m_GlobalHook.MouseClick -= OnMouseClick;
                    m_GlobalHook.MouseDoubleClick -= OnMouseDoubleClick;
                    m_GlobalHook.MouseWheel -= HookManager_MouseWheel;
                    m_GlobalHook.Dispose();
                    Debug.Write("Unsubscribed from mouse");
                });
            })
            .Publish().RefCount();
        }

        private Keys MouseButtonsToKeys(MouseButtons button) {
            switch (button) {
                case MouseButtons.Left:
                    return Keys.LButton;
                case MouseButtons.Middle:
                    return Keys.MButton;
                case MouseButtons.Right:
                    return Keys.RButton;
                case MouseButtons.XButton1:
                    return Keys.XButton1;
                case MouseButtons.XButton2:
                    return Keys.XButton2;
                default:
                    return Keys.None;
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e) {
            bool alt = (Control.ModifierKeys & Keys.Alt) != 0;
            bool control = (Control.ModifierKeys & Keys.Control) != 0;
            bool shift = (Control.ModifierKeys & Keys.Shift) != 0;
            observer.OnNext(new InterceptKeyEventArgs(
                MouseButtonsToKeys(e.Button),
                KeyDirection.Down,
                alt,
                control,
                shift));
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e) {
            observer.OnNext(new InterceptKeyEventArgs(
                MouseButtonsToKeys(e.Button),
                KeyDirection.Down,
                Control.ModifierKeys == Keys.Alt,
                Control.ModifierKeys == Keys.Control,
                Control.ModifierKeys == Keys.Shift));
        }

        private void HookManager_MouseWheel(object sender, MouseEventArgs e) {
            // for now using VolumeDown and Up as proxy could be refactored
            observer.OnNext(new InterceptKeyEventArgs(
                e.Delta > 0 ? Keys.VolumeUp : Keys.VolumeDown,
                KeyDirection.Down,
                Control.ModifierKeys == Keys.Alt,
                Control.ModifierKeys == Keys.Control,
                Control.ModifierKeys == Keys.Shift));
        }

        public IObservable<InterceptKeyEventArgs> GetKeyStream() {
            return keyStream;
        }
    }
}

