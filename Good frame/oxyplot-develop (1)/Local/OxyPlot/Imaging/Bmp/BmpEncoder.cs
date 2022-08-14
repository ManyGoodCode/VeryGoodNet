namespace OxyPlot
{
    using System;
    using System.IO;

    public class BmpEncoder : IImageEncoder
    {
        private readonly BmpEncoderOptions options;
        public BmpEncoder(BmpEncoderOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// 将指定的图像数据编码为png格式
        /// </summary>
        /// <param name="pixels">像素数据(首先是底线)。</param>
        /// <returns>The png image data.</returns>
        public byte[] Encode(OxyColor[,] pixels)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            byte[] bytes = new byte[width * height * 4];
            int k = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bytes[k++] = pixels[x, y].B;
                    bytes[k++] = pixels[x, y].G;
                    bytes[k++] = pixels[x, y].R;
                    bytes[k++] = pixels[x, y].A;
                }
            }

            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            const int OffBits = 14 + 40;
            int size = OffBits + bytes.Length;

            // Bitmap file header (14 bytes)
            w.Write((byte)'B');
            w.Write((byte)'M');
            w.Write((uint)size);
            w.Write((ushort)0);
            w.Write((ushort)0);
            w.Write((uint)OffBits);

            // Bitmap info header (40 bytes)
            WriteBitmapInfoHeader(w, width, height, 32, bytes.Length, this.options.DpiX, this.options.DpiY);

            w.Write(bytes);
            return ms.ToArray();
        }

        public byte[] Encode(byte[,] pixels, OxyColor[] palette)
        {
            if (palette.Length == 0)
            {
                throw new ArgumentException("Palette not defined.", "palette");
            }

            if (palette.Length > 256)
            {
                throw new ArgumentException("Too many colors in the palette.", "palette");
            }

            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);
            int length = width * height;

            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            int offBits = 14 + 40 + (4 * palette.Length);
            int size = offBits + length;

            // Bitmap file header (14 bytes)
            w.Write((byte)'B');
            w.Write((byte)'M');
            w.Write((uint)size);
            w.Write((ushort)0);
            w.Write((ushort)0);
            w.Write((uint)offBits);

            // Bitmap info header
            WriteBitmapInfoHeader(w, width, height, 8, length, this.options.DpiX, this.options.DpiY, palette.Length);

            // Color table
            foreach (OxyColor color in palette)
            {
                w.Write(color.B);
                w.Write(color.G);
                w.Write(color.R);
                w.Write(color.A);
            }

            int rowSize = (int)Math.Floor((double)((8 * width) + 31) / 32) * 4;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    w.Write(pixels[x, y]);
                }

                // padding
                for (int j = width; j < rowSize; j++)
                {
                    w.Write((byte)0);
                }
            }

            return ms.ToArray();
        }

        private static void WriteBitmapInfoHeader(BinaryWriter w, int width, int height, int bitsPerPixel, int length, double dpix, double dpiy, int colors = 0)
        {
            w.Write((uint)40);
            w.Write((uint)width);
            w.Write((uint)height);
            w.Write((ushort)1);
            w.Write((ushort)bitsPerPixel);
            w.Write((uint)0);
            w.Write((uint)length);

            // Convert resolutions to pixels per meter
            w.Write((uint)(dpix / 0.0254));
            w.Write((uint)(dpiy / 0.0254));

            w.Write((uint)colors);
            w.Write((uint)colors);
        }

        private static void WriteBitmapV4Header(BinaryWriter w, int width, int height, int bitsPerPixel, int length, int dpi, int colors = 0)
        {
            // Convert resolution to pixels per meter
            var ppm = (uint)(dpi / 0.0254);

            w.Write((uint)108);
            w.Write((uint)width);
            w.Write((uint)height);
            w.Write((ushort)1);
            w.Write((ushort)bitsPerPixel);
            w.Write((uint)3);
            w.Write((uint)length);
            w.Write(ppm);
            w.Write(ppm);
            w.Write((uint)colors);
            w.Write((uint)colors);

            // Write the channel bit masks
            w.Write(0x00FF0000);
            w.Write(0x0000FF00);
            w.Write(0x000000FF);
            w.Write(0xFF000000);

            // Write the color space
            w.Write((uint)0x206E6957);
            w.Write(new byte[3 * 3 * 4]);

            // Write the gamma RGB
            w.Write((uint)0);
            w.Write((uint)0);
            w.Write((uint)0);
        }
    }
}