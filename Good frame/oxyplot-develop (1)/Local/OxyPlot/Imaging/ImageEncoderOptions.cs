namespace OxyPlot
{
    public abstract class ImageEncoderOptions
    {
        protected ImageEncoderOptions()
        {
            this.DpiX = 96;
            this.DpiY = 96;
        }

        /// <summary>
        /// 以每英寸点为单位
        /// </summary>
        public double DpiX { get; set; }

        /// <summary>
        /// 以每英寸点为单位
        /// </summary>
        public double DpiY { get; set; }
    }
}