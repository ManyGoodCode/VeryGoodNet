namespace OxyPlot
{
    /// <summary>
    /// 解码图像的功能
    /// </summary>
    public interface IImageDecoder
    {
        /// <summary>
        /// 获取有关指定字节数组中图像的信息
        /// </summary>
        OxyImageInfo GetImageInfo(byte[] bytes);

        /// <summary>
        /// 从指定的字节数组中解码图像
        /// </summary>
        /// <param name="bytes">图像数据</param>
        /// <returns>32位像素数据。 索引是[x,y]，其中[0,0]是左上角。 </returns>
        OxyColor[,] Decode(byte[] bytes);
    }
}