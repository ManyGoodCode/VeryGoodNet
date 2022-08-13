namespace OxyPlot
{
    public class OxyImageInfo
    {
        /// <summary>
        /// 像素宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 像素高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 获取或设置每个像素的位
        /// </summary>
        public int BitsPerPixel { get; set; }

        /// <summary>
        /// 获取或设置图像的水平分辨率
        /// </summary>
        public double DpiX { get; set; }

        /// <summary>
        /// 获取或设置图像的垂直分辨率
        /// </summary>
        public double DpiY { get; set; }
    }
}