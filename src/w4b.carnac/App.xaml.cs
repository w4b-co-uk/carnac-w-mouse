﻿using Carnac.Logic;
using Carnac.Logic.KeyMonitor;
using Carnac.UI;
using Carnac.Utilities;
using SettingsProviderNet;
using System.Net;
using System.Windows;
using w4b.carnac;
using w4b.carnac.logic;
using w4b.carnac.logic.Models;
using w4b.carnac.UI;

namespace Carnac {
    public partial class App: System.IDisposable {
        private readonly SettingsProvider settingsProvider;
        private readonly IMessageProvider messageProvider;
        private readonly PopupSettings settings;
        private KeyShowView keyShowView;
        private CarnacTrayIcon trayIcon;
        private KeysController carnac;

#if !DEBUG
        readonly string carnacUpdateUrl = "https://github.com/Code52/carnac";
#endif

        public App() {
            settingsProvider = new SettingsProvider(new RoamingAppDataStorage("Carnac"));
            settings = settingsProvider.GetSettings<PopupSettings>();
            KeyProvider keyProvider = new(InterceptKeys.Current, new PasswordModeService(), new DesktopLockEventService(), settingsProvider);
            messageProvider = new MessageProvider(new ShortcutProvider(), keyProvider, settings);
        }

        protected override void OnStartup(StartupEventArgs e) {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            // Check if there was instance before this. If there was-close the current one.  
            if (ProcessUtilities.ThisProcessIsAlreadyRunning()) {
                ProcessUtilities.SetFocusToPreviousInstance("Carnac");
                Shutdown();
                return;
            }

            trayIcon = new CarnacTrayIcon();
            trayIcon.OpenPreferences += TrayIconOnOpenPreferences;
            KeyShowViewModel keyShowViewModel = new(settings);
            keyShowView = new KeyShowView(keyShowViewModel);
            keyShowView.Show();

            carnac = new KeysController(keyShowViewModel.Messages, messageProvider, new ConcurrencyService(), settingsProvider);
            carnac.Start();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e) {
            trayIcon.Dispose();
            carnac.Dispose();
            keyShowView.Dispose();
            ProcessUtilities.DestroyMutex();

            base.OnExit(e);
        }

        private void TrayIconOnOpenPreferences() {
            PreferencesViewModel preferencesViewModel = new(settingsProvider, new ScreenManager());
            PreferencesView preferencesView = new(preferencesViewModel);
            preferencesView.Show();
        }

        public void Dispose() {
            throw new System.NotImplementedException();
        }
    }
}
