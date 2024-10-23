using Carnac.Logic;
using Carnac.Logic.Enums;
using System;
using System.ComponentModel;
using System.Windows;

namespace w4b.carnac.logic.Models {
    public class PopupSettings: NotifyPropertyChanged {
        [DefaultValue(350)]
        public int ItemMaxWidth { get; set; }

        [DefaultValue(0.5)]
        public double ItemOpacity { get; set; }

        [DefaultValue(5)]
        public double ItemFadeDelay { get; set; }

        [DefaultValue("Black")]
        public string ItemBackgroundColor { get; set; }

        [DefaultValue("White")]
        public string FontColor { get; set; }

        [DefaultValue(40)]
        public int FontSize { get; set; }

        public int Screen { get; set; }

        [NotifyProperty(AlsoNotifyFor = new[] { "ScaleTransform", "Alignment" })]
        public NotificationPlacement Placement { get; set; }

        //Used to determine which from it's leftmost co-ord
        private double left;
        public double Left {
            get => left;
            set {
                left = value;
                OnLeftChanged(EventArgs.Empty);
            }
        }

        public event EventHandler LeftChanged;

        protected void OnLeftChanged(EventArgs e) {
            LeftChanged?.Invoke(this, e);
        }

        private double top;
        public double Top {
            get => top;
            set {
                top = value;
                OnTopChanged(EventArgs.Empty);
            }
        }

        public event EventHandler TopChanged;

        protected void OnTopChanged(EventArgs e) {
            TopChanged?.Invoke(this, e);
        }

        [NotifyProperty(AlsoNotifyFor = new[] { "Margins" })]
        public int TopOffset { get; set; }

        [NotifyProperty(AlsoNotifyFor = new[] { "Margins" })]
        public int BottomOffset { get; set; }

        [NotifyProperty(AlsoNotifyFor = new[] { "Margins" })]
        public int LeftOffset { get; set; }

        [NotifyProperty(AlsoNotifyFor = new[] { "Margins" })]
        public int RightOffset { get; set; }

        [DefaultValue("")]
        public string ProcessFilterExpression { get; set; }

        public double ScaleTransform => Placement is NotificationPlacement.TopLeft or NotificationPlacement.TopRight ? 1 : -1;

        public string Alignment => Placement is NotificationPlacement.TopLeft or NotificationPlacement.BottomLeft ? "Left" : "Right";

        public Thickness Margins => new(LeftOffset, TopOffset, RightOffset, BottomOffset);

        public bool DetectShortcutsOnly { get; set; }

        public bool ShowApplicationIcon { get; set; }

        public bool SettingsConfigured { get; set; }

        public bool ShowOnlyModifiers { get; set; }

        public bool ShowSpaceAsUnicode { get; set; }

        [DefaultValue(true)]
        public bool ShowMouseClicks { get; set; }

        [DefaultValue(true)]
        public bool ShowMouseClickKeys { get; set; }

        [DefaultValue(true)]
        public bool ShowMouseScrollKeys { get; set; }

        [DefaultValue(40)]
        public int MouseKeySize { get; set; }

        [DefaultValue("OrangeRed")]
        public string LeftClickColor { get; set; }

        [DefaultValue("RoyalBlue")]
        public string RightClickColor { get; set; }

        [DefaultValue("Gold")]
        public string ScrollClickColor { get; set; }

        [DefaultValue("Peru")]
        public string XButton1ClickColor { get; set; }

        [DefaultValue("Plum")]
        public string XButton2ClickColor { get; set; }

        [DefaultValue(1)]
        public double ClickStartScale { get; set; }

        [DefaultValue(4)]
        public double ClickStopScale { get; set; }

        [DefaultValue(3700)]
        public int ClickFadeDelay { get; set; }

        [DefaultValue(1)]
        public double ClickStartBorder { get; set; }

        [DefaultValue(0.8)]
        public double ClickStartOpacity { get; set; }

        [DefaultValue(2)]
        public double ClickStopBorder { get; set; }

        [DefaultValue(0)]
        public double ClickStopOpacity { get; set; }
        public string ClickColor { get; set; }
    }
}
