using System;
using System.Drawing;
using System.Windows.Forms;

namespace Celowniczek
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            ConfigData config = ConfigManager.LoadConfig();
            CrosshairForm crosshair = new CrosshairForm();
            ConfigManager.ApplyToForm(config, crosshair);

            _ = UpdateChecker.CheckForUpdatesAsync(silent: true);

            NotifyIcon trayIcon = new NotifyIcon();
            trayIcon.Icon = SystemIcons.Application;
            trayIcon.Text = "Celowniczek";
            trayIcon.Visible = true;

            ContextMenuStrip menu = new ContextMenuStrip();
            
            ToolStripMenuItem itemSettings = new ToolStripMenuItem("Ustawienia");
            itemSettings.Click += (s, e) => {
                SettingsForm settings = new SettingsForm(config, crosshair);
                settings.ShowDialog();
            };

            ToolStripMenuItem itemUpdate = new ToolStripMenuItem("Sprawdź aktualizacje");
            itemUpdate.Click += async (s, e) => {
                await UpdateChecker.CheckForUpdatesAsync(silent: false);
            };

            ToolStripMenuItem itemExit = new ToolStripMenuItem("Wyjście");
            itemExit.Click += (s, e) => {
                trayIcon.Visible = false;
                Application.Exit();
            };

            menu.Items.Add(itemSettings);
            menu.Items.Add(itemUpdate);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemExit);

            trayIcon.ContextMenuStrip = menu;

            crosshair.Show();
            Application.Run();
        }
    }
}