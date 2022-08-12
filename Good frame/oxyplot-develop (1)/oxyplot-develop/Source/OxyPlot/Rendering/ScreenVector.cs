namespace OxyPlot
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a vector defined in screen space.
    /// </summary>
    public struct ScreenVector : IEquatable<ScreenVector>
    {
        /// <summary>
        /// The x-coordinate.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        internal double x;

        /// <summary>
        /// The y-coordinate.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        internal double y;

        /// <summary>
        /// Initializes a new instance of the structure.
        /// </summary>
        public ScreenVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt((this.x * this.x) + (this.y * this.y));
            }
        }

        /// <summary>
        /// Gets the length squared.
        /// </summary>
        public double LengthSquared
        {
            get { return (this.x * this.x) + (this.y * this.y); }
        }

        /// <summary>
        /// Gets the x-coordinate.
        /// </summary>
        public double X
        {
            get { return this.x; }
        }

        /// <summary>
        /// Gets the y-coordinate.
        /// </summary>
        public double Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        public static ScreenVector operator *(ScreenVector v, double d)
        {
            return new ScreenVector(v.x * d, v.y * d);
        }

        /// <summary>
        /// Adds a vector to another vector.
        /// </summary>
        public static ScreenVector operator +(ScreenVector v, ScreenVector d)
        {
            return new ScreenVector(v.x + d.x, v.y + d.y);
        }

        /// <summary>
        /// Subtracts one specified vector from another.
        /// </summary>
        public static ScreenVector operator -(ScreenVector v, ScreenVector d)
        {
            return new ScreenVector(v.x - d.x, v.y - d.y);
        }

        /// <summary>
        /// Negates the specified vector.
        /// </summary>
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

        /// <summary>
        /// Returns that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return this.x + " " + this.y;
        }

        /// <summary>
        /// Determines whether this instance and another specified object have the same value.
        /// </summary>
        public bool Equals(ScreenVector other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y);
        }
    }
}
