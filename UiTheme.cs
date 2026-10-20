using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Celowniczek
{
    public sealed class UiThemePalette
    {
        public string Id { get; }
        public string EnglishName { get; }
        public string PolishName { get; }
        public Color Background { get; }
        public Color Surface { get; }
        public Color RaisedSurface { get; }
        public Color InputBackground { get; }
        public Color Text { get; }
        public Color MutedText { get; }
        public Color Accent { get; }
        public Color AccentHover { get; }
        public Color Border { get; }
        public Color SecondaryButton { get; }
        public Color SelectedText { get; }
        public bool LiquidBackground { get; }

        public UiThemePalette(
            string id,
            string englishName,
            string polishName,
            Color background,
            Color surface,
            Color raisedSurface,
            Color inputBackground,
            Color text,
            Color mutedText,
            Color accent,
            Color accentHover,
            Color border,
            Color secondaryButton,
            Color selectedText,
            bool liquidBackground)
        {
            Id = id;
            EnglishName = englishName;
            PolishName = polishName;
            Background = background;
            Surface = surface;
            RaisedSurface = raisedSurface;
            InputBackground = inputBackground;
            Text = text;
            MutedText = mutedText;
            Accent = accent;
            AccentHover = accentHover;
            Border = border;
            SecondaryButton = secondaryButton;
            SelectedText = selectedText;
            LiquidBackground = liquidBackground;
        }
    }

    public static class UiThemeManager
    {
        public const string DefaultThemeId = "liquid-blue";

        private static readonly UiThemePalette[] Themes =
        {
            new UiThemePalette(
                "liquid-blue", "Liquid Blue", "Liquid Blue",
                Color.FromArgb(6, 16, 31), Color.FromArgb(10, 27, 46), Color.FromArgb(17, 41, 65),
                Color.FromArgb(8, 23, 40), Color.FromArgb(230, 245, 255), Color.FromArgb(154, 183, 205),
                Color.FromArgb(79, 199, 239), Color.FromArgb(115, 218, 250), Color.FromArgb(39, 75, 101),
                Color.FromArgb(21, 62, 91), Color.White, true),
            new UiThemePalette(
                "aurora", "Aurora", "Aurora",
                Color.FromArgb(17, 22, 35), Color.FromArgb(25, 33, 48), Color.FromArgb(35, 47, 63),
                Color.FromArgb(20, 29, 43), Color.FromArgb(236, 248, 246), Color.FromArgb(166, 190, 190),
                Color.FromArgb(116, 220, 196), Color.FromArgb(148, 235, 216), Color.FromArgb(55, 81, 87),
                Color.FromArgb(31, 75, 76), Color.White, true),
            new UiThemePalette(
                "graphite", "Graphite", "Grafit",
                Color.FromArgb(23, 26, 31), Color.FromArgb(32, 37, 44), Color.FromArgb(45, 52, 61),
                Color.FromArgb(25, 30, 36), Color.FromArgb(239, 243, 247), Color.FromArgb(171, 181, 192),
                Color.FromArgb(116, 195, 231), Color.FromArgb(145, 211, 241), Color.FromArgb(64, 74, 84),
                Color.FromArgb(48, 64, 77), Color.White, false),
            new UiThemePalette(
                "arctic", "Arctic", "Arktyczny",
                Color.FromArgb(226, 237, 246), Color.FromArgb(240, 247, 252), Color.White,
                Color.White, Color.FromArgb(27, 49, 68), Color.FromArgb(88, 111, 132),
                Color.FromArgb(24, 119, 178), Color.FromArgb(16, 99, 153), Color.FromArgb(181, 202, 219),
                Color.FromArgb(211, 228, 240), Color.White, false)
        };

        public static IReadOnlyList<UiThemePalette> AvailableThemes => Themes;

        public static UiThemePalette GetTheme(string? themeId)
        {
            foreach (UiThemePalette theme in Themes)
            {
                if (string.Equals(theme.Id, themeId, StringComparison.OrdinalIgnoreCase))
                {
                    return theme;
                }
            }

            return Themes[0];
        }

        public static string GetDisplayName(UiThemePalette theme, bool polish)
        {
            return polish ? theme.PolishName : theme.EnglishName;
        }

        public static void Apply(Control root, UiThemePalette theme)
        {
            ApplyControl(root, theme);
        }

        private static void ApplyControl(Control control, UiThemePalette theme)
        {
            if (control is ThemePanel themePanel)
            {
                themePanel.Palette = theme;
            }
            else if (control is Form)
            {
                control.BackColor = theme.Background;
                control.ForeColor = theme.Text;
                control.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
            else if (control is TabControl tabControl)
            {
                tabControl.BackColor = theme.Surface;
                tabControl.ForeColor = theme.Text;
                tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
                tabControl.ItemSize = new Size(128, 34);
                tabControl.SizeMode = TabSizeMode.Fixed;
            }
            else if (control is TabPage)
            {
                control.BackColor = theme.Surface;
                control.ForeColor = theme.Text;
            }
            else if (control is Label label)
            {
                bool surfaceLabel = string.Equals(label.Tag as string, "surface", StringComparison.Ordinal);
                label.BackColor = surfaceLabel ? theme.RaisedSurface : Color.Transparent;
                label.ForeColor = theme.Text;
                if (label.Font.Size >= 14)
                {
                    label.Font = new Font("Segoe UI", label.Font.Size, FontStyle.Bold);
                }
            }
            else if (control is Button button)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = theme.Border;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.MouseOverBackColor = theme.AccentHover;
                string buttonRole = button.Tag as string ?? "";
                if (buttonRole == "swatch")
                {
                    button.FlatAppearance.BorderSize = 1;
                }
                else if (buttonRole == "primary")
                {
                    button.BackColor = theme.Accent;
                    button.ForeColor = theme.SelectedText;
                }
                else if (buttonRole == "secondary")
                {
                    button.BackColor = theme.SecondaryButton;
                    button.ForeColor = theme.Text;
                }
                else
                {
                    button.BackColor = theme.RaisedSurface;
                    button.ForeColor = theme.Text;
                }
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.BackColor = theme.InputBackground;
                comboBox.ForeColor = theme.Text;
            }
            else if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.BackColor = theme.InputBackground;
                textBox.ForeColor = theme.Text;
            }
            else if (control is ListBox listBox)
            {
                listBox.BackColor = theme.InputBackground;
                listBox.ForeColor = theme.Text;
                listBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.BackColor = Color.Transparent;
                checkBox.ForeColor = theme.Text;
            }
            else if (control is TrackBar trackBar)
            {
                trackBar.BackColor = theme.Surface;
                trackBar.ForeColor = theme.Accent;
            }
            else if (control is TableLayoutPanel)
            {
                control.BackColor = Color.Transparent;
                control.ForeColor = theme.Text;
            }
            else if (control is Panel)
            {
                control.BackColor = theme.Surface;
                control.ForeColor = theme.Text;
            }
            else if (control is UserControl)
            {
                control.BackColor = theme.Background;
                control.ForeColor = theme.Text;
            }
            else
            {
                control.ForeColor = theme.Text;
            }

            foreach (Control child in control.Controls)
            {
                ApplyControl(child, theme);
            }
        }
    }

    public class ThemePanel : Panel
    {
        private UiThemePalette palette = UiThemeManager.GetTheme(UiThemeManager.DefaultThemeId);

        public UiThemePalette Palette
        {
            get => palette;
            set
            {
                palette = value;
                BackColor = palette.Background;
                Invalidate();
            }
        }

        public ThemePanel()
        {
            DoubleBuffered = true;
            BackColor = palette.Background;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            using LinearGradientBrush background = new LinearGradientBrush(
                ClientRectangle,
                palette.Background,
                palette.Surface,
                LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(background, ClientRectangle);

            if (!palette.LiquidBackground || AutoScroll)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int waveBase = ClientSize.Height - 18;
            using GraphicsPath wavePath = new GraphicsPath();
            wavePath.AddBezier(
                new Point(-ClientSize.Width / 5, waveBase),
                new Point(ClientSize.Width / 4, waveBase - 18),
                new Point(ClientSize.Width * 2 / 3, waveBase + 8),
                new Point(ClientSize.Width + ClientSize.Width / 5, waveBase - 10));
            using Pen softWave = new Pen(Color.FromArgb(10, palette.Accent), 14);
            using Pen fineWave = new Pen(Color.FromArgb(18, palette.AccentHover), 2);
            e.Graphics.DrawPath(softWave, wavePath);
            e.Graphics.DrawPath(fineWave, wavePath);
        }
    }
}