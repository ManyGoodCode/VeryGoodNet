namespace OxyPlot
{
    using System;
    using System.Globalization;

    public static class FractionHelper
    {
        public static string ConvertToFractionString(
            double value,
            double unit = 1,
            string unitSymbol = null,
            double eps = 1e-6,
            IFormatProvider formatProvider = null,
            string formatString = null)
        {
            if (Math.Abs(value) < eps)
            {
                return "0";
            }

            value /= unit;
            for (int d = 1; d <= 64; d++)
            {
                double n = value * d;
                var ni = (int)Math.Round(n);
                if (Math.Abs(n - ni) < eps)
                {
                    string nis = unitSymbol == null || ni != 1 ? ni.ToString(CultureInfo.InvariantCulture) : string.Empty;
                    if (d == 1)
                    {
                        return string.Format("{0}{1}", nis, unitSymbol);
                    }

                    return string.Format("{0}{1}/{2}", nis, unitSymbol, d);
                }
            }

            var format = string.IsNullOrEmpty(formatString) ? "{0}{1}" : "{0:" + formatString + "}{1}";
            return string.Format(formatProvider ?? CultureInfo.CurrentCulture, format, value, unitSymbol);
        }
    }
}