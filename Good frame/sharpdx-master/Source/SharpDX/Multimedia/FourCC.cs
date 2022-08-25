using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SharpDX.Multimedia
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct FourCC : IEquatable<FourCC>, IFormattable
    {
        public static readonly FourCC Empty = new FourCC(0);

        private uint value;

        public FourCC(string fourCC)
        {
            if (fourCC.Length != 4)
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid length for FourCC(\"{0}\". Must be be 4 characters long ", fourCC), "fourCC");
            this.value = ((uint)fourCC[3]) << 24 | ((uint)fourCC[2]) << 16 | ((uint)fourCC[1]) << 8 | ((uint)fourCC[0]);
        }

        public FourCC(char byte1, char byte2, char byte3, char byte4)
        {
            this.value = ((uint)byte4) << 24 | ((uint)byte3) << 16 | ((uint)byte2) << 8 | ((uint)byte1);
        }

        public FourCC(uint fourCC)
        {
            this.value = fourCC;
        }

        public FourCC(int fourCC)
        {
            this.value = unchecked((uint)fourCC);
        }

        public static implicit operator uint(FourCC d)
        {
            return d.value;
        }

        public static implicit operator int(FourCC d)
        {
            return unchecked((int)d.value);
        }

        public static implicit operator FourCC(uint d)
        {
            return new FourCC(d);
        }

        public static implicit operator FourCC(int d)
        {
            return new FourCC(d);
        }

        public static implicit operator string(FourCC d)
        {
            return d.ToString();
        }

        public static implicit operator FourCC(string d)
        {
            return new FourCC(d);
        }

        public override string ToString()
        {
            return string.Format("{0}", new string(new[]
                                  {
                                      (char) (value & 0xFF),
                                      (char) ((value >> 8) & 0xFF),
                                      (char) ((value >> 16) & 0xFF),
                                      (char) ((value >> 24) & 0xFF),
                                  }));
        }

        public bool Equals(FourCC other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is FourCC && Equals((FourCC) obj);
        }

        public override int GetHashCode()
        {
            return (int) value;
        }

        public string ToString(string format, IFormatProvider formatProvider) 
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            switch (format.ToUpperInvariant())
            {
                case "G":
                    return this.ToString();

                case "I":
                    return this.value.ToString("X08", formatProvider);

                default:
                    return this.value.ToString(format, formatProvider);
            }
        }

        public static bool operator ==(FourCC left, FourCC right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FourCC left, FourCC right)
        {
            return !left.Equals(right);
        }
    }
}