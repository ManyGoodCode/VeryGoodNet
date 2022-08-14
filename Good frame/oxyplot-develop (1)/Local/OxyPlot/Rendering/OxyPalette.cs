namespace OxyPlot
{
    using System.Collections.Generic;
    using System.Linq;

    public class OxyPalette
    {
        public OxyPalette()
        {
            this.Colors = new List<OxyColor>();
        }

        public OxyPalette(params OxyColor[] colors)
        {
            this.Colors = new List<OxyColor>(colors);
        }

        public OxyPalette(IEnumerable<OxyColor> colors)
        {
            this.Colors = new List<OxyColor>(colors);
        }

        public IList<OxyColor> Colors { get; set; }

        public static OxyPalette Interpolate(int paletteSize, params OxyColor[] colors)
        {
            if (colors == null || colors.Length == 0 || paletteSize < 1)
            {
                return new OxyPalette(new OxyColor[0]);
            }

            var palette = new OxyColor[paletteSize];

            double incrementStepSize = (paletteSize == 1) ? 0 : (1.0d / (paletteSize - 1));

            for (int i = 0; i < paletteSize; i++)
            {
                double y = i * incrementStepSize;
                double x = y * (colors.Length - 1);
                int i0 = (int)x;
                int i1 = i0 + 1 < colors.Length ? i0 + 1 : i0;
                palette[i] = OxyColor.Interpolate(colors[i0], colors[i1], x - i0);
            }

            return new OxyPalette(palette);
        }

        public OxyPalette Reverse()
        {
            return new OxyPalette(this.Colors.Reverse());
        }
    }
}