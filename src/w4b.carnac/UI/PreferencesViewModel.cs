using Carnac.Logic;
using Carnac.Logic.Enums;
using Carnac.Logic.Native;
using Carnac.UI;
using Carnac.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using Carnac.Logic.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using Carnac.logic.Models;

namespace Carnac.UI {
    public class LanguageOption {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public partial class PreferencesViewModel : NotifyPropertyChanged {
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

            SaveCommand = new DelegateCommand(async () => await SaveSettingsAsync());
            ResetToDefaultsCommand = new DelegateCommand(async () => await settingsProvider.ResetToDefaultsAsync<PopupSettings>());
            BrowseKeymapsFolderCommand = new DelegateCommand(BrowseKeymapsFolder);
            VisitCommand = new DelegateCommand(Visit);

            SelectedLanguage = Settings.Language ?? "";
        }

        public ICommand VisitCommand { get; private set; }

        public ICommand ResetToDefaultsCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand BrowseKeymapsFolderCommand { get; private set; }

        public ObservableCollection<AvailableColor> AvailableColors { get; private set; }

        public List<LanguageOption> AvailableLanguages { get; } = new() {
            new LanguageOption { Name = "English", Code = "" },
            new LanguageOption { Name = "Español", Code = "es" },
            new LanguageOption { Name = "Português (Brasil)", Code = "pt-BR" }
        };

        [ObservableProperty]
        private string selectedLanguage;

        partial void OnSelectedLanguageChanged(string value) {
            Settings.Language = value;
            Loc.Instance.SwitchCulture(value);
        }

        [ObservableProperty]
        private ObservableCollection<DetailedScreen> screens;

        [ObservableProperty]
        private DetailedScreen selectedScreen;

        [ObservableProperty]
        private PopupSettings settings;

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
            "CommunityToolkit.Mvvm",
            "NSubstitute",
            "Reactive Extensions"
        };
        public string Authors => string.Join(", ", authors);

        public string Components => string.Join(", ", components);

        [ObservableProperty]
        private AvailableColor fontColor;

        [ObservableProperty]
        private AvailableColor itemBackgroundColor;

        [ObservableProperty]
        private AvailableColor leftClickColor;

        [ObservableProperty]
        private AvailableColor rightClickColor;

        [ObservableProperty]
        private AvailableColor scrollClickColor;

        [ObservableProperty]
        private AvailableColor xButton1ClickColor;

        [ObservableProperty]
        private AvailableColor xButton2ClickColor;

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

        private void BrowseKeymapsFolder() {
            Microsoft.Win32.OpenFolderDialog dialog = new() {
                Title = Loc.Instance.SelectKeymapsFolder
            };
            if (!string.IsNullOrWhiteSpace(Settings.CustomKeymapsFolder)
                && System.IO.Directory.Exists(Settings.CustomKeymapsFolder)) {
                dialog.InitialDirectory = Settings.CustomKeymapsFolder;
            }
            if (dialog.ShowDialog() == true) {
                Settings.CustomKeymapsFolder = dialog.FolderName;
            }
        }

        private async System.Threading.Tasks.Task SaveSettingsAsync() {
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
            await settingsProvider.SaveSettingsAsync(Settings);
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