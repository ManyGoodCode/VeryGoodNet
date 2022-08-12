namespace OxyPlot
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// 屏幕空间中定义的点
    /// </summary>
    public struct ScreenPoint : IEquatable<ScreenPoint>
    {
        /// <summary>
        /// 未定义的点
        /// </summary>
        public static readonly ScreenPoint Undefined = new ScreenPoint(double.NaN, double.NaN);

        /// <summary>
        /// X坐标
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        internal double x;

        /// <summary>
        /// Y坐标
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        internal double y;

        /// <summary>
        /// 初始化屏幕坐标
        /// </summary>
        public ScreenPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 获取X坐标
        /// </summary>
        public double X
        {
            get { return this.x; }
        }

        /// <summary>
        /// 获取Y坐标
        /// </summary>
        public double Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// 检查点是否定义
        /// </summary>
        public static bool IsUndefined(ScreenPoint point)
        {
            return double.IsNaN(point.x) && double.IsNaN(point.y);
        }

        /// <summary>
        /// 屏幕坐标通过向量转换为另一个坐标
        /// </summary>
        public static ScreenPoint operator +(ScreenPoint p1, ScreenVector p2)
        {
            return new ScreenPoint(p1.x + p2.x, p1.y + p2.y);
        }

        /// <summary>
        /// 两点之间的向量
        /// </summary>
        public static ScreenVector operator -(ScreenPoint p1, ScreenPoint p2)
        {
            return new ScreenVector(p1.x - p2.x, p1.y - p2.y);
        }

        /// <summary>
        /// 点通过向量转换成另一点
        /// </summary>
        public static ScreenPoint operator -(ScreenPoint point, ScreenVector vector)
        {
            return new ScreenPoint(point.x - vector.x, point.y - vector.y);
        }

        /// <summary>
        /// 点之间的距离
        /// </summary>
        public double DistanceTo(ScreenPoint point)
        {
            double dx = point.x - this.x;
            double dy = point.y - this.y;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        /// <summary>
        /// 两点之间距离平方
        /// </summary>
        public double DistanceToSquared(ScreenPoint point)
        {
            double dx = point.x - this.x;
            double dy = point.y - this.y;
            return (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// 字符串
        /// </summary>
        public override string ToString()
        {
            return this.x + " " + this.y;
        }

        /// <summary>
        /// 检查是否相等 值
        /// </summary>
        public bool Equals(ScreenPoint other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y);
        }
    }
}
