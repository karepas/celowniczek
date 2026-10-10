using System;
using System.Drawing;
using System.Windows.Forms;

namespace Celowniczek
{
    public class SettingsForm : Form
    {
        private readonly ConfigData config;
        private readonly CrosshairForm crosshairForm;

        private ComboBox cbLanguage = null!;
        private Button btnColor = null!;
        private CheckBox cbOutline = null!;
        private TrackBar tbSize = null!;
        private TrackBar tbThickness = null!;
        private TrackBar tbGap = null!;
        private TrackBar tbDotRadius = null!;

        private Label lblLang = null!;
        private Label lblSize = null!;
        private Label lblThickness = null!;
        private Label lblGap = null!;
        private Label lblDotRadius = null!;

        public SettingsForm(ConfigData config, CrosshairForm crosshairForm)
        {
            this.config = config;
            this.crosshairForm = crosshairForm;

            this.Size = new Size(320, 470);
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
            lblLang = new Label { Location = new Point(20, 15), AutoSize = true };
            cbLanguage = new ComboBox { Location = new Point(20, 35), Size = new Size(260, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbLanguage.Items.Add("English (Default)");
            cbLanguage.Items.Add("Polski");
            cbLanguage.SelectedIndex = config.Language == "pl" ? 1 : 0;
            cbLanguage.SelectedIndexChanged += (s, e) => {
                config.Language = cbLanguage.SelectedIndex == 1 ? "pl" : "en";
                UpdateTexts();
                RefreshLabels();
                ConfigManager.SaveConfig(config);
            };

            btnColor = new Button { Location = new Point(20, 75), Size = new Size(260, 32) };
            btnColor.Click += BtnColor_Click;

            cbOutline = new CheckBox { Checked = config.EnableOutline, Location = new Point(20, 115), AutoSize = true };
            cbOutline.CheckedChanged += (s, e) => {
                config.EnableOutline = cbOutline.Checked;
                ApplySettings();
            };

            lblSize = new Label { Location = new Point(20, 150), AutoSize = true };
            tbSize = new TrackBar { Minimum = 2, Maximum = 50, Value = config.Size, Location = new Point(20, 170), Size = new Size(260, 45) };
            tbSize.ValueChanged += (s, e) => {
                config.Size = tbSize.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblThickness = new Label { Location = new Point(20, 215), AutoSize = true };
            tbThickness = new TrackBar { Minimum = 1, Maximum = 10, Value = config.Thickness, Location = new Point(20, 235), Size = new Size(260, 45) };
            tbThickness.ValueChanged += (s, e) => {
                config.Thickness = tbThickness.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblGap = new Label { Location = new Point(20, 280), AutoSize = true };
            tbGap = new TrackBar { Minimum = 0, Maximum = 30, Value = config.Gap, Location = new Point(20, 300), Size = new Size(260, 45) };
            tbGap.ValueChanged += (s, e) => {
                config.Gap = tbGap.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblDotRadius = new Label { Location = new Point(20, 345), AutoSize = true };
            tbDotRadius = new TrackBar { Minimum = 0, Maximum = 10, Value = config.DotRadius, Location = new Point(20, 365), Size = new Size(260, 45) };
            tbDotRadius.ValueChanged += (s, e) => {
                config.DotRadius = tbDotRadius.Value;
                RefreshLabels();
                ApplySettings();
            };

            RefreshLabels();

            this.Controls.Add(lblLang);
            this.Controls.Add(cbLanguage);
            this.Controls.Add(btnColor);
            this.Controls.Add(cbOutline);
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
            if (config.Language == "pl")
            {
                lblLang.Text = "Język / Language:";
                btnColor.Text = "Zmień kolor celownika";
                cbOutline.Text = "Włącz czarny obrys (Outline)";
                lblSize.Text = $"Rozmiar (Size): {config.Size}";
                lblThickness.Text = $"Grubość (Thickness): {config.Thickness}";
                lblGap.Text = $"Przerwa (Gap): {config.Gap}";
                lblDotRadius.Text = $"Rozmiar kropki (0 = brak): {config.DotRadius}";
            }
            else
            {
                lblLang.Text = "Language / Język:";
                btnColor.Text = "Change Crosshair Color";
                cbOutline.Text = "Enable Black Outline";
                lblSize.Text = $"Size: {config.Size}";
                lblThickness.Text = $"Thickness: {config.Thickness}";
                lblGap.Text = $"Gap: {config.Gap}";
                lblDotRadius.Text = $"Center Dot Size (0 = none): {config.DotRadius}";
            }
        }

        private void BtnColor_Click(object? sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = ColorTranslator.FromHtml(config.ColorHex);
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    config.ColorHex = ColorTranslator.ToHtml(cd.Color);
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