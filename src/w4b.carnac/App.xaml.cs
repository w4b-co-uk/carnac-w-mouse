using Carnac.Logic;
using Carnac.Logic.KeyMonitor;
using Carnac.UI;
using Carnac.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SettingsProviderNet;
using System;
using System.IO;
using System.Windows;
using Carnac.logic;
using Carnac.logic.Models;

namespace Carnac {
    public partial class App : IDisposable {
        private IHost host;
        private KeyShowView keyShowView;
        private CarnacTrayIcon trayIcon;
        private KeysController carnac;

        public App() {
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Carnac", "logs", "carnac-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();

            host = Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices(services => {
                    // Settings infrastructure
                    services.AddSingleton<ISettingsProvider>(
                        new SettingsProvider(new RoamingAppDataStorage("Carnac")));
                    services.AddSingleton(sp =>
                        sp.GetRequiredService<ISettingsProvider>().GetSettings<PopupSettings>());

                    // Core services (logic layer)
                    services.AddSingleton<IInterceptKeys>(InterceptKeys.Current);
                    services.AddSingleton<IPasswordModeService, PasswordModeService>();
                    services.AddSingleton<IDesktopLockEventService, DesktopLockEventService>();
                    services.AddSingleton<IKeyProvider, KeyProvider>();
                    services.AddSingleton<IShortcutProvider, ShortcutProvider>();
                    services.AddSingleton<IMessageProvider, MessageProvider>();
                    services.AddSingleton<IScreenManager, ScreenManager>();
                    services.AddSingleton<IConcurrencyService, ConcurrencyService>();

                    // UI layer
                    services.AddSingleton<KeyShowViewModel>();
                    services.AddTransient<PreferencesViewModel>();
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e) {
            if (ProcessUtilities.ThisProcessIsAlreadyRunning()) {
                ProcessUtilities.SetFocusToPreviousInstance("Carnac");
                Shutdown();
                return;
            }

            var services = host.Services;

            Log.Information("Carnac starting up");

            trayIcon = new CarnacTrayIcon();
            trayIcon.OpenPreferences += TrayIconOnOpenPreferences;

            var keyShowViewModel = services.GetRequiredService<KeyShowViewModel>();
            keyShowView = new KeyShowView(keyShowViewModel);
            keyShowView.Show();

            var messageProvider = services.GetRequiredService<IMessageProvider>();
            var concurrencyService = services.GetRequiredService<IConcurrencyService>();
            var settingsProvider = services.GetRequiredService<ISettingsProvider>();
            carnac = new KeysController(keyShowViewModel.Messages, messageProvider, concurrencyService, settingsProvider);
            carnac.Start();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e) {
            trayIcon.Dispose();
            carnac.Dispose();
            keyShowView.Dispose();
            ProcessUtilities.DestroyMutex();
            host.Dispose();
            Log.CloseAndFlush();

            base.OnExit(e);
        }

        private void TrayIconOnOpenPreferences() {
            var preferencesViewModel = host.Services.GetRequiredService<PreferencesViewModel>();
            PreferencesView preferencesView = new(preferencesViewModel);
            preferencesView.Show();
        }

        public void Dispose() {
            host?.Dispose();
        }
    }
}
