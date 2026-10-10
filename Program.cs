using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Celowniczek
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
            try
            {
                IconGenerator.GenerateAppIcon(iconPath);
            }
            catch { }

            ConfigData config = ConfigManager.LoadConfig();
            CrosshairForm crosshair = new CrosshairForm();
            ConfigManager.ApplyToForm(config, crosshair);

            _ = UpdateChecker.CheckForUpdatesAsync(silent: true);

            NotifyIcon trayIcon = new NotifyIcon();
            
            if (File.Exists(iconPath))
            {
                trayIcon.Icon = new Icon(iconPath);
            }
            else
            {
                trayIcon.Icon = SystemIcons.Application;
            }

            trayIcon.Text = "Celowniczek";
            trayIcon.Visible = true;

            ContextMenuStrip menu = new ContextMenuStrip();
            
            ToolStripMenuItem itemSettings = new ToolStripMenuItem(config.Language == "pl" ? "Ustawienia" : "Settings");
            itemSettings.Click += (s, e) => {
                SettingsForm settings = new SettingsForm(config, crosshair);
                settings.ShowDialog();
            };

            ToolStripMenuItem itemUpdate = new ToolStripMenuItem(config.Language == "pl" ? "Sprawdź aktualizacje" : "Check for Updates");
            itemUpdate.Click += async (s, e) => {
                await UpdateChecker.CheckForUpdatesAsync(silent: false);
            };

            ToolStripMenuItem itemExit = new ToolStripMenuItem(config.Language == "pl" ? "Wyjście" : "Exit");
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