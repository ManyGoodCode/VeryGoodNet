namespace OxyPlot
{
    using System;

    /// <summary>
    /// 创建数组的提供器
    /// </summary>
    public static class ArrayBuilder
    {
        /// <summary>
        /// 创建向量
        /// 将X1和X0线段等分成n个点.例如 0 , 11.11111111 , 22.22222222 , 33.33333333 , 44.44444444
        /// </summary>
        public static double[] CreateVector(double x0, double x1, int n)
        {
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = Math.Round(x0 + ((x1 - x0) * i / (n - 1)), 8);
            }

            return result;
        }

        /// <summary>
        /// 创建向量
        /// 将X1和X0线段等分成距离dx
        /// </summary>
        public static double[] CreateVector(double x0, double x1, double dx)
        {
            int n = (int)Math.Round((x1 - x0) / dx);
            double[] result = new double[n + 1];
            for (int i = 0; i <= n; i++)
            {
                result[i] = Math.Round(x0 + (i * dx), 8);
            }

            return result;
        }

        /// <summary>
        /// 数组 X[]中元素依次和Y[]中的元素做计算得到新的二维元素
        /// </summary>
        public static double[,] Evaluate(Func<double, double, double> f, double[] x, double[] y)
        {
            int m = x.Length;
            int n = y.Length;
            double[,] result = new double[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = f(x[i], y[j]);
                }
            }

            return result;
        }

        /// <summary>
        /// 填充数组为某个值
        /// </summary>
        public static void Fill(this double[] array, double value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        /// <summary>
        /// 填充二维数组为某个值 dimensional:二维
        /// </summary>
        public static void Fill2D(this double[,] array, double value)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = value;
                }
            }
        }
    }
}
