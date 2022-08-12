namespace OxyPlot
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 扩展比较器的方法
    /// </summary>
    public static class ComparerHelper
    {
        /// <summary>
        /// 创建一个比较器对象。返回比较器接口
        /// </summary>
        public static IComparer<T> CreateComparer<T>(Comparison<T> comparison)
        {
            return new ComparisonComparer<T>(comparison);
        }

        /// <summary>
        /// 比较对象
        /// </summary>
        private class ComparisonComparer<T> : IComparer<T>
        {
            /// <summary>
            /// 比较器
            /// </summary>
            private readonly Comparison<T> comparison;

            /// <summary>
            /// 构造器注入一个比较器
            /// </summary>
            public ComparisonComparer(Comparison<T> comparison)
            {
                this.comparison = comparison;
            }

            /// <summary>
            /// 实现接口比较方法
            /// </summary>
            public int Compare(T x, T y)
            {
                return this.comparison.Invoke(x, y);
            }
        }
    }
}
