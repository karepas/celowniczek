using System;
using System.Drawing;
using System.Windows.Forms;

namespace Celowniczek
{
    public class SettingsForm : Form
    {
        private readonly ConfigData config;
        private readonly CrosshairForm crosshairForm;

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

            this.ClientSize = new Size(390, 680);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            UpdateTexts();
            InitializeComponents();
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

            CrosshairPositionProfile selectedPosition = ConfigManager.GetSelectedPositionProfile(config);
            lblPositionX = new Label { Location = new Point(20, 188), AutoSize = true };
            tbPositionX = new TrackBar { Minimum = 0, Maximum = 1000, TickFrequency = 50, Value = (int)Math.Round(selectedPosition.X * 1000), Location = new Point(20, 205), Size = new Size(350, 35) };
            tbPositionX.ValueChanged += (s, e) => UpdatePositionFromSliders();

            lblPositionY = new Label { Location = new Point(20, 241), AutoSize = true };
            tbPositionY = new TrackBar { Minimum = 0, Maximum = 1000, TickFrequency = 50, Value = (int)Math.Round(selectedPosition.Y * 1000), Location = new Point(20, 258), Size = new Size(350, 35) };
            tbPositionY.ValueChanged += (s, e) => UpdatePositionFromSliders();

            lblStyle = new Label { Location = new Point(20, 299), AutoSize = true };
            cbStyle = new ComboBox { Location = new Point(20, 316), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbStyle.SelectedIndexChanged += (s, e) =>
            {
                if (!refreshingTexts && cbStyle.SelectedIndex >= 0)
                {
                    config.Style = (CrosshairStyle)cbStyle.SelectedIndex;
                    ApplySettings();
                }
            };

            btnColor = new Button { Location = new Point(20, 347), Size = new Size(350, 32) };
            btnColor.Click += BtnColor_Click;

            cbOutline = new CheckBox { Checked = config.EnableOutline, Location = new Point(20, 385), AutoSize = true };
            cbOutline.CheckedChanged += (s, e) =>
            {
                config.EnableOutline = cbOutline.Checked;
                btnOutlineColor.Enabled = cbOutline.Checked;
                ApplySettings();
            };

            btnOutlineColor = new Button { Location = new Point(20, 409), Size = new Size(350, 26), Enabled = config.EnableOutline };
            btnOutlineColor.Click += BtnOutlineColor_Click;

            lblSize = new Label { Location = new Point(20, 439), AutoSize = true };
            tbSize = new TrackBar { Minimum = 2, Maximum = 50, Value = Math.Clamp(config.Size, 2, 50), Location = new Point(20, 456), Size = new Size(350, 38) };
            tbSize.ValueChanged += (s, e) =>
            {
                config.Size = tbSize.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblThickness = new Label { Location = new Point(20, 496), AutoSize = true };
            tbThickness = new TrackBar { Minimum = 1, Maximum = 10, Value = Math.Clamp(config.Thickness, 1, 10), Location = new Point(20, 513), Size = new Size(350, 38) };
            tbThickness.ValueChanged += (s, e) =>
            {
                config.Thickness = tbThickness.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblGap = new Label { Location = new Point(20, 553), AutoSize = true };
            tbGap = new TrackBar { Minimum = 0, Maximum = 30, Value = Math.Clamp(config.Gap, 0, 30), Location = new Point(20, 570), Size = new Size(350, 38) };
            tbGap.ValueChanged += (s, e) =>
            {
                config.Gap = tbGap.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblDotRadius = new Label { Location = new Point(20, 610), AutoSize = true };
            tbDotRadius = new TrackBar { Minimum = 0, Maximum = 10, Value = Math.Clamp(config.DotRadius, 0, 10), Location = new Point(20, 627), Size = new Size(350, 38) };
            tbDotRadius.ValueChanged += (s, e) =>
            {
                config.DotRadius = tbDotRadius.Value;
                RefreshLabels();
                ApplySettings();
            };

            RefreshLabels();

            this.Controls.Add(lblProfiles);
            this.Controls.Add(cbProfiles);
            this.Controls.Add(lblProfileName);
            this.Controls.Add(txtProfileName);
            this.Controls.Add(btnNewProfile);
            this.Controls.Add(btnSaveProfile);
            this.Controls.Add(btnChangePosition);
            this.Controls.Add(btnDeleteProfile);
            this.Controls.Add(lblLang);
            this.Controls.Add(cbLanguage);
            this.Controls.Add(lblPositionX);
            this.Controls.Add(tbPositionX);
            this.Controls.Add(lblPositionY);
            this.Controls.Add(tbPositionY);
            this.Controls.Add(lblStyle);
            this.Controls.Add(cbStyle);
            this.Controls.Add(btnColor);
            this.Controls.Add(cbOutline);
            this.Controls.Add(btnOutlineColor);
            this.Controls.Add(lblSize);
            this.Controls.Add(tbSize);
            this.Controls.Add(lblThickness);
            this.Controls.Add(tbThickness);
            this.Controls.Add(lblGap);
            this.Controls.Add(tbGap);
            this.Controls.Add(lblDotRadius);
            this.Controls.Add(tbDotRadius);
        }

        private void RefreshLabels()
        {
            bool polish = config.Language == "pl";
            ConfigManager.NormalizeProfiles(config);
            this.ClientSize = new Size(390, 680);
            lblProfiles.Text = polish ? "Profil pozycji:" : "Position profile:";
            lblProfileName.Text = polish ? "Nazwa profilu:" : "Profile name:";
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