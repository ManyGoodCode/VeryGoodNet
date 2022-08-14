namespace OxyPlot
{
    public static class PortableDocumentImageUtilities
    {
        public static PortableDocumentImage Convert(OxyImage image, bool interpolate)
        {
            OxyColor[,] pixels;
            try
            {
                pixels = image.GetPixels();
            }
            catch
            {
                return null;
            }

            byte[] bits = new byte[image.Width * image.Height * 3];
            byte[] maskBits = new byte[image.Width * image.Height];
            int i = 0;
            int j = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    maskBits[j++] = pixels[x, y].A;
                    bits[i++] = pixels[x, y].R;
                    bits[i++] = pixels[x, y].G;
                    bits[i++] = pixels[x, y].B;
                }
            }

            return new PortableDocumentImage(
                image.Width, 
                image.Height,
                8, bits, maskBits, interpolate);
        }
    }
}