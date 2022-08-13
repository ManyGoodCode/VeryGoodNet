namespace OxyPlot
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public struct DataPoint : ICodeGenerating, IEquatable<DataPoint>
    {
        /// <summary>
        /// 未定义的点
        /// </summary>
        public static readonly DataPoint Undefined = new DataPoint(double.NaN, double.NaN);

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        internal readonly double x;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        internal readonly double y;

        public DataPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double X
        {
            get { return this.x; }
        }

        public double Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// Returns C# code that generates this instance.
        /// </summary>
        /// <returns>The to code.</returns>
        public string ToCode()
        {
            return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1}", this.x, this.y);
        }

        public bool Equals(DataPoint other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y);
        }

        public override string ToString()
        {
            return this.x + " " + this.y;
        }

        /// <summary>
        /// 检查X和y是否为NaN,此方法比 double.IsNaN 快
        /// </summary>
        public bool IsDefined()
        {
#pragma warning disable 1718
            return this.x == this.x && this.y == this.y;
        }
    }
}