using Carnac.Logic;
using Carnac.Logic.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Windows;

namespace Carnac.logic.Models {
    public partial class PopupSettings : NotifyPropertyChanged {
        [ObservableProperty]
        [property: DefaultValue(350)]
        private int itemMaxWidth;

        [ObservableProperty]
        [property: DefaultValue(0.5)]
        private double itemOpacity;

        [ObservableProperty]
        [property: DefaultValue(5)]
        private double itemFadeDelay;

        /// <summary>
        /// Maximum number of messages visible on screen. 0 = unlimited.
        /// </summary>
        [ObservableProperty]
        [property: DefaultValue(5)]
        private int maxMessages;

        [ObservableProperty]
        [property: DefaultValue("Black")]
        private string itemBackgroundColor;

        [ObservableProperty]
        [property: DefaultValue("White")]
        private string fontColor;

        [ObservableProperty]
        [property: DefaultValue(40)]
        private int fontSize;

        [ObservableProperty]
        private int screen;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ScaleTransform))]
        [NotifyPropertyChangedFor(nameof(Alignment))]
        private NotificationPlacement placement;

        //Used to determine which from it's leftmost co-ord
        private double left;
        public double Left {
            get => left;
            set {
                if (SetProperty(ref left, value)) {
                    LeftChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler LeftChanged;

        private double top;
        public double Top {
            get => top;
            set {
                if (SetProperty(ref top, value)) {
                    TopChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler TopChanged;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Margins))]
        private int topOffset;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Margins))]
        private int bottomOffset;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Margins))]
        private int leftOffset;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Margins))]
        private int rightOffset;

        [ObservableProperty]
        [property: DefaultValue("")]
        private string processFilterExpression;

        public double ScaleTransform => Placement is NotificationPlacement.TopLeft or NotificationPlacement.TopRight ? 1 : -1;

        public string Alignment => Placement is NotificationPlacement.TopLeft or NotificationPlacement.BottomLeft ? "Left" : "Right";

        public Thickness Margins => new(LeftOffset, TopOffset, RightOffset, BottomOffset);

        [ObservableProperty]
        private bool detectShortcutsOnly;

        [ObservableProperty]
        private bool showApplicationIcon;

        [ObservableProperty]
        private bool settingsConfigured;

        [ObservableProperty]
        private bool showOnlyModifiers;

        [ObservableProperty]
        private bool showSpaceAsUnicode;

        [ObservableProperty]
        [property: DefaultValue(true)]
        private bool showMouseClicks;

        [ObservableProperty]
        [property: DefaultValue(true)]
        private bool showMouseClickKeys;

        [ObservableProperty]
        [property: DefaultValue(true)]
        private bool showMouseScrollKeys;

        [ObservableProperty]
        [property: DefaultValue(40)]
        private int mouseKeySize;

        [ObservableProperty]
        [property: DefaultValue("OrangeRed")]
        private string leftClickColor;

        [ObservableProperty]
        [property: DefaultValue("RoyalBlue")]
        private string rightClickColor;

        [ObservableProperty]
        [property: DefaultValue("Gold")]
        private string scrollClickColor;

        [ObservableProperty]
        [property: DefaultValue("Peru")]
        private string xButton1ClickColor;

        [ObservableProperty]
        [property: DefaultValue("Plum")]
        private string xButton2ClickColor;

        [ObservableProperty]
        [property: DefaultValue(1)]
        private double clickStartScale;

        [ObservableProperty]
        [property: DefaultValue(4)]
        private double clickStopScale;

        [ObservableProperty]
        [property: DefaultValue(3700)]
        private int clickFadeDelay;

        [ObservableProperty]
        [property: DefaultValue(1)]
        private double clickStartBorder;

        [ObservableProperty]
        [property: DefaultValue(0.8)]
        private double clickStartOpacity;

        [ObservableProperty]
        [property: DefaultValue(2)]
        private double clickStopBorder;

        [ObservableProperty]
        [property: DefaultValue(0)]
        private double clickStopOpacity;

        [ObservableProperty]
        private string clickColor;

        /// <summary>
        /// Optional folder path for additional user-supplied keymap YAML files.
        /// </summary>
        [ObservableProperty]
        [property: DefaultValue("")]
        private string customKeymapsFolder;

        /// <summary>
        /// UI language code: "" (English), "es" (Spanish), "pt-BR" (Portuguese Brazil).
        /// </summary>
        [ObservableProperty]
        [property: DefaultValue("")]
        private string language;
    }
}
