namespace OxyPlot
{
    using System;
    using System.IO;

    public class BmpDecoder : IImageDecoder
    {
        public OxyImageInfo GetImageInfo(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader r = new BinaryReader(ms);

            // bitmap header 文件头 
            r.ReadBytes(count: 2); // headerField
            r.ReadUInt32();        // size
            r.ReadBytes(count: 4); // reserved
            r.ReadUInt32();        // imageDataOffset

            // BitMap Info Header 
            r.ReadUInt32();         // headerSize
            int width = r.ReadInt32();
            int height = r.ReadInt32();
            r.ReadInt16();          // colorPlane
            short bitsPerPixel = r.ReadInt16();
            r.ReadInt32();          // compressionMethod
            r.ReadInt32();          // imageSize
            int horizontalResolution = r.ReadInt32();
            int verticalResolution = r.ReadInt32();
            r.ReadInt32();          // numberOfColors
            r.ReadInt32();          // importantColors

            return new OxyImageInfo
            {
                Width = width,
                Height = height,
                DpiX = horizontalResolution * 0.0254,
                DpiY = verticalResolution * 0.0254,
                BitsPerPixel = bitsPerPixel
            };
        }

        public OxyColor[,] Decode(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}