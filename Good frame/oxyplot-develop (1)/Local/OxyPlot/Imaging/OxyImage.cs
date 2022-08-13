namespace OxyPlot
{
    using System;
    using System.IO;

    /// <summary>
    /// 代表一个图像
    /// </summary>
    public class OxyImage
    {
        /// <summary>
        /// T图像数据
        /// </summary>
        private readonly byte[] data;

        /// <summary>
        /// 像素
        /// </summary>
        private OxyColor[,] pixels;

        public OxyImage(Stream s)
            : this(GetBytes(s))
        {
        }

        public OxyImage(byte[] bytes)
        {
            this.data = bytes;
            this.Format = GetImageFormat(bytes);
            this.UpdateImageInfo();
        }

        public ImageFormat Format { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        /// <summary>
        /// 获取每个像素的位数。
        /// </summary>
        public int BitsPerPixel { get; private set; }

        /// <summary>
        /// 获取图像的水平分辨率
        /// </summary>
        public double DpiX { get; private set; }

        public double DpiY { get; private set; }

        /// <summary>
        /// 从8位索引像素创建图像。
        /// </summary>
        public static OxyImage Create(
            byte[,] pixels,
            OxyColor[] palette,
            ImageFormat format,
            ImageEncoderOptions encoderOptions = null)
        {
            IImageEncoder encoder = GetEncoder(format, encoderOptions);
            return new OxyImage(encoder.Encode(pixels, palette));
        }


        public static OxyImage Create(
            OxyColor[,] pixels, 
            ImageFormat format, 
            ImageEncoderOptions encoderOptions = null)
        {
            IImageEncoder encoder = GetEncoder(format, encoderOptions);
            OxyImage image = new OxyImage(encoder.Encode(pixels));

            // TODO: remove when PNG decoder is implemented
            image.pixels = pixels;
            return image;
        }

        public byte[] GetData()
        {
            return this.data;
        }

        public OxyColor[,] GetPixels()
        {
            if (this.pixels != null)
            {
                return this.pixels;
            }

            IImageDecoder decoder = GetDecoder(this.Format);
            return decoder.Decode(this.data);
        }

        private static IImageDecoder GetDecoder(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.Bmp:
                    return new BmpDecoder();

                case ImageFormat.Png:
                    return new PngDecoder();

                case ImageFormat.Jpeg:
                    throw new NotImplementedException();

                default:
                    throw new InvalidOperationException("Image format not supported");
            }
        }


        private static IImageEncoder GetEncoder(
            ImageFormat format, 
            ImageEncoderOptions encoderOptions)
        {
            switch (format)
            {
                case ImageFormat.Bmp:
                    if (encoderOptions == null)
                    {
                        encoderOptions = new BmpEncoderOptions();
                    }

                    if (!(encoderOptions is BmpEncoderOptions))
                    {
                        throw new ArgumentException("encoderOptions");
                    }

                    return new BmpEncoder((BmpEncoderOptions)encoderOptions);

                case ImageFormat.Png:
                    if (encoderOptions == null)
                    {
                        encoderOptions = new PngEncoderOptions();
                    }

                    if (!(encoderOptions is PngEncoderOptions))
                    {
                        throw new ArgumentException("encoderOptions");
                    }

                    return new PngEncoder((PngEncoderOptions)encoderOptions);

                case ImageFormat.Jpeg:

                    throw new NotImplementedException();

                default:
                    throw new InvalidOperationException("Image format not supported");
            }
        }


        private static ImageFormat GetImageFormat(byte[] bytes)
        {
            // Jpeg: 0xff 0xDB
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xD8)
            {
                return ImageFormat.Jpeg;
            }

            // BMP:0x42 0x4D
            if (bytes.Length >= 2 && bytes[0] == 0x42 && bytes[1] == 0x4D)
            {
                return ImageFormat.Bmp;
            }

            //Png:0x89 0x50 0x4E 0x47
            if (bytes.Length >= 4 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            {
                return ImageFormat.Png;
            }

            return ImageFormat.Unknown;
        }

        private static byte[] GetBytes(Stream s)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 更新图像信息
        /// </summary>
        private void UpdateImageInfo()
        {
            IImageDecoder decoder = GetDecoder(this.Format);
            OxyImageInfo info = decoder.GetImageInfo(this.data);
            if (info != null)
            {
                this.Width = info.Width;
                this.Height = info.Height;
                this.BitsPerPixel = info.BitsPerPixel;
                this.DpiX = info.DpiX;
                this.DpiY = info.DpiY;
            }
        }
    }
}