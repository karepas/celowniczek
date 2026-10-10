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

        public SettingsForm(ConfigData config, CrosshairForm crosshairForm)
        {
            this.config = config;
            this.crosshairForm = crosshairForm;

            this.ClientSize = new Size(300, 500);
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
            lblLang = new Label { Location = new Point(20, 10), AutoSize = true };
            cbLanguage = new ComboBox { Location = new Point(20, 28), Size = new Size(260, 25), DropDownStyle = ComboBoxStyle.DropDownList };
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

            lblStyle = new Label { Location = new Point(20, 62), AutoSize = true };
            cbStyle = new ComboBox { Location = new Point(20, 80), Size = new Size(260, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbStyle.SelectedIndexChanged += (s, e) =>
            {
                if (!refreshingTexts && cbStyle.SelectedIndex >= 0)
                {
                    config.Style = (CrosshairStyle)cbStyle.SelectedIndex;
                    ApplySettings();
                }
            };

            btnColor = new Button { Location = new Point(20, 116), Size = new Size(260, 32) };
            btnColor.Click += BtnColor_Click;

            cbOutline = new CheckBox { Checked = config.EnableOutline, Location = new Point(20, 154), AutoSize = true };
            cbOutline.CheckedChanged += (s, e) =>
            {
                config.EnableOutline = cbOutline.Checked;
                btnOutlineColor.Enabled = cbOutline.Checked;
                ApplySettings();
            };

            btnOutlineColor = new Button { Location = new Point(20, 178), Size = new Size(260, 32), Enabled = config.EnableOutline };
            btnOutlineColor.Click += BtnOutlineColor_Click;

            lblSize = new Label { Location = new Point(20, 216), AutoSize = true };
            tbSize = new TrackBar { Minimum = 2, Maximum = 50, Value = Math.Clamp(config.Size, 2, 50), Location = new Point(20, 233), Size = new Size(260, 45) };
            tbSize.ValueChanged += (s, e) =>
            {
                config.Size = tbSize.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblThickness = new Label { Location = new Point(20, 278), AutoSize = true };
            tbThickness = new TrackBar { Minimum = 1, Maximum = 10, Value = Math.Clamp(config.Thickness, 1, 10), Location = new Point(20, 295), Size = new Size(260, 45) };
            tbThickness.ValueChanged += (s, e) =>
            {
                config.Thickness = tbThickness.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblGap = new Label { Location = new Point(20, 340), AutoSize = true };
            tbGap = new TrackBar { Minimum = 0, Maximum = 30, Value = Math.Clamp(config.Gap, 0, 30), Location = new Point(20, 357), Size = new Size(260, 45) };
            tbGap.ValueChanged += (s, e) =>
            {
                config.Gap = tbGap.Value;
                RefreshLabels();
                ApplySettings();
            };

            lblDotRadius = new Label { Location = new Point(20, 402), AutoSize = true };
            tbDotRadius = new TrackBar { Minimum = 0, Maximum = 10, Value = Math.Clamp(config.DotRadius, 0, 10), Location = new Point(20, 419), Size = new Size(260, 45) };
            tbDotRadius.ValueChanged += (s, e) =>
            {
                config.DotRadius = tbDotRadius.Value;
                RefreshLabels();
                ApplySettings();
            };

            RefreshLabels();

            this.Controls.Add(lblLang);
            this.Controls.Add(cbLanguage);
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