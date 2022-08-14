namespace OxyPlot
{
    using System;
    using System.Globalization;
    using System.Text;

    public struct OxySize : IFormattable, IEquatable<OxySize>
    {
        public static readonly OxySize Empty = new OxySize(0, 0);
        private readonly double height;
        private readonly double width;
        public OxySize(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

        public double Height
        {
            get
            {
                return this.height;
            }
        }

        public double Width
        {
            get
            {
                return this.width;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1})", this.Width, this.Height);
        }


        public string ToString(string format, IFormatProvider formatProvider)
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(this.Width.ToString(format, formatProvider));
            builder.Append(","); // or get from culture?
            builder.Append(" ");
            builder.Append(this.Height.ToString(format, formatProvider));
            builder.Append(")");
            return builder.ToString();
        }

        public bool Equals(OxySize other)
        {
            return this.Width.Equals(other.Width) && this.Height.Equals(other.Height);
        }


        public OxySize Include(OxySize other)
        {
            return new OxySize(Math.Max(this.Width, other.Width), Math.Max(this.Height, other.Height));
        }
    }
}
