namespace OxyPlot
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public struct ScreenVector : IEquatable<ScreenVector>
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        internal double x;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        internal double y;

        public ScreenVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        public double Length
        {
            get
            {
                return Math.Sqrt((this.x * this.x) + (this.y * this.y));
            }
        }

        public double LengthSquared
        {
            get { return (this.x * this.x) + (this.y * this.y); }
        }

        public double X
        {
            get { return this.x; }
        }

        public double Y
        {
            get { return this.y; }
        }

        public static ScreenVector operator *(ScreenVector v, double d)
        {
            return new ScreenVector(v.x * d, v.y * d);
        }

        public static ScreenVector operator +(ScreenVector v, ScreenVector d)
        {
            return new ScreenVector(v.x + d.x, v.y + d.y);
        }

        public static ScreenVector operator -(ScreenVector v, ScreenVector d)
        {
            return new ScreenVector(v.x - d.x, v.y - d.y);
        }

        public static ScreenVector operator -(ScreenVector v)
        {
            return new ScreenVector(-v.x, -v.y);
        }

        /// <summary>
        /// 使这个向量规范化
        /// </summary>
        public void Normalize()
        {
            double l = Math.Sqrt((this.x * this.x) + (this.y * this.y));
            if (l > 0)
            {
                this.x /= l;
                this.y /= l;
            }
        }

        public override string ToString()
        {
            return this.x + " " + this.y;
        }

        public bool Equals(ScreenVector other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y);
        }
    }
}
