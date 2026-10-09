using System;
using System.Windows.Forms;

namespace Celowniczek
{
    internal static class Program
    {
        private static NotifyIcon _notifyIcon = null!;
        private static CrosshairForm _crosshairForm = null!;
        private static SettingsForm _settingsForm = null!;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _crosshairForm = new CrosshairForm();
            
            ConfigManager.LoadConfig(_crosshairForm);

            _crosshairForm.Show();

            _settingsForm = new SettingsForm(_crosshairForm);

            ContextMenuStrip trayMenu = new ContextMenuStrip();
            
            ToolStripMenuItem toggleItem = new ToolStripMenuItem("Pokaż / Ukryj Celownik");
            toggleItem.Click += (s, e) =>
            {
                if (_crosshairForm.Visible)
                    _crosshairForm.Hide();
                else
                    _crosshairForm.Show();
            };

            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Ustawienia Celownika");
            settingsItem.Click += (s, e) =>
            {
                _settingsForm.Show();
                _settingsForm.BringToFront();
            };

            // Dodajemy przycisk ręcznego sprawdzania aktualizacji w Tray Menu
            ToolStripMenuItem updateItem = new ToolStripMenuItem("Sprawdź aktualizacje");
            updateItem.Click += async (s, e) =>
            {
                await UpdateChecker.CheckForUpdatesAsync(silent: false);
            };

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Wyjście");
            exitItem.Click += (s, e) =>
            {
                _notifyIcon.Visible = false;
                Application.Exit();
            };

            trayMenu.Items.Add(settingsItem);
            trayMenu.Items.Add(toggleItem);
            trayMenu.Items.Add(updateItem);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(exitItem);

            _notifyIcon = new NotifyIcon()
            {
                Icon = IconGenerator.CreateCrosshairIcon(),
                ContextMenuStrip = trayMenu,
                Text = "Celowniczek",
                Visible = true
            };

            _notifyIcon.DoubleClick += (s, e) =>
            {
                _settingsForm.Show();
                _settingsForm.BringToFront();
            };

            // Automatyczne sprawdzanie w tle po uruchomieniu aplikacji (ciche, bez komunikatów o braku aktualizacji)
            _ = UpdateChecker.CheckForUpdatesAsync(silent: true);

            Application.Run();
        }
    }
}