namespace OxyPlot
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 提供数组的有用扩展方法
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// 查找二维数组的最大值
        /// </summary>
        public static double MaxOrDefault(this IEnumerable<double> sequence, double defaultValue)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            using var e = sequence.GetEnumerator();
            if (!e.MoveNext())
            {
                return defaultValue;
            }

            double max = e.Current;
            while (e.MoveNext())
            {
                max = Math.Max(max, e.Current);
            }

            return max;
        }

        /// <summary>
        /// 查找数组中的最小值
        /// </summary>
        public static double MinOrDefault(this IEnumerable<double> sequence, double defaultValue)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            using var e = sequence.GetEnumerator();
            if (!e.MoveNext())
            {
                return defaultValue;
            }

            double min = e.Current;
            while (e.MoveNext())
            {
                min = Math.Min(min, e.Current);
            }

            return min;
        }

        /// <summary>
        /// 查找二维数组中的最大值
        /// </summary>
        public static double Max2D(this double[,] array)
        {
            double max = double.MinValue;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j].CompareTo(max) > 0)
                    {
                        max = array[i, j];
                    }
                }
            }

            return max;
        }

        /// <summary>
        /// 查找二维数组中的最小值
        /// </summary>
        public static double Min2D(this double[,] array, bool excludeNaN = false)
        {
            double min = double.MaxValue;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (excludeNaN && double.IsNaN(array[i, j]))
                    {
                        continue;
                    }

                    if (array[i, j].CompareTo(min) < 0)
                    {
                        min = array[i, j];
                    }
                }
            }

            return min;
        }
    }
}
