namespace OxyPlot
{
    public static partial class OxyPalettes
    {
        static OxyPalettes()
        {
            BlueWhiteRed31 = BlueWhiteRed(31);
            Hot64 = Hot(64);
            Hue64 = Hue(64);
        }

        public static OxyPalette BlueWhiteRed31 { get; private set; }
        public static OxyPalette Hot64 { get; private set; }
        public static OxyPalette Hue64 { get; private set; }
        public static OxyPalette BlackWhiteRed(int numberOfColors)
        {
            return OxyPalette.Interpolate(numberOfColors, OxyColors.Black, OxyColors.White, OxyColors.Red);
        }

        public static OxyPalette BlueWhiteRed(int numberOfColors)
        {
            return OxyPalette.Interpolate(numberOfColors, OxyColors.Blue, OxyColors.White, OxyColors.Red);
        }

        public static OxyPalette Cool(int numberOfColors)
        {
            return OxyPalette.Interpolate(numberOfColors, OxyColors.Cyan, OxyColors.Magenta);
        }

        public static OxyPalette Gray(int numberOfColors)
        {
            return OxyPalette.Interpolate(numberOfColors, OxyColors.Black, OxyColors.White);
        }

        public static OxyPalette Hot(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.Black,
                OxyColor.FromRgb(127, 0, 0),
                OxyColor.FromRgb(255, 127, 0),
                OxyColor.FromRgb(255, 255, 127),
                OxyColors.White);
        }

        public static OxyPalette Hue(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.Red,
                OxyColors.Yellow,
                OxyColors.Green,
                OxyColors.Cyan,
                OxyColors.Blue,
                OxyColors.Magenta,
                OxyColors.Red);
        }

        public static OxyPalette HueDistinct(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.Magenta,
                OxyColors.Blue,
                OxyColors.Cyan,
                OxyColors.Green,
                OxyColors.Yellow,
                OxyColors.Red);
        }


        public static OxyPalette Jet(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.DarkBlue,
                OxyColors.Cyan,
                OxyColors.Yellow,
                OxyColors.Orange,
                OxyColors.DarkRed);
        }

        public static OxyPalette Rainbow(int numberOfColors)
        {
            return OxyPalette.Interpolate(
                numberOfColors,
                OxyColors.Violet,
                OxyColors.Indigo,
                OxyColors.Blue,
                OxyColors.Green,
                OxyColors.Yellow,
                OxyColors.Orange,
                OxyColors.Red);
        }
    }
}
