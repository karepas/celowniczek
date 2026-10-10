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

            _ = UpdateChecker.CheckForUpdatesAsync(silent: true, language: config.Language);

            NotifyIcon trayIcon = new NotifyIcon();
            
            if (File.Exists(iconPath))
            {
                try
                {
                    trayIcon.Icon = new Icon(iconPath);
                }
                catch (ArgumentException)
                {
                    trayIcon.Icon = SystemIcons.Application;
                }
            }
            else
            {
                trayIcon.Icon = SystemIcons.Application;
            }

            trayIcon.Text = "Celowniczek";
            trayIcon.Visible = true;

            ContextMenuStrip menu = new ContextMenuStrip();
            
            ToolStripMenuItem itemSettings = new ToolStripMenuItem();
            ToolStripMenuItem itemUpdate = new ToolStripMenuItem();
            ToolStripMenuItem itemExit = new ToolStripMenuItem();

            void RefreshTrayTexts()
            {
                bool polish = config.Language == "pl";
                itemSettings.Text = polish ? "Ustawienia" : "Settings";
                itemUpdate.Text = polish ? "Sprawdź aktualizacje" : "Check for Updates";
                itemExit.Text = polish ? "Wyjście" : "Exit";
            }

            RefreshTrayTexts();
            itemSettings.Click += (s, e) => {
                using (SettingsForm settings = new SettingsForm(config, crosshair))
                {
                    settings.ShowDialog();
                }
                RefreshTrayTexts();
            };

            itemUpdate.Click += async (s, e) => {
                await UpdateChecker.CheckForUpdatesAsync(silent: false, language: config.Language);
            };

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