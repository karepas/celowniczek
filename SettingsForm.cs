using System;
using System.Drawing;
using System.Windows.Forms;

namespace Celowniczek
{
    public class SettingsForm : Form
    {
        private CrosshairForm _crosshair;

        private ComboBox _comboStyle = null!;
        private Button _btnColor = null!;
        private Button _btnOutlineColor = null!;
        private CheckBox _chkOutline = null!;
        private NumericUpDown _numSize = null!;
        private NumericUpDown _numThickness = null!;
        private NumericUpDown _numGap = null!;
        private NumericUpDown _numDotRadius = null!;

        public SettingsForm(CrosshairForm crosshair)
        {
            _crosshair = crosshair;
            this.Icon = IconGenerator.CreateCrosshairIcon();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Celowniczek - Ustawienia";
            this.Size = new Size(350, 360);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            int top = 20;

            Label lblStyle = new Label() { Text = "Styl:", Left = 20, Top = top, Width = 100 };
            _comboStyle = new ComboBox() { Left = 130, Top = top - 3, Width = 170, DropDownStyle = ComboBoxStyle.DropDownList };
            _comboStyle.Items.AddRange(Enum.GetNames(typeof(CrosshairForm.CrosshairStyle)));
            _comboStyle.SelectedItem = _crosshair.Style.ToString();
            _comboStyle.SelectedIndexChanged += (s, e) =>
            {
                if (_comboStyle.SelectedItem != null && Enum.TryParse(_comboStyle.SelectedItem.ToString(), out CrosshairForm.CrosshairStyle newStyle))
                {
                    _crosshair.Style = newStyle;
                    _crosshair.Invalidate();
                    ConfigManager.SaveConfig(_crosshair);
                }
            };
            this.Controls.Add(lblStyle);
            this.Controls.Add(_comboStyle);

            top += 35;
            Label lblColor = new Label() { Text = "Kolor:", Left = 20, Top = top, Width = 100 };
            _btnColor = new Button() { Left = 130, Top = top - 3, Width = 170, BackColor = _crosshair.CrosshairColor, Text = "Wybierz kolor" };
            _btnColor.Click += (s, e) =>
            {
                using (ColorDialog cd = new ColorDialog())
                {
                    cd.Color = _crosshair.CrosshairColor;
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        _crosshair.CrosshairColor = cd.Color;
                        _btnColor.BackColor = cd.Color;
                        _crosshair.Invalidate();
                        ConfigManager.SaveConfig(_crosshair);
                    }
                }
            };
            this.Controls.Add(lblColor);
            this.Controls.Add(_btnColor);

            top += 35;
            _chkOutline = new CheckBox() { Text = "Włącz obrys", Left = 20, Top = top, Checked = _crosshair.EnableOutline };
            _chkOutline.CheckedChanged += (s, e) =>
            {
                _crosshair.EnableOutline = _chkOutline.Checked;
                _crosshair.Invalidate();
                ConfigManager.SaveConfig(_crosshair);
            };
            this.Controls.Add(_chkOutline);

            top += 30;
            Label lblOutlineColor = new Label() { Text = "Kolor obrysu:", Left = 20, Top = top, Width = 100 };
            _btnOutlineColor = new Button() { Left = 130, Top = top - 3, Width = 170, BackColor = _crosshair.OutlineColor, Text = "Wybierz obrys" };
            _btnOutlineColor.Click += (s, e) =>
            {
                using (ColorDialog cd = new ColorDialog())
                {
                    cd.Color = _crosshair.OutlineColor;
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        _crosshair.OutlineColor = cd.Color;
                        _btnOutlineColor.BackColor = cd.Color;
                        _crosshair.Invalidate();
                        ConfigManager.SaveConfig(_crosshair);
                    }
                }
            };
            this.Controls.Add(lblOutlineColor);
            this.Controls.Add(_btnOutlineColor);

            top += 35;
            Label lblSize = new Label() { Text = "Rozmiar:", Left = 20, Top = top, Width = 100 };
            _numSize = new NumericUpDown() { Left = 130, Top = top - 3, Width = 170, Minimum = 2, Maximum = 100, Value = _crosshair.SizePx };
            _numSize.ValueChanged += (s, e) =>
            {
                _crosshair.SizePx = (int)_numSize.Value;
                _crosshair.UpdateBoundsToCenter();
                ConfigManager.SaveConfig(_crosshair);
            };
            this.Controls.Add(lblSize);
            this.Controls.Add(_numSize);

            top += 30;
            Label lblThickness = new Label() { Text = "Grubość:", Left = 20, Top = top, Width = 100 };
            _numThickness = new NumericUpDown() { Left = 130, Top = top - 3, Width = 170, Minimum = 1, Maximum = 20, Value = _crosshair.Thickness };
            _numThickness.ValueChanged += (s, e) =>
            {
                _crosshair.Thickness = (int)_numThickness.Value;
                _crosshair.Invalidate();
                ConfigManager.SaveConfig(_crosshair);
            };
            this.Controls.Add(lblThickness);
            this.Controls.Add(_numThickness);

            top += 30;
            Label lblGap = new Label() { Text = "Przerwa (Gap):", Left = 20, Top = top, Width = 100 };
            _numGap = new NumericUpDown() { Left = 130, Top = top - 3, Width = 170, Minimum = 0, Maximum = 50, Value = _crosshair.Gap };
            _numGap.ValueChanged += (s, e) =>
            {
                _crosshair.Gap = (int)_numGap.Value;
                _crosshair.Invalidate();
                ConfigManager.SaveConfig(_crosshair);
            };
            this.Controls.Add(lblGap);
            this.Controls.Add(_numGap);

            top += 30;
            Label lblDotRadius = new Label() { Text = "Kropka środek:", Left = 20, Top = top, Width = 100 };
            _numDotRadius = new NumericUpDown() { Left = 130, Top = top - 3, Width = 170, Minimum = 1, Maximum = 20, Value = _crosshair.DotRadius };
            _numDotRadius.ValueChanged += (s, e) =>
            {
                _crosshair.DotRadius = (int)_numDotRadius.Value;
                _crosshair.Invalidate();
                ConfigManager.SaveConfig(_crosshair);
            };
            this.Controls.Add(lblDotRadius);
            this.Controls.Add(_numDotRadius);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            base.OnFormClosing(e);
        }
    }
}