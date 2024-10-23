using Carnac.Logic;
using Carnac.Logic.Enums;
using Carnac.Logic.Native;
using Carnac.UI;
using SettingsProviderNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using w4b.carnac.logic.Models;

namespace w4b.carnac.UI {
    public class PreferencesViewModel: NotifyPropertyChanged {
        private readonly ISettingsProvider settingsProvider;

        public PreferencesViewModel(ISettingsProvider settingsProvider, IScreenManager screenManager) {
            this.settingsProvider = settingsProvider;

            Screens = new ObservableCollection<DetailedScreen>(screenManager.GetScreens());

            Settings = settingsProvider.GetSettings<PopupSettings>();

            PlaceScreen();

            AvailableColors = new ObservableCollection<AvailableColor>();
            PropertyInfo[] properties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (PropertyInfo prop in properties) {
                string name = prop.Name;
                Color value = (Color)prop.GetValue(null, null);

                AvailableColor availableColor = new(name, value);
                if (Settings.FontColor == name) {
                    FontColor = availableColor;
                }

                if (Settings.ItemBackgroundColor == name) {
                    ItemBackgroundColor = availableColor;
                }

                if (Settings.LeftClickColor == name) {
                    LeftClickColor = availableColor;
                }

                if (Settings.RightClickColor == name) {
                    RightClickColor = availableColor;
                }

                if (Settings.ScrollClickColor == name) {
                    ScrollClickColor = availableColor;
                }

                if (Settings.XButton1ClickColor == name) {
                    XButton1ClickColor = availableColor;
                }

                if (Settings.XButton2ClickColor == name) {
                    XButton2ClickColor = availableColor;
                }

                AvailableColors.Add(availableColor);
            }

            LeftClickColor ??= new AvailableColor("OrangeRed", Colors.OrangeRed);
            RightClickColor ??= new AvailableColor("RoyalBlue", Colors.RoyalBlue);
            ScrollClickColor ??= new AvailableColor("Gold", Colors.Gold);
            XButton1ClickColor ??= new AvailableColor("Peru", Colors.Peru);
            XButton2ClickColor ??= new AvailableColor("Plum", Colors.Plum);

            SaveCommand = new DelegateCommand(SaveSettings);
            ResetToDefaultsCommand = new DelegateCommand(() => settingsProvider.ResetToDefaults<PopupSettings>());
            VisitCommand = new DelegateCommand(Visit);
        }

        public ICommand VisitCommand { get; private set; }

        public ICommand ResetToDefaultsCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ObservableCollection<AvailableColor> AvailableColors { get; private set; }

        public ObservableCollection<DetailedScreen> Screens { get; set; }

        public DetailedScreen SelectedScreen { get; set; }

        public PopupSettings Settings { get; set; }

        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private readonly List<string> authors = new() {
            "Brendan Forster",
            "Alex Friedman",
            "Jon Galloway",
            "Jake Ginnivan",
            "Paul Jenkins",
            "Dmitry Pursanov",
            "Chris Sainty",
            "Andrew Tobin",
            "Henrik Andersson",
            "Boris Fritscher"
        };
        private readonly List<string> components = new() {
            "MahApps.Metro",
            "Fody",
            "NSubstitute",
            "Reactive Extensions",
            "Squirrel.Windows",
            "MouseKeyHook"
        };
        public string Authors => string.Join(", ", authors);

        public string Components => string.Join(", ", components);

        public AvailableColor FontColor { get; set; }

        public AvailableColor ItemBackgroundColor { get; set; }

        public AvailableColor LeftClickColor { get; set; }

        public AvailableColor RightClickColor { get; set; }

        public AvailableColor ScrollClickColor { get; set; }

        public AvailableColor XButton1ClickColor { get; set; }

        public AvailableColor XButton2ClickColor { get; set; }

        private void Visit() {
            try {
                _ = Process.Start(new ProcessStartInfo {
                    FileName = "http://code52.org/carnac/",
                    UseShellExecute = true
                });
            } catch (Exception ex) {
                Debug.WriteLine($"Failed to open URL: {ex.Message}");
            }
        }

        private void SaveSettings() {
            if (Screens.Count < 1) {
                return;
            }

            SelectedScreen ??= Screens.First();

            Settings.Screen = SelectedScreen.Index;

            Settings.Placement = SelectedScreen.NotificationPlacementTopLeft
                ? NotificationPlacement.TopLeft
                : SelectedScreen.NotificationPlacementBottomLeft
                    ? NotificationPlacement.BottomLeft
                    : SelectedScreen.NotificationPlacementTopRight
                                    ? NotificationPlacement.TopRight
                                    : SelectedScreen.NotificationPlacementBottomRight ? NotificationPlacement.BottomRight : NotificationPlacement.BottomLeft;

            PlaceScreen();

            Settings.SettingsConfigured = true;
            Settings.FontColor = FontColor.Name;
            Settings.ItemBackgroundColor = ItemBackgroundColor.Name;
            Settings.LeftClickColor = LeftClickColor.Name;
            Settings.RightClickColor = RightClickColor.Name;
            Settings.ScrollClickColor = ScrollClickColor.Name;
            Settings.XButton1ClickColor = XButton1ClickColor.Name;
            Settings.XButton2ClickColor = XButton2ClickColor.Name;
            settingsProvider.SaveSettings(Settings);
        }

        private void PlaceScreen() {
            if (Screens == null) {
                return;
            }

            SelectedScreen = Screens.FirstOrDefault(s => s.Index == Settings.Screen);

            if (SelectedScreen == null) {
                return;
            }

            switch (Settings.Placement) {
                case NotificationPlacement.TopLeft:
                    SelectedScreen.NotificationPlacementTopLeft = true;
                    break;
                case NotificationPlacement.BottomLeft:
                    SelectedScreen.NotificationPlacementBottomLeft = true;
                    break;
                case NotificationPlacement.TopRight:
                    SelectedScreen.NotificationPlacementTopRight = true;
                    break;
                case NotificationPlacement.BottomRight:
                    SelectedScreen.NotificationPlacementBottomRight = true;
                    break;
                default:
                    SelectedScreen.NotificationPlacementBottomLeft = true;
                    break;
            }

            Settings.Left = SelectedScreen.Left;
            Settings.Top = SelectedScreen.Top;
        }
    }
}