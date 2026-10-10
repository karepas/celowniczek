using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Celowniczek
{
    public static class IconGenerator
    {
        public static void GenerateAppIcon(string outputPath)
        {
            using (Bitmap bmp = new Bitmap(256, 256))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(230, 20, 20, 25)))
                {
                    g.FillEllipse(bgBrush, 10, 10, 236, 236);
                }

                using (Pen ringPen = new Pen(Color.FromArgb(255, 0, 255, 200), 12))
                {
                    g.DrawEllipse(ringPen, 35, 35, 186, 186);
                }

                using (Pen crossPen = new Pen(Color.Cyan, 16))
                {
                    g.DrawLine(crossPen, 60, 128, 100, 128);
                    g.DrawLine(crossPen, 156, 128, 196, 128);
                    g.DrawLine(crossPen, 128, 60, 128, 100);
                    g.DrawLine(crossPen, 128, 156, 128, 196);
                }

                using (SolidBrush dotBrush = new SolidBrush(Color.Cyan))
                {
                    g.FillEllipse(dotBrush, 116, 116, 24, 24);
                }

                using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                {
                    SaveAsIcon(bmp, fs);
                }
            }
        }

        private static void SaveAsIcon(Bitmap sourceBitmap, Stream outputStream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                sourceBitmap.Save(ms, ImageFormat.Png);
                byte[] pngBytes = ms.ToArray();

                using (BinaryWriter iconWriter = new BinaryWriter(outputStream))
                {
                    iconWriter.Write((short)0);
                    iconWriter.Write((short)1);
                    iconWriter.Write((short)1);

                    iconWriter.Write((byte)0);
                    iconWriter.Write((byte)0);
                    iconWriter.Write((byte)0);
                    iconWriter.Write((byte)0);
                    iconWriter.Write((short)1);
                    iconWriter.Write((short)32);
                    iconWriter.Write(pngBytes.Length);
                    iconWriter.Write(22);

                    iconWriter.Write(pngBytes);
                }
            }
        }
    }
}