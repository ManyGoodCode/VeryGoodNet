namespace OxyPlot
{
    using System.IO;

    /// <summary>
    /// 流提供的有用扩展方法
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 复制一个流
        /// </summary>
        public static void CopyTo(this Stream input, Stream output)
        {
            var buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
