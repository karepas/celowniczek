using System;
using System.Drawing;
using System.Windows.Forms;

namespace Celowniczek
{
    public class PositionPickerForm : Form
    {
        private readonly bool polish;
        private Point previewPoint;

        public double PositionX { get; private set; }
        public double PositionY { get; private set; }

        public PositionPickerForm(bool polish, double initialX, double initialY)
        {
            this.polish = polish;
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            Bounds = screen.Bounds;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            KeyPreview = true;
            BackColor = Color.Black;
            Opacity = 0.35;
            Cursor = Cursors.Cross;

            PositionX = Math.Clamp(initialX, 0, 1);
            PositionY = Math.Clamp(initialY, 0, 1);
            previewPoint = new Point(
                (int)Math.Round((Width - 1) * PositionX),
                (int)Math.Round((Height - 1) * PositionY));

            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Activate();
            Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            previewPoint = e.Location;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            PositionX = Math.Clamp(e.X / (double)Math.Max(1, Width - 1), 0, 1);
            PositionY = Math.Clamp(e.Y / (double)Math.Max(1, Height - 1), 0, 1);
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            string instruction = polish
                ? "Kliknij, aby ustawić celownik. Esc anuluje."
                : "Click to place the crosshair. Esc cancels.";

            using Font font = new Font("Segoe UI", 16, FontStyle.Bold);
            SizeF textSize = graphics.MeasureString(instruction, font);
            RectangleF banner = new RectangleF(
                (Width - textSize.Width - 48) / 2,
                24,
                textSize.Width + 48,
                textSize.Height + 28);
            using SolidBrush background = new SolidBrush(Color.FromArgb(220, 18, 22, 28));
            using SolidBrush foreground = new SolidBrush(Color.White);
            graphics.FillRectangle(background, banner);
            graphics.DrawString(instruction, font, foreground, banner.X + 24, banner.Y + 14);

            int x = previewPoint.X;
            int y = previewPoint.Y;
            using Pen outline = new Pen(Color.Black, 6);
            using Pen crosshair = new Pen(Color.Red, 3);
            graphics.DrawLine(outline, x - 15, y, x - 5, y);
            graphics.DrawLine(outline, x + 5, y, x + 15, y);
            graphics.DrawLine(outline, x, y - 15, x, y - 5);
            graphics.DrawLine(outline, x, y + 5, x, y + 15);
            graphics.DrawLine(crosshair, x - 15, y, x - 5, y);
            graphics.DrawLine(crosshair, x + 5, y, x + 15, y);
            graphics.DrawLine(crosshair, x, y - 15, x, y - 5);
            graphics.DrawLine(crosshair, x, y + 5, x, y + 15);
        }
    }
}
