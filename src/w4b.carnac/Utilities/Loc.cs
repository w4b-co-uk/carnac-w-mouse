using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Carnac.Utilities {
    public sealed class Loc : INotifyPropertyChanged {
        private static readonly Loc instance = new();
        public static Loc Instance => instance;

        private static readonly ResourceManager rm =
            new("w4b.carnac.Properties.Strings",
                typeof(Loc).Assembly);

        public event PropertyChangedEventHandler PropertyChanged;

        public void SwitchCulture(string cultureCode) {
            CultureInfo culture = string.IsNullOrEmpty(cultureCode)
                ? CultureInfo.InvariantCulture
                : new CultureInfo(cultureCode);
            CultureInfo.CurrentUICulture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); // refresh all bindings
        }

        private string Get(string key) => rm.GetString(key, CultureInfo.CurrentUICulture) ?? key;

        // Tray
        public string TrayExit => Get(nameof(TrayExit));

        // Tab Headers
        public string TabGeneral => Get(nameof(TabGeneral));
        public string TabKeyboard => Get(nameof(TabKeyboard));
        public string TabMouse => Get(nameof(TabMouse));
        public string TabAbout => Get(nameof(TabAbout));

        // General
        public string TopOffset => Get(nameof(TopOffset));
        public string BottomOffset => Get(nameof(BottomOffset));
        public string LeftOffset => Get(nameof(LeftOffset));
        public string RightOffset => Get(nameof(RightOffset));
        public string Language => Get(nameof(Language));
        public string ResetToDefaults => Get(nameof(ResetToDefaults));
        public string Save => Get(nameof(Save));

        // Keyboard
        public string PopupTextWidth => Get(nameof(PopupTextWidth));
        public string PopupOpacity => Get(nameof(PopupOpacity));
        public string PopupFadeDelay => Get(nameof(PopupFadeDelay));
        public string MaxMessages => Get(nameof(MaxMessages));
        public string FontSize => Get(nameof(FontSize));
        public string FontColour => Get(nameof(FontColour));
        public string BackgroundColor => Get(nameof(BackgroundColor));
        public string ShortcutsOnly => Get(nameof(ShortcutsOnly));
        public string ShortcutsOnlyDesc => Get(nameof(ShortcutsOnlyDesc));
        public string CustomKeymaps => Get(nameof(CustomKeymaps));
        public string CustomKeymapsDesc => Get(nameof(CustomKeymapsDesc));
        public string Browse => Get(nameof(Browse));
        public string OnlyModifiers => Get(nameof(OnlyModifiers));
        public string OnlyModifiersDesc => Get(nameof(OnlyModifiersDesc));
        public string ShowSpaceAsUnicode => Get(nameof(ShowSpaceAsUnicode));
        public string ShowSpaceAsUnicodeDesc => Get(nameof(ShowSpaceAsUnicodeDesc));
        public string ShowApplicationIcon => Get(nameof(ShowApplicationIcon));
        public string ShowApplicationIconDesc => Get(nameof(ShowApplicationIconDesc));
        public string ProcessFilter => Get(nameof(ProcessFilter));
        public string ProcessFilterDesc => Get(nameof(ProcessFilterDesc));

        // Mouse
        public string ShowMouseClicks => Get(nameof(ShowMouseClicks));
        public string ShowClicksAsKeys => Get(nameof(ShowClicksAsKeys));
        public string ShowScrollAsKeys => Get(nameof(ShowScrollAsKeys));
        public string MouseKeySize => Get(nameof(MouseKeySize));
        public string StartScale => Get(nameof(StartScale));
        public string StartBorder => Get(nameof(StartBorder));
        public string StartOpacity => Get(nameof(StartOpacity));
        public string StopScale => Get(nameof(StopScale));
        public string StopBorder => Get(nameof(StopBorder));
        public string StopOpacity => Get(nameof(StopOpacity));
        public string CircleFadeDelay => Get(nameof(CircleFadeDelay));
        public string LeftClickColor => Get(nameof(LeftClickColor));
        public string RightClickColor => Get(nameof(RightClickColor));
        public string ScrollClickColor => Get(nameof(ScrollClickColor));
        public string XButton1ClickColor => Get(nameof(XButton1ClickColor));
        public string XButton2ClickColor => Get(nameof(XButton2ClickColor));

        // About
        public string AboutCode52 => Get(nameof(AboutCode52));
        public string AboutProjectBy => Get(nameof(AboutProjectBy));
        public string AboutUses => Get(nameof(AboutUses));
        public string AboutVersion => Get(nameof(AboutVersion));
        public string VisitWebsite => Get(nameof(VisitWebsite));

        // Dialogs
        public string SelectKeymapsFolder => Get(nameof(SelectKeymapsFolder));
        public string LanguageRestart => Get(nameof(LanguageRestart));
    }
}
