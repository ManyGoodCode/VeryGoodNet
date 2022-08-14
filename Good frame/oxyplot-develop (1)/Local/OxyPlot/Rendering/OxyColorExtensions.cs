namespace OxyPlot
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public static class OxyColorExtensions
    {
        public static OxyColor ChangeIntensity(this OxyColor color, double factor)
        {
            var hsv = color.ToHsv();
            hsv[2] *= factor;
            if (hsv[2] > 1.0)
            {
                hsv[2] = 1.0;
            }

            return OxyColor.FromHsv(hsv);
        }

        public static OxyColor ChangeSaturation(this OxyColor color, double factor)
        {
            var hsv = color.ToHsv();
            hsv[1] *= factor;
            if (hsv[1] > 1.0)
            {
                hsv[1] = 1.0;
            }

            return OxyColor.FromHsv(hsv);
        }

        public static OxyColor Complementary(this OxyColor color)
        {
            // http://en.wikipedia.org/wiki/Complementary_Color
            var hsv = color.ToHsv();
            double newHue = hsv[0] - 0.5;

            // clamp to [0,1]
            if (newHue < 0)
            {
                newHue += 1.0;
            }

            return OxyColor.FromHsv(newHue, hsv[1], hsv[2]);
        }

        public static double[] ToHsv(this OxyColor color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            byte min = Math.Min(Math.Min(r, g), b);
            byte v = Math.Max(Math.Max(r, g), b);
            double delta = v - min;

            double s = v.Equals(0) ? 0 : delta / v;
            double h = 0;

            if (s.Equals(0))
            {
                h = 0.0;
            }
            else
            {
                if (r == v)
                {
                    h = (g - b) / delta;
                }
                else if (g == v)
                {
                    h = 2 + ((b - r) / delta);
                }
                else if (b == v)
                {
                    h = 4 + ((r - g) / delta);
                }

                h *= 60;
                if (h < 0.0)
                {
                    h += 360;
                }
            }

            var hsv = new double[3];
            hsv[0] = h / 360.0;
            hsv[1] = s;
            hsv[2] = v / 255.0;
            return hsv;
        }

        public static uint ToUint(this OxyColor color)
        {
            var u = (uint)color.A << 24;
            u += (uint)color.R << 16;
            u += (uint)color.G << 8;
            u += color.B;
            return u;
        }


        public static string ToByteString(this OxyColor color)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", color.A, color.R, color.G, color.B);
        }

        public static string ToCode(this OxyColor color)
        {
            var name = color.GetColorName();
            if (name != null)
            {
                return string.Format("OxyColors.{0}", name);
            }

            return string.Format("OxyColor.FromArgb({0}, {1}, {2}, {3})", color.A, color.R, color.G, color.B);
        }

        public static string GetColorName(this OxyColor color)
        {
            var t = typeof(OxyColors);

            var colors = t.GetRuntimeFields().Where(fi => fi.IsPublic && fi.IsStatic);
            var colorField = colors.FirstOrDefault(field => color.Equals(field.GetValue(null)));

            return colorField != null ? colorField.Name : null;
        }
    }
}
