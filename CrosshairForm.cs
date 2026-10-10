using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Celowniczek
{
    public enum CrosshairStyle
    {
        Cross,
        DotOnly,
        Circle
    }

    public class CrosshairForm : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_TOPMOST = 0x00000008;

        public Color CrosshairColor { get; set; } = Color.Cyan;
        public Color OutlineColor { get; set; } = Color.Black;
        public bool EnableOutline { get; set; } = true;
        public int SizePx { get; set; } = 12;
        public int Thickness { get; set; } = 2;
        public int Gap { get; set; } = 4;
        public int DotRadius { get; set; } = 2;
        public CrosshairStyle Style { get; set; } = CrosshairStyle.Cross;

        public CrosshairForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;

            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.DoubleBuffered = true;

            UpdateBoundsToCenter();
        }

        public void UpdateBoundsToCenter()
        {
            Rectangle screen = Screen.PrimaryScreen?.Bounds ?? Screen.AllScreens[0].Bounds;
            this.Size = new Size(screen.Width, screen.Height);
            this.Location = new Point(0, 0);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int initialStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, initialStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_TOPMOST;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            int centerX = this.Width / 2;
            int centerY = this.Height / 2;

            if (Style == CrosshairStyle.Cross)
            {
                // Obrys (czarne tło pod celownikiem)
                if (EnableOutline)
                {
                    using (Pen outlinePen = new Pen(OutlineColor, Thickness + 2))
                    {
                        g.DrawLine(outlinePen, centerX, centerY - Gap - SizePx, centerX, centerY - Gap);
                        g.DrawLine(outlinePen, centerX, centerY + Gap, centerX, centerY + Gap + SizePx);
                        g.DrawLine(outlinePen, centerX - Gap - SizePx, centerY, centerX - Gap, centerY);
                        g.DrawLine(outlinePen, centerX + Gap, centerY, centerX + Gap + SizePx, centerY);
                    }
                }

                // Główne linie celownika
                using (Pen pen = new Pen(CrosshairColor, Thickness))
                {
                    g.DrawLine(pen, centerX, centerY - Gap - SizePx, centerX, centerY - Gap);
                    g.DrawLine(pen, centerX, centerY + Gap, centerX, centerY + Gap + SizePx);
                    g.DrawLine(pen, centerX - Gap - SizePx, centerY, centerX - Gap, centerY);
                    g.DrawLine(pen, centerX + Gap, centerY, centerX + Gap + SizePx, centerY);
                }
            }

            // Kropka na środku
            if (DotRadius > 0 || Style == CrosshairStyle.DotOnly)
            {
                int r = DotRadius > 0 ? DotRadius : 2;

                if (EnableOutline)
                {
                    using (SolidBrush outlineBrush = new SolidBrush(OutlineColor))
                    {
                        g.FillRectangle(outlineBrush, centerX - r - 1, centerY - r - 1, (r * 2) + 2, (r * 2) + 2);
                    }
                }

                using (SolidBrush brush = new SolidBrush(CrosshairColor))
                {
                    g.FillRectangle(brush, centerX - r, centerY - r, r * 2, r * 2);
                }
            }
        }

        public void Redraw()
        {
            this.Invalidate();
        }
    }
}