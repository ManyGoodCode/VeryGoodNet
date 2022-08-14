namespace OxyPlot
{
    using System;
    using System.Globalization;
    using System.Text;

    public struct OxyRect : IFormattable, IEquatable<OxyRect>
    {
        public static readonly OxyRect Everything = new OxyRect(0, 0, double.PositiveInfinity, double.PositiveInfinity);
        private readonly double height;
        private readonly double left;
        private readonly double top;
        private readonly double width;
        public OxyRect(double left, double top, double width, double height)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width", "The width should not be negative.");
            }

            if (height < 0)
            {
                throw new ArgumentOutOfRangeException("height", "The height should not be negative.");
            }

            this.left = left;
            this.top = top;
            this.width = width;
            this.height = height;
        }

        public OxyRect(ScreenPoint p0, ScreenPoint p1)
            : this(Math.Min(p0.X, p1.X), Math.Min(p0.Y, p1.Y), Math.Abs(p1.X - p0.X), Math.Abs(p1.Y - p0.Y))
        {
        }

        public OxyRect(ScreenPoint p0, OxySize size)
            : this(p0.X, p0.Y, size.Width, size.Height)
        {
        }


        public double Bottom
        {
            get
            {
                return this.top + this.height;
            }
        }

        public double Height
        {
            get
            {
                return this.height;
            }
        }

        public double Left
        {
            get
            {
                return this.left;
            }
        }

        public double Right
        {
            get
            {
                return this.left + this.width;
            }
        }

        public double Top
        {
            get
            {
                return this.top;
            }
        }

        public double Width
        {
            get
            {
                return this.width;
            }
        }

        public ScreenPoint Center
        {
            get
            {
                return new ScreenPoint(this.left + (this.width * 0.5), this.top + (this.height * 0.5));
            }
        }


        public ScreenPoint TopLeft => new ScreenPoint(this.Left, this.Top);
        public ScreenPoint TopRight => new ScreenPoint(this.Right, this.Top);
        public ScreenPoint BottomLeft => new ScreenPoint(this.Left, this.Bottom);
        public ScreenPoint BottomRight => new ScreenPoint(this.Right, this.Bottom);
        public static OxyRect Create(double x0, double y0, double x1, double y1)
        {
            return new OxyRect(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x1 - x0), Math.Abs(y1 - y0));
        }

        public bool Contains(double x, double y)
        {
            return x >= this.Left && x <= this.Right && y >= this.Top && y <= this.Bottom;
        }

        public bool Contains(ScreenPoint p)
        {
            return this.Contains(p.x, p.y);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", this.left, this.top, this.width, this.height);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            const string Separator = ", ";
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(this.Left.ToString(format, formatProvider));
            builder.Append(Separator);
            builder.Append(this.Top.ToString(format, formatProvider));
            builder.Append(Separator);
            builder.Append(this.Width.ToString(format, formatProvider));
            builder.Append(Separator);
            builder.Append(this.Height.ToString(format, formatProvider));
            builder.Append(")");
            return builder.ToString();
        }


        public bool Equals(OxyRect other)
        {
            return this.Left.Equals(other.Left) && this.Top.Equals(other.Top) && this.Width.Equals(other.Width) && this.Height.Equals(other.Height);
        }

        public OxyRect Inflate(double dx, double dy)
        {
            return new OxyRect(this.left - dx, this.top - dy, this.width + (dx * 2), this.height + (dy * 2));
        }

        public OxyRect Inflate(OxyThickness t)
        {
            return new OxyRect(this.left - t.Left, this.top - t.Top, this.width + t.Left + t.Right, this.height + t.Top + t.Bottom);
        }

        public OxyRect Intersect(OxyRect rect)
        {
            var left = Math.Max(this.Left, rect.Left);
            var top = Math.Max(this.Top, rect.Top);
            var right = Math.Min(this.Right, rect.Right);
            var bottom = Math.Min(this.Bottom, rect.Bottom);

            if (right < left || bottom < top)
            {
                return new OxyRect();
            }

            return new OxyRect(left, top, right - left, bottom - top);
        }

        public OxyRect Deflate(OxyThickness t)
        {
            return new OxyRect(this.left + t.Left, this.top + t.Top, Math.Max(0, this.width - t.Left - t.Right), Math.Max(0, this.height - t.Top - t.Bottom));
        }

        public OxyRect Offset(double offsetX, double offsetY)
        {
            return new OxyRect(this.left + offsetX, this.top + offsetY, this.width, this.height);
        }

        public OxyRect Clip(OxyRect clipRect)
        {
            var clipRight = double.IsNegativeInfinity(clipRect.Left) && double.IsPositiveInfinity(clipRect.Width)
                            ? double.PositiveInfinity
                            : clipRect.Right;            
            
            var clipBottom = double.IsNegativeInfinity(clipRect.Top) && double.IsPositiveInfinity(clipRect.Height)
                            ? double.PositiveInfinity
                            : clipRect.Bottom;

            return Create(
                Math.Max(Math.Min(this.Left, clipRight), clipRect.Left),
                Math.Max(Math.Min(this.Top, clipBottom), clipRect.Top),
                Math.Max(Math.Min(this.Right, clipRight), clipRect.Left),
                Math.Max(Math.Min(this.Bottom, clipBottom), clipRect.Top));
        }
    }
}
