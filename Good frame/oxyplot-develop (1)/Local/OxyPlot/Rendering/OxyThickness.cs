namespace OxyPlot
{
    using System;
    using System.Globalization;

    public struct OxyThickness : ICodeGenerating
    {
        private readonly double bottom;
        private readonly double left;
        private readonly double right;
        private readonly double top;
        public OxyThickness(double thickness)
            : this(thickness, thickness, thickness, thickness)
        {
        }

        public OxyThickness(double left, double top, double right, double bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public double Bottom
        {
            get
            {
                return this.bottom;
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
                return this.right;
            }
        }

        public double Top
        {
            get
            {
                return this.top;
            }
        }

        public string ToCode()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "new OxyThickness({0},{1},{2},{3})",
                this.Left,
                this.Top,
                this.Right,
                this.Bottom);
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", this.left, this.top, this.right, this.bottom);
        }


        public bool Equals(OxyThickness other)
        {
            return this.Left.Equals(other.Left) && this.Top.Equals(other.Top) && this.Right.Equals(other.Right) && this.Bottom.Equals(other.Bottom);
        }


        public OxyThickness Include(OxyThickness other)
        {
            return new OxyThickness(Math.Max(other.Left, this.Left), Math.Max(other.Top, this.Top), Math.Max(other.Right, this.Right), Math.Max(other.Bottom, this.Bottom));
        }
    }
}
