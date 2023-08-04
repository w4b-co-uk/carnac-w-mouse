using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace CarnacCore {
    public class CarnacTrayIcon: IDisposable {
        private readonly NotifyIcon trayIcon;

        public CarnacTrayIcon() {
            ToolStripMenuItem exitMenuItem = new() {
                Text = "Exit" //Properties.Resources.ShellView_Exit
            };

            Stream iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CarnacCore.icon.embedded.ico");

            ContextMenuStrip contextMenu = new();
            _ = contextMenu.Items.Add(exitMenuItem);

            trayIcon = new NotifyIcon {
                Icon = new Icon(iconStream),
                ContextMenuStrip = contextMenu
            };

            exitMenuItem.Click += (sender, args) => {
                trayIcon.Visible = false;
                Application.Current.Shutdown();
            };
            trayIcon.MouseClick += NotifyIconClick;
            trayIcon.Visible = true;
        }

        public event Action OpenPreferences = () => { };

        private void NotifyIconClick(object sender, MouseEventArgs mouseEventArgs) {
            if (mouseEventArgs.Button == MouseButtons.Left) {
                Window preferencesWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(x => x.Name == "PreferencesViewWindow");
                if (preferencesWindow != null) {
                    _ = preferencesWindow.Activate();
                } else {
                    OpenPreferences();
                }
            }
        }

        public void Dispose() {
            trayIcon.Dispose();
        }
    }
}