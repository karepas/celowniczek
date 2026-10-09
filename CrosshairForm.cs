using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Celowniczek
{
    public class CrosshairForm : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TOOLWINDOW = 0x80;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0001;
        private const uint SWP_NOSIZE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;

        public enum CrosshairStyle { Dot, Cross, Circle, CrossAndDot }

        public CrosshairStyle Style { get; set; } = CrosshairStyle.CrossAndDot;
        public Color CrosshairColor { get; set; } = Color.Cyan;
        public Color OutlineColor { get; set; } = Color.Black;
        public bool EnableOutline { get; set; } = true;
        public int SizePx { get; set; } = 12;
        public int Thickness { get; set; } = 2;
        public int Gap { get; set; } = 4;
        public int DotRadius { get; set; } = 2;

        public CrosshairForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            UpdateBoundsToCenter();
        }

        public void UpdateBoundsToCenter()
        {
            Rectangle screen = Screen.PrimaryScreen?.Bounds ?? Screen.AllScreens[0].Bounds;
            
            // Wymiar okna musi być parzysty, aby środek wypadał idealnie na przecieciu pikseli
            int boxSize = Math.Max(SizePx * 4, 120);
            if (boxSize % 2 != 0) boxSize++;

            int x = screen.Left + (screen.Width - boxSize) / 2;
            int y = screen.Top + (screen.Height - boxSize) / 2;

            this.Bounds = new Rectangle(x, y, boxSize, boxSize);
            this.Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            int initialStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, initialStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW);

            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Wymuszenie precyzyjnego pozycjonowania linii
            g.PixelOffsetMode = PixelOffsetMode.Half;

            float centerX = this.Width / 2.0f;
            float centerY = this.Height / 2.0f;

            using (Pen mainPen = new Pen(CrosshairColor, Thickness))
            using (Pen outlinePen = new Pen(OutlineColor, Thickness + 2))
            using (SolidBrush mainBrush = new SolidBrush(CrosshairColor))
            using (SolidBrush outlineBrush = new SolidBrush(OutlineColor))
            {
                mainPen.StartCap = LineCap.Flat;
                mainPen.EndCap = LineCap.Flat;
                outlinePen.StartCap = LineCap.Flat;
                outlinePen.EndCap = LineCap.Flat;

                // Kropka
                if (Style == CrosshairStyle.Dot || Style == CrosshairStyle.CrossAndDot)
                {
                    if (EnableOutline)
                    {
                        g.FillEllipse(outlineBrush, centerX - DotRadius - 1, centerY - DotRadius - 1, (DotRadius + 1) * 2, (DotRadius + 1) * 2);
                    }
                    g.FillEllipse(mainBrush, centerX - DotRadius, centerY - DotRadius, DotRadius * 2, DotRadius * 2);
                }

                // Krzyżyk
                if (Style == CrosshairStyle.Cross || Style == CrosshairStyle.CrossAndDot)
                {
                    if (EnableOutline)
                    {
                        g.DrawLine(outlinePen, centerX, centerY - Gap - SizePx, centerX, centerY - Gap);
                        g.DrawLine(outlinePen, centerX, centerY + Gap, centerX, centerY + Gap + SizePx);
                        g.DrawLine(outlinePen, centerX - Gap - SizePx, centerY, centerX - Gap, centerY);
                        g.DrawLine(outlinePen, centerX + Gap, centerY, centerX + Gap + SizePx, centerY);
                    }

                    g.DrawLine(mainPen, centerX, centerY - Gap - SizePx, centerX, centerY - Gap);
                    g.DrawLine(mainPen, centerX, centerY + Gap, centerX, centerY + Gap + SizePx);
                    g.DrawLine(mainPen, centerX - Gap - SizePx, centerY, centerX - Gap, centerY);
                    g.DrawLine(mainPen, centerX + Gap, centerY, centerX + Gap + SizePx, centerY);
                }

                // Okrąg
                if (Style == CrosshairStyle.Circle)
                {
                    float radius = SizePx;
                    if (EnableOutline)
                    {
                        g.DrawEllipse(outlinePen, centerX - radius, centerY - radius, radius * 2, radius * 2);
                    }
                    g.DrawEllipse(mainPen, centerX - radius, centerY - radius, radius * 2, radius * 2);
                }
            }
        }
    }
}