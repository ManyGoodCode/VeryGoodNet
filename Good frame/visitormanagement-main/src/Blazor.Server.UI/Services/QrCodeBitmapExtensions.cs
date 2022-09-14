using Net.Codecrete.QrCodeGenerator;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;


namespace Blazor.Server.UI.Services
{
    public static class QrCodeBitmapExtensions
    {
        public static Image ToBitmap(this QrCode qrCode, int scale, int border, Color foreground, Color background)
        {
            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Value out of range");
            }
            if (border < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(border), "Value out of range");
            }

            int size = qrCode.Size;
            int dim = (size + border * 2) * scale;
            if (dim > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Scale or border too large");
            }

            Image<Rgb24> image = new Image<Rgb24>(dim, dim);
            image.Mutate(img =>
            {
                img.Fill(background);
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        if (qrCode.GetModule(x, y))
                        {
                            img.Fill(foreground, new Rectangle((x + border) * scale, (y + border) * scale, scale, scale));
                        }
                    }
                }
            });

            return image;
        }

        public static Image ToBitmap(this QrCode qrCode, int scale, int border)
        {
            return qrCode.ToBitmap(scale, border, Color.Black, Color.White);
        }

        public static byte[] ToPng(this QrCode qrCode, int scale, int border, Color foreground, Color background)
        {
            using Image image = qrCode.ToBitmap(scale, border, foreground, background);
            using MemoryStream ms = new MemoryStream();
            image.SaveAsPng(ms);
            return ms.ToArray();
        }

        public static byte[] ToPng(this QrCode qrCode, int scale, int border)
        {
            return qrCode.ToPng(scale, border, Color.Black, Color.White);
        }

        public static void SaveAsPng(this QrCode qrCode, string filename, int scale, int border, Color foreground, Color background)
        {
            using Image image = qrCode.ToBitmap(scale, border, foreground, background);
            image.SaveAsPng(filename);
        }

        public static void SaveAsPng(this QrCode qrCode, string filename, int scale, int border)
        {
            qrCode.SaveAsPng(filename, scale, border, Color.Black, Color.White);
        }
    }
}
