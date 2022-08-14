namespace OxyPlot
{
    using System;
    using System.Globalization;

    public struct OxyColor : ICodeGenerating, IEquatable<OxyColor>
    {
        private readonly byte r;
        private readonly byte g;
        private readonly byte b;
        private readonly byte a;

        private OxyColor(byte a, byte r, byte g, byte b)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public byte A
        {
            get
            {
                return this.a;
            }
        }

        public byte B
        {
            get
            {
                return this.b;
            }
        }

        public byte G
        {
            get
            {
                return this.g;
            }
        }

        public byte R
        {
            get
            {
                return this.r;
            }
        }

        public static OxyColor Parse(string value)
        {
            if (value == null || string.Equals(value, "none", StringComparison.OrdinalIgnoreCase))
            {
                return OxyColors.Undefined;
            }

            if (string.Equals(value, "auto", StringComparison.OrdinalIgnoreCase))
            {
                return OxyColors.Automatic;
            }

            value = value.Trim();
            if (value.StartsWith("#"))
            {
                value = value.Trim('#');
                if (value.Length == 3)
                {
                    value = string.Format("{0}{0}{1}{1}{2}{2}", value[0], value[1], value[2]);
                }

                var u = uint.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                if (value.Length < 8)
                {
                    u += 0xFF000000;
                }

                return FromUInt32(u);
            }

            var values = value.Split(',');
            if (values.Length < 3 || values.Length > 4)
            {
                throw new FormatException("Invalid format.");
            }

            var i = 0;

            byte alpha = 255;
            if (values.Length > 3)
            {
                alpha = byte.Parse(values[i++], CultureInfo.InvariantCulture);
            }

            var red = byte.Parse(values[i++], CultureInfo.InvariantCulture);
            var green = byte.Parse(values[i++], CultureInfo.InvariantCulture);
            var blue = byte.Parse(values[i], CultureInfo.InvariantCulture);
            return FromArgb(alpha, red, green, blue);
        }

        public static double ColorDifference(OxyColor c1, OxyColor c2)
        {
            // http://en.wikipedia.org/wiki/OxyColor_difference
            // http://mathworld.wolfram.com/L2-Norm.html
            double dr = (c1.R - c2.R) / 255.0;
            double dg = (c1.G - c2.G) / 255.0;
            double db = (c1.B - c2.B) / 255.0;
            double da = (c1.A - c2.A) / 255.0;
            double e = (dr * dr) + (dg * dg) + (db * db) + (da * da);
            return Math.Sqrt(e);
        }

        public static OxyColor FromUInt32(uint color)
        {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return FromArgb(a, r, g, b);
        }

        public static OxyColor FromHsv(double[] hsv)
        {
            if (hsv.Length != 3)
            {
                throw new InvalidOperationException("Wrong length of hsv array.");
            }

            return FromHsv(hsv[0], hsv[1], hsv[2]);
        }

        public static OxyColor FromHsv(double hue, double sat, double val)
        {
            double g, b;
            double r = g = b = 0;

            if (sat.Equals(0))
            {
                // Gray scale
                r = g = b = val;
            }
            else
            {
                if (hue.Equals(1))
                {
                    hue = 0;
                }

                hue *= 6.0;
                var i = (int)Math.Floor(hue);
                double f = hue - i;
                double aa = val * (1 - sat);
                double bb = val * (1 - (sat * f));
                double cc = val * (1 - (sat * (1 - f)));
                switch (i)
                {
                    case 0:
                        r = val;
                        g = cc;
                        b = aa;
                        break;
                    case 1:
                        r = bb;
                        g = val;
                        b = aa;
                        break;
                    case 2:
                        r = aa;
                        g = val;
                        b = cc;
                        break;
                    case 3:
                        r = aa;
                        g = bb;
                        b = val;
                        break;
                    case 4:
                        r = cc;
                        g = aa;
                        b = val;
                        break;
                    case 5:
                        r = val;
                        g = aa;
                        b = bb;
                        break;
                }
            }

            return FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        public static double HueDifference(OxyColor c1, OxyColor c2)
        {
            var hsv1 = c1.ToHsv();
            var hsv2 = c2.ToHsv();
            double dh = hsv1[0] - hsv2[0];

            // clamp to [-0.5,0.5]
            if (dh > 0.5)
            {
                dh -= 1.0;
            }

            if (dh < -0.5)
            {
                dh += 1.0;
            }

            double e = dh * dh;
            return Math.Sqrt(e);
        }

        public static OxyColor FromAColor(byte a, OxyColor color)
        {
            return FromArgb(a, color.R, color.G, color.B);
        }

        public static OxyColor FromArgb(byte a, byte r, byte g, byte b)
        {
            return new OxyColor(a, r, g, b);
        }


        public static OxyColor FromRgb(byte r, byte g, byte b)
        {
            return new OxyColor(255, r, g, b);
        }

        public static OxyColor Interpolate(OxyColor color1, OxyColor color2, double t)
        {
            double a = (color1.A * (1 - t)) + (color2.A * t);
            double r = (color1.R * (1 - t)) + (color2.R * t);
            double g = (color1.G * (1 - t)) + (color2.G * t);
            double b = (color1.B * (1 - t)) + (color2.B * t);
            return FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

        public static bool operator ==(OxyColor first, OxyColor second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(OxyColor first, OxyColor second)
        {
            return !first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(OxyColor))
            {
                return false;
            }

            return this.Equals((OxyColor)obj);
        }

        public bool Equals(OxyColor other)
        {
            return other.A == this.A && other.R == this.R && other.G == this.G && other.B == this.B;
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.A.GetHashCode();
                result = (result * 397) ^ this.R.GetHashCode();
                result = (result * 397) ^ this.G.GetHashCode();
                result = (result * 397) ^ this.B.GetHashCode();
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture, "#{0:x2}{1:x2}{2:x2}{3:x2}", this.A, this.R, this.G, this.B);
        }

        public bool IsInvisible()
        {
            return this.A == 0;
        }

        public bool IsVisible()
        {
            return this.A > 0;
        }


        public bool IsUndefined()
        {
            return this.Equals(OxyColors.Undefined);
        }

        public bool IsAutomatic()
        {
            return this.Equals(OxyColors.Automatic);
        }

        public OxyColor GetActualColor(OxyColor defaultColor)
        {
            return this.IsAutomatic() ? defaultColor : this;
        }


        string ICodeGenerating.ToCode()
        {
            return this.ToCode();
        }
    }
}
