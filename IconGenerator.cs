using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
                g.SmoothingMode = SmoothingMode.AntiAlias;

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
            int[] sizes = { 16, 24, 32, 48, 64, 128, 256 };
            List<byte[]> images = new List<byte[]>(sizes.Length);

            foreach (int size in sizes)
            {
                using Bitmap frame = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(frame))
                {
                    graphics.Clear(Color.Transparent);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(sourceBitmap, new Rectangle(0, 0, size, size));
                }

                using MemoryStream imageStream = new MemoryStream();
                using (BinaryWriter imageWriter = new BinaryWriter(imageStream))
                {
                    int pixelDataSize = size * size * 4;
                    int maskRowSize = ((size + 31) / 32) * 4;

                    imageWriter.Write(40);
                    imageWriter.Write(size);
                    imageWriter.Write(size * 2);
                    imageWriter.Write((short)1);
                    imageWriter.Write((short)32);
                    imageWriter.Write(0);
                    imageWriter.Write(pixelDataSize);
                    imageWriter.Write(0);
                    imageWriter.Write(0);
                    imageWriter.Write(0);
                    imageWriter.Write(0);

                    for (int y = size - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            Color pixel = frame.GetPixel(x, y);
                            imageWriter.Write(pixel.B);
                            imageWriter.Write(pixel.G);
                            imageWriter.Write(pixel.R);
                            imageWriter.Write(pixel.A);
                        }
                    }

                    for (int y = size - 1; y >= 0; y--)
                    {
                        byte[] maskRow = new byte[maskRowSize];
                        for (int x = 0; x < size; x++)
                        {
                            if (frame.GetPixel(x, y).A == 0)
                            {
                                maskRow[x / 8] |= (byte)(0x80 >> (x % 8));
                            }
                        }
                        imageWriter.Write(maskRow);
                    }
                }

                images.Add(imageStream.ToArray());
            }

            using BinaryWriter iconWriter = new BinaryWriter(outputStream);
            iconWriter.Write((short)0);
            iconWriter.Write((short)1);
            iconWriter.Write((short)sizes.Length);

            int imageOffset = 6 + sizes.Length * 16;
            for (int index = 0; index < sizes.Length; index++)
            {
                int size = sizes[index];
                byte dimension = size == 256 ? (byte)0 : (byte)size;

                iconWriter.Write(dimension);
                iconWriter.Write(dimension);
                iconWriter.Write((byte)0);
                iconWriter.Write((byte)0);
                iconWriter.Write((short)1);
                iconWriter.Write((short)32);
                iconWriter.Write(images[index].Length);
                iconWriter.Write(imageOffset);
                imageOffset += images[index].Length;
            }

            foreach (byte[] image in images)
            {
                iconWriter.Write(image);
            }
        }
    }
}