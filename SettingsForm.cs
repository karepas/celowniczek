using System;
using System.Drawing;
using System.Windows.Forms;

namespace Celowniczek
{
    public class SettingsForm : Form
    {
        private readonly ConfigData config;
        private readonly CrosshairForm crosshairForm;

        private Button btnColor = null!;
        private TrackBar tbSize = null!;
        private TrackBar tbThickness = null!;
        private TrackBar tbGap = null!;
        private TrackBar tbDotRadius = null!;
        private CheckBox cbOutline = null!;
        private Label lblSize = null!;
        private Label lblThickness = null!;
        private Label lblGap = null!;
        private Label lblDotRadius = null!;

        public SettingsForm(ConfigData config, CrosshairForm crosshairForm)
        {
            this.config = config;
            this.crosshairForm = crosshairForm;

            this.Text = "Celowniczek - Ustawienia";
            this.Size = new Size(320, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            btnColor = new Button { Text = "Zmień kolor celownika", Location = new Point(20, 15), Size = new Size(260, 35) };
            btnColor.Click += BtnColor_Click;

            cbOutline = new CheckBox { Text = "Włącz czarny obrys (Outline)", Checked = config.EnableOutline, Location = new Point(20, 60), AutoSize = true };
            cbOutline.CheckedChanged += (s, e) => {
                config.EnableOutline = cbOutline.Checked;
                ApplySettings();
            };

            lblSize = new Label { Text = $"Rozmiar: {config.Size}", Location = new Point(20, 95), AutoSize = true };
            tbSize = new TrackBar { Minimum = 2, Maximum = 50, Value = config.Size, Location = new Point(20, 115), Size = new Size(260, 45) };
            tbSize.ValueChanged += (s, e) => {
                config.Size = tbSize.Value;
                lblSize.Text = $"Rozmiar: {config.Size}";
                ApplySettings();
            };

            lblThickness = new Label { Text = $"Grubość: {config.Thickness}", Location = new Point(20, 160), AutoSize = true };
            tbThickness = new TrackBar { Minimum = 1, Maximum = 10, Value = config.Thickness, Location = new Point(20, 180), Size = new Size(260, 45) };
            tbThickness.ValueChanged += (s, e) => {
                config.Thickness = tbThickness.Value;
                lblThickness.Text = $"Grubość: {config.Thickness}";
                ApplySettings();
            };

            lblGap = new Label { Text = $"Przerwa (Gap): {config.Gap}", Location = new Point(20, 225), AutoSize = true };
            tbGap = new TrackBar { Minimum = 0, Maximum = 30, Value = config.Gap, Location = new Point(20, 245), Size = new Size(260, 45) };
            tbGap.ValueChanged += (s, e) => {
                config.Gap = tbGap.Value;
                lblGap.Text = $"Przerwa (Gap): {config.Gap}";
                ApplySettings();
            };

            lblDotRadius = new Label { Text = $"Rozmiar kropki (0 = brak): {config.DotRadius}", Location = new Point(20, 290), AutoSize = true };
            tbDotRadius = new TrackBar { Minimum = 0, Maximum = 10, Value = config.DotRadius, Location = new Point(20, 310), Size = new Size(260, 45) };
            tbDotRadius.ValueChanged += (s, e) => {
                config.DotRadius = tbDotRadius.Value;
                lblDotRadius.Text = $"Rozmiar kropki (0 = brak): {config.DotRadius}";
                ApplySettings();
            };

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