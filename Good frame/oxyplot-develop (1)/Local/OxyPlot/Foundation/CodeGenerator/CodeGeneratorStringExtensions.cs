﻿namespace OxyPlot
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class CodeGeneratorStringExtensions
    {
        public static string ToCode(this string value)
        {
            StringBuilder sb = new StringBuilder(capacity: value.Length + 2);
            sb.Append("\"");
            foreach (char c in value.Replace("\r\n", "\n"))
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append(@"\\"); break;
                    case '\n':
                        sb.Append(@"\n"); break;
                    case '\t':
                        sb.Append(@"\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }

            sb.Append("\"");
            return sb.ToString();
        }

        public static string ToCode(this bool value)
        {
            return value.ToString().ToLower();
        }

        public static string ToCode(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToCode(this Enum value)
        {
            return string.Format("{0}.{1}", value.GetType().Name, value);
        }

        public static string ToCode(this double value)
        {
            if (double.IsNaN(value))
            {
                return "double.NaN";
            }

            if (double.IsPositiveInfinity(value))
            {
                return "double.PositiveInfinity";
            }

            if (double.IsNegativeInfinity(value))
            {
                return "double.NegativeInfinity";
            }

            if (value.Equals(double.MinValue))
            {
                return "double.MinValue";
            }

            if (value.Equals(double.MaxValue))
            {
                return "double.MaxValue";
            }

            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToCode(this object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is int)
            {
                return ((int)value).ToCode();
            }

            if (value is double)
            {
                return ((double)value).ToCode();
            }

            if (value is string)
            {
                return ((string)value).ToCode();
            }

            if (value is bool)
            {
                return ((bool)value).ToCode();
            }

            if (value is Enum)
            {
                return ((Enum)value).ToCode();
            }

            if (value is ICodeGenerating)
            {
                return ((ICodeGenerating)value).ToCode();
            }

            return null;
        }
    }
}
