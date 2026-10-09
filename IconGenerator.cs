using System.Drawing;
using System.Drawing.Drawing2D;

namespace Celowniczek
{
    public static class IconGenerator
    {
        public static Icon CreateCrosshairIcon()
        {
            using (Bitmap bmp = new Bitmap(32, 32))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(Color.Cyan, 3))
                using (Pen outlinePen = new Pen(Color.Black, 5))
                {
                    float center = 16f;

                    // Obrys
                    g.DrawLine(outlinePen, center, 4, center, 28);
                    g.DrawLine(outlinePen, 4, center, 28, center);

                    // Wnętrze
                    g.DrawLine(pen, center, 4, center, 28);
                    g.DrawLine(pen, 4, center, 28, center);

                    // Środkowa kropka
                    g.FillEllipse(Brushes.Black, center - 4, center - 4, 8, 8);
                    g.FillEllipse(Brushes.Red, center - 2, center - 2, 4, 4);
                }

                IntPtr hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }
    }
}