using System;
using System.Drawing;
using System.Windows.Forms;

namespace Celowniczek
{
    public class SettingsForm : Form
    {
        private readonly ConfigData config;
        private readonly CrosshairForm crosshairForm;
        private readonly TabControl settingsTabs;
        private readonly TabPage settingsTab;
        private readonly TabPage communityTab;
        private readonly Panel settingsPanel;
        private readonly CommunityCrosshairsControl communityControl;

        private Label lblTheme = null!;
        private ComboBox cbTheme = null!;
        private ComboBox cbProfiles = null!;
        private TextBox txtProfileName = null!;
        private Button btnNewProfile = null!;
        private Button btnSaveProfile = null!;
        private Button btnDeleteProfile = null!;
        private Button btnChangePosition = null!;
        private Label lblProfiles = null!;
        private Label lblProfileName = null!;
        private Label lblPositionX = null!;
        private Label lblPositionY = null!;
        private TrackBar tbPositionX = null!;
        private TrackBar tbPositionY = null!;
        private ComboBox cbLanguage = null!;
        private ComboBox cbStyle = null!;
        private Button btnColor = null!;
        private Button btnOutlineColor = null!;
        private CheckBox cbOutline = null!;
        private TrackBar tbSize = null!;
        private TrackBar tbThickness = null!;
        private TrackBar tbGap = null!;
        private TrackBar tbDotRadius = null!;

        private Label lblLang = null!;
        private Label lblStyle = null!;
        private Label lblSize = null!;
        private Label lblThickness = null!;
        private Label lblGap = null!;
        private Label lblDotRadius = null!;
        private bool refreshingTexts;
        private bool creatingProfile;
        private double? pendingPositionX;
        private double? pendingPositionY;

        public SettingsForm(ConfigData config, CrosshairForm crosshairForm)
        {
            this.config = config;
            this.crosshairForm = crosshairForm;

            this.ClientSize = new Size(430, 740);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            settingsTabs = new TabControl { Dock = DockStyle.Fill };
            settingsTab = new TabPage();
            communityTab = new TabPage();
            settingsPanel = new ThemePanel { Dock = DockStyle.Fill, AutoScroll = true };
            communityControl = new CommunityCrosshairsControl(config, crosshairForm);
            settingsTab.Controls.Add(settingsPanel);
            communityTab.Controls.Add(communityControl);
            settingsTabs.TabPages.Add(settingsTab);
            settingsTabs.TabPages.Add(communityTab);
            settingsTabs.SelectedIndexChanged += async (s, e) =>
            {
                if (settingsTabs.SelectedTab == communityTab)
                {
                    await communityControl.EnsureLoadedAsync();
                }
            };
            settingsTabs.DrawItem += SettingsTabs_DrawItem;
            this.Controls.Add(settingsTabs);

            UpdateTexts();
            InitializeComponents();
            ApplyTheme();
        }

        private void UpdateTexts()
        {
            if (config.Language == "pl")
            {
                this.Text = "Celowniczek - Ustawienia";
            }
            else
            {
                this.Text = "Celowniczek - Settings";
            }

            UpdateTabTexts();
            communityControl.UpdateTexts();
        }

        private void InitializeComponents()
        {
            lblProfiles = new Label { Location = new Point(20, 10), AutoSize = true };
            cbProfiles = new ComboBox { Location = new Point(20, 28), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbProfiles.SelectedIndexChanged += (s, e) =>
            {
                if (!refreshingTexts && cbProfiles.SelectedItem is string selectedProfile)
                {
                    creatingProfile = false;
                    pendingPositionX = null;
                    pendingPositionY = null;
                    config.SelectedPositionProfile = selectedProfile;
                    ConfigManager.ApplyToForm(config, crosshairForm);
                    SetPositionSliderValues(crosshairForm.PositionX, crosshairForm.PositionY);
                    txtProfileName.Text = selectedProfile;
                    ConfigManager.SaveConfig(config);
                }
            };

            lblProfileName = new Label { Location = new Point(20, 57), AutoSize = true };
            txtProfileName = new TextBox { Location = new Point(20, 74), Size = new Size(160, 25), MaxLength = 40 };
            btnNewProfile = new Button { Location = new Point(188, 72), Size = new Size(56, 29) };
            btnNewProfile.Click += (s, e) =>
            {
                creatingProfile = true;
                pendingPositionX = null;
                pendingPositionY = null;
                txtProfileName.Clear();
                txtProfileName.Focus();
            };

            btnSaveProfile = new Button { Location = new Point(248, 72), Size = new Size(56, 29) };
            btnSaveProfile.Click += (s, e) => SavePositionProfile();

            btnChangePosition = new Button { Location = new Point(20, 106), Size = new Size(350, 28) };
            btnChangePosition.Click += (s, e) => ChoosePosition();

            btnDeleteProfile = new Button { Location = new Point(308, 72), Size = new Size(62, 29) };
            btnDeleteProfile.Click += (s, e) => DeletePositionProfile();

            lblLang = new Label { Location = new Point(20, 140), AutoSize = true };
            cbLanguage = new ComboBox { Location = new Point(20, 158), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbLanguage.Items.Add("English");
            cbLanguage.Items.Add("Polski");
            cbLanguage.SelectedIndex = config.Language == "pl" ? 1 : 0;
            cbLanguage.SelectedIndexChanged += (s, e) =>
            {
                config.Language = cbLanguage.SelectedIndex == 1 ? "pl" : "en";
                UpdateTexts();
                RefreshLabels();
                ConfigManager.SaveConfig(config);
            };

            lblTheme = new Label { Location = new Point(20, 188), AutoSize = true };
            cbTheme = new ComboBox { Location = new Point(20, 206), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbTheme.SelectedIndexChanged += (s, e) =>
            {
                if (!refreshingTexts && cbTheme.SelectedIndex >= 0)
                {
                    config.Theme = UiThemeManager.AvailableThemes[cbTheme.SelectedIndex].Id;
                    ApplyTheme();
                    ConfigManager.SaveConfig(config);
                }
            };

            CrosshairPositionProfile selectedPosition = ConfigManager.GetSelectedPositionProfile(config);
            lblPositionX = new Label { Location = new Point(20, 236), AutoSize = true };
            tbPositionX = new TrackBar { AutoSize = false, Minimum = 0, Maximum = 1000, TickFrequency = 50, Value = (int)Math.Round(selectedPosition.X * 1000), Location = new Point(20, 253), Size = new Size(350, 44) };
            tbPositionX.ValueChanged += (s, e) => UpdatePositionFromSliders();

            lblPositionY = new Label { Location = new Point(20, 307), AutoSize = true };
            tbPositionY = new TrackBar { AutoSize = false, Minimum = 0, Maximum = 1000, TickFrequency = 50, Value = (int)Math.Round(selectedPosition.Y * 1000), Location = new Point(20, 324), Size = new Size(350, 44) };
            tbPositionY.ValueChanged += (s, e) => UpdatePositionFromSliders();

            lblStyle = new Label { Location = new Point(20, 380), AutoSize = true };
            cbStyle = new ComboBox { Location = new Point(20, 397), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbStyle.SelectedIndexChanged += (s, e) =>
            {
                if (!refreshingTexts && cbStyle.SelectedIndex >= 0)
                {
                    config.Style = (CrosshairStyle)cbStyle.SelectedIndex;
                    ApplySettings();
                }
            };

            btnColor = new Button { Location = new Point(20, 428), Size = new Size(350, 32), Tag = "swatch" };
            btnColor.Click += BtnColor_Click;

            cbOutline = new CheckBox { Checked = config.EnableOutline, Location = new Point(20, 466), AutoSize = true };
            cbOutline.CheckedChanged += (s, e) =>
            {
                config.EnableOutline = cbOutline.Checked;
                btnOutlineColor.Enabled = cbOutline.Checked;
                ApplySettings();
            };

            btnOutlineColor = new Button { Location = new Point(20, 490), Size = new Size(350, 26), Enabled = config.EnableOutline, Tag = "swatch" };
            btnOutlineColor.Click += BtnOutlineColor_Click;

            lblSize = new Label { Location = new Point(20, 520), AutoSize = true };
            tbSize = new TrackBar { AutoSize = false, Minimum = 2, Maximum = 50, Value = Math.Clamp(config.Size, 2, 50), Location = new Point(20, 537), Size = new Size(350, 44) };
            tbSize.ValueChanged += (s, e) =>
            {
                config.Size = tbSize.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblThickness = new Label { Location = new Point(20, 587), AutoSize = true };
            tbThickness = new TrackBar { AutoSize = false, Minimum = 1, Maximum = 10, Value = Math.Clamp(config.Thickness, 1, 10), Location = new Point(20, 604), Size = new Size(350, 44) };
            tbThickness.ValueChanged += (s, e) =>
            {
                config.Thickness = tbThickness.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblGap = new Label { Location = new Point(20, 654), AutoSize = true };
            tbGap = new TrackBar { AutoSize = false, Minimum = 0, Maximum = 30, Value = Math.Clamp(config.Gap, 0, 30), Location = new Point(20, 671), Size = new Size(350, 44) };
            tbGap.ValueChanged += (s, e) =>
            {
                config.Gap = tbGap.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblDotRadius = new Label { Location = new Point(20, 721), AutoSize = true };
            tbDotRadius = new TrackBar { AutoSize = false, Minimum = 0, Maximum = 10, Value = Math.Clamp(config.DotRadius, 0, 10), Location = new Point(20, 738), Size = new Size(350, 44) };
            tbDotRadius.ValueChanged += (s, e) =>
            {
                config.DotRadius = tbDotRadius.Value;
                RefreshLabels();
                ApplySettings();
            };

            RefreshLabels();

            settingsPanel.Controls.Add(lblProfiles);
            settingsPanel.Controls.Add(cbProfiles);
            settingsPanel.Controls.Add(lblProfileName);
            settingsPanel.Controls.Add(txtProfileName);
            settingsPanel.Controls.Add(btnNewProfile);
            settingsPanel.Controls.Add(btnSaveProfile);
            settingsPanel.Controls.Add(btnChangePosition);
            settingsPanel.Controls.Add(btnDeleteProfile);
            settingsPanel.Controls.Add(lblLang);
            settingsPanel.Controls.Add(cbLanguage);
            settingsPanel.Controls.Add(lblTheme);
            settingsPanel.Controls.Add(cbTheme);
            settingsPanel.Controls.Add(lblPositionX);
            settingsPanel.Controls.Add(tbPositionX);
            settingsPanel.Controls.Add(lblPositionY);
            settingsPanel.Controls.Add(tbPositionY);
            settingsPanel.Controls.Add(lblStyle);
            settingsPanel.Controls.Add(cbStyle);
            settingsPanel.Controls.Add(btnColor);
            settingsPanel.Controls.Add(cbOutline);
            settingsPanel.Controls.Add(btnOutlineColor);
            settingsPanel.Controls.Add(lblSize);
            settingsPanel.Controls.Add(tbSize);
            settingsPanel.Controls.Add(lblThickness);
            settingsPanel.Controls.Add(tbThickness);
            settingsPanel.Controls.Add(lblGap);
            settingsPanel.Controls.Add(tbGap);
            settingsPanel.Controls.Add(lblDotRadius);
            settingsPanel.Controls.Add(tbDotRadius);
        }

        private void RefreshLabels()
        {
            bool polish = config.Language == "pl";
            ConfigManager.NormalizeProfiles(config);
            lblProfiles.Text = polish ? "Profil pozycji:" : "Position profile:";
            lblProfileName.Text = polish ? "Nazwa profilu:" : "Profile name:";
            lblTheme.Text = polish ? "Motyw:" : "Theme:";
            lblPositionX.Text = polish ? $"Poziomo: {tbPositionX.Value / 10}%" : $"Horizontal: {tbPositionX.Value / 10}%";
            lblPositionY.Text = polish ? $"Pionowo: {tbPositionY.Value / 10}%" : $"Vertical: {tbPositionY.Value / 10}%";
            btnNewProfile.Text = polish ? "Nowy" : "New";
            btnSaveProfile.Text = polish ? "Zapisz" : "Save";
            btnChangePosition.Text = polish ? "Zmień miejsce" : "Change position";
            btnDeleteProfile.Text = polish ? "Usuń" : "Delete";
            btnDeleteProfile.Enabled = config.PositionProfiles.Count > 1;
            lblLang.Text = polish ? "Język / Language:" : "Language / Język:";
            lblStyle.Text = polish ? "Styl:" : "Style:";
            btnColor.Text = polish ? "Wybierz kolor" : "Choose color";
            cbOutline.Text = polish ? "Włącz obrys" : "Enable outline";
            btnOutlineColor.Text = polish ? "Wybierz kolor obrysu" : "Choose outline color";
            lblSize.Text = polish ? $"Rozmiar: {config.Size}" : $"Size: {config.Size}";
            lblThickness.Text = polish ? $"Grubość: {config.Thickness}" : $"Thickness: {config.Thickness}";
            lblGap.Text = polish ? $"Przerwa: {config.Gap}" : $"Gap: {config.Gap}";
            lblDotRadius.Text = polish ? $"Promień kropki (0 = brak): {config.DotRadius}" : $"Dot radius (0 = off): {config.DotRadius}";
            UpdateTabTexts();

            refreshingTexts = true;
            cbProfiles.BeginUpdate();
            cbProfiles.Items.Clear();
            foreach (CrosshairPositionProfile profile in config.PositionProfiles)
            {
                cbProfiles.Items.Add(profile.Name);
            }
            cbProfiles.SelectedIndex = config.PositionProfiles.FindIndex(profile =>
                string.Equals(profile.Name, config.SelectedPositionProfile, StringComparison.OrdinalIgnoreCase));
            cbProfiles.EndUpdate();

            cbTheme.BeginUpdate();
            cbTheme.Items.Clear();
            foreach (UiThemePalette theme in UiThemeManager.AvailableThemes)
            {
                cbTheme.Items.Add(UiThemeManager.GetDisplayName(theme, polish));
            }
            cbTheme.SelectedIndex = 0;
            for (int i = 0; i < UiThemeManager.AvailableThemes.Count; i++)
            {
                if (string.Equals(UiThemeManager.AvailableThemes[i].Id, config.Theme, StringComparison.OrdinalIgnoreCase))
                {
                    cbTheme.SelectedIndex = i;
                    break;
                }
            }
            cbTheme.EndUpdate();

            cbStyle.BeginUpdate();
            cbStyle.Items.Clear();
            cbStyle.Items.Add(polish ? "Krzyż" : "Cross");
            cbStyle.Items.Add(polish ? "Sama kropka" : "Dot only");
            cbStyle.Items.Add(polish ? "Okrąg" : "Circle");
            cbStyle.SelectedIndex = Enum.IsDefined(config.Style) ? (int)config.Style : 0;
            cbStyle.EndUpdate();
            refreshingTexts = false;

            UpdateColorButton(btnColor, config.ColorHex);
            UpdateColorButton(btnOutlineColor, config.OutlineColorHex);
            if (!creatingProfile)
            {
                txtProfileName.Text = config.SelectedPositionProfile;
            }
        }

        private void UpdateTabTexts()
        {
            if (settingsTab == null || communityTab == null)
            {
                return;
            }

            bool polish = config.Language == "pl";
            settingsTab.Text = polish ? "Ustawienia" : "Settings";
            communityTab.Text = polish ? "Społeczność" : "Community";
        }

        private void ApplyTheme()
        {
            UiThemePalette theme = UiThemeManager.GetTheme(config.Theme);
            UiThemeManager.Apply(this, theme);
            communityControl.ApplyTheme(theme);
            settingsTabs.Invalidate();
        }

        private void SettingsTabs_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= settingsTabs.TabPages.Count)
            {
                return;
            }

            UiThemePalette theme = UiThemeManager.GetTheme(config.Theme);
            bool selected = e.Index == settingsTabs.SelectedIndex;
            Color tabColor = selected ? theme.RaisedSurface : theme.Surface;
            using SolidBrush background = new SolidBrush(tabColor);
            e.Graphics.FillRectangle(background, e.Bounds);
            TextRenderer.DrawText(
                e.Graphics,
                settingsTabs.TabPages[e.Index].Text,
                Font,
                e.Bounds,
                selected ? theme.Text : theme.MutedText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (selected)
            {
                using Pen accent = new Pen(theme.Accent, 3);
                e.Graphics.DrawLine(accent, e.Bounds.Left + 12, e.Bounds.Bottom - 2, e.Bounds.Right - 12, e.Bounds.Bottom - 2);
            }
        }

        private void ChoosePosition()
        {
            bool polish = config.Language == "pl";
            using PositionPickerForm picker = new PositionPickerForm(polish, crosshairForm.PositionX, crosshairForm.PositionY);
            if (picker.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            crosshairForm.PositionX = picker.PositionX;
            crosshairForm.PositionY = picker.PositionY;
            SetPositionSliderValues(picker.PositionX, picker.PositionY);
            crosshairForm.Redraw();

            if (creatingProfile)
            {
                pendingPositionX = picker.PositionX;
                pendingPositionY = picker.PositionY;
            }
            else
            {
                CrosshairPositionProfile profile = ConfigManager.GetSelectedPositionProfile(config);
                profile.X = picker.PositionX;
                profile.Y = picker.PositionY;
                ConfigManager.SaveConfig(config);
            }
        }

        private void UpdatePositionFromSliders()
        {
            if (refreshingTexts)
            {
                return;
            }

            crosshairForm.PositionX = tbPositionX.Value / (double)tbPositionX.Maximum;
            crosshairForm.PositionY = tbPositionY.Value / (double)tbPositionY.Maximum;
            crosshairForm.Redraw();

            if (creatingProfile)
            {
                pendingPositionX = crosshairForm.PositionX;
                pendingPositionY = crosshairForm.PositionY;
            }
            else
            {
                CrosshairPositionProfile profile = ConfigManager.GetSelectedPositionProfile(config);
                profile.X = crosshairForm.PositionX;
                profile.Y = crosshairForm.PositionY;
                ConfigManager.SaveConfig(config);
            }

            bool polish = config.Language == "pl";
            lblPositionX.Text = polish ? $"Poziomo: {tbPositionX.Value / 10}%" : $"Horizontal: {tbPositionX.Value / 10}%";
            lblPositionY.Text = polish ? $"Pionowo: {tbPositionY.Value / 10}%" : $"Vertical: {tbPositionY.Value / 10}%";
        }

        private void SetPositionSliderValues(double x, double y)
        {
            refreshingTexts = true;
            tbPositionX.Value = Math.Clamp((int)Math.Round(x * tbPositionX.Maximum), tbPositionX.Minimum, tbPositionX.Maximum);
            tbPositionY.Value = Math.Clamp((int)Math.Round(y * tbPositionY.Maximum), tbPositionY.Minimum, tbPositionY.Maximum);
            refreshingTexts = false;
            bool polish = config.Language == "pl";
            lblPositionX.Text = polish ? $"Poziomo: {tbPositionX.Value / 10}%" : $"Horizontal: {tbPositionX.Value / 10}%";
            lblPositionY.Text = polish ? $"Pionowo: {tbPositionY.Value / 10}%" : $"Vertical: {tbPositionY.Value / 10}%";
        }

        private void SavePositionProfile()
        {
            string name = txtProfileName.Text.Trim();
            if (name.Length == 0)
            {
                ShowProfileMessage("Wpisz nazwę profilu.", "Enter a profile name.");
                txtProfileName.Focus();
                return;
            }

            CrosshairPositionProfile? namedProfile = config.PositionProfiles.FirstOrDefault(item =>
                string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
            CrosshairPositionProfile? profile;

            if (creatingProfile)
            {
                if (namedProfile != null)
                {
                    ShowProfileMessage("Profil o tej nazwie już istnieje.", "A profile with that name already exists.");
                    return;
                }

                profile = new CrosshairPositionProfile { Name = name };
                config.PositionProfiles.Add(profile);
            }
            else
            {
                profile = ConfigManager.GetSelectedPositionProfile(config);
                if (namedProfile != null && !ReferenceEquals(namedProfile, profile))
                {
                    ShowProfileMessage("Profil o tej nazwie już istnieje.", "A profile with that name already exists.");
                    return;
                }
            }

            profile.Name = name;
            profile.X = pendingPositionX ?? crosshairForm.PositionX;
            profile.Y = pendingPositionY ?? crosshairForm.PositionY;
            config.SelectedPositionProfile = name;
            creatingProfile = false;
            pendingPositionX = null;
            pendingPositionY = null;
            ConfigManager.ApplyToForm(config, crosshairForm);
            ConfigManager.SaveConfig(config);
            RefreshLabels();
        }

        private void DeletePositionProfile()
        {
            if (config.PositionProfiles.Count <= 1)
            {
                return;
            }

            CrosshairPositionProfile profile = ConfigManager.GetSelectedPositionProfile(config);
            DialogResult result = MessageBox.Show(
                config.Language == "pl" ? $"Usunąć profil \"{profile.Name}\"?" : $"Delete profile \"{profile.Name}\"?",
                config.Language == "pl" ? "Usuń profil" : "Delete profile",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            config.PositionProfiles.Remove(profile);
            config.SelectedPositionProfile = config.PositionProfiles[0].Name;
            ConfigManager.ApplyToForm(config, crosshairForm);
            ConfigManager.SaveConfig(config);
            RefreshLabels();
        }

        private void ShowProfileMessage(string polishMessage, string englishMessage)
        {
            MessageBox.Show(
                config.Language == "pl" ? polishMessage : englishMessage,
                config.Language == "pl" ? "Profile pozycji" : "Position profiles",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static void UpdateColorButton(Button button, string colorHex)
        {
            Color color = ColorTranslator.FromHtml(colorHex);
            button.BackColor = color;
            button.ForeColor = color.GetBrightness() < 0.55f ? Color.White : Color.Black;
        }

        private void BtnColor_Click(object? sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = ColorTranslator.FromHtml(config.ColorHex);
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    config.ColorHex = ColorTranslator.ToHtml(cd.Color);
                    RefreshLabels();
                    ApplySettings();
                }
            }
        }

        private void BtnOutlineColor_Click(object? sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = ColorTranslator.FromHtml(config.OutlineColorHex);
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    config.OutlineColorHex = ColorTranslator.ToHtml(cd.Color);
                    RefreshLabels();
                    ApplySettings();
                }
            }
        }

        private void ApplySettings()
        {
            ConfigManager.ApplyToForm(config, crosshairForm);
            ConfigManager.SaveConfig(config);
        }
    }
}