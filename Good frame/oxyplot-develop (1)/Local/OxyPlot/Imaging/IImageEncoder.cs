namespace OxyPlot
{
    /// <summary>
    /// 对图像进行编码的功能
    /// </summary>
    public interface IImageEncoder
    {
        /// <summary>
        /// 对指定像素进行编码
        /// </summary>
        /// <param name="pixels">像素数据。 索引是[x,y]，其中[0,0]是左上角。 </param>
        /// <returns>图像数据</returns>
        byte[] Encode(OxyColor[,] pixels);

        /// <summary>
        /// 对指定的8位索引像素进行编码
        /// </summary>
        /// <param name="pixels">索引的像素数据。 索引是[x,y]，其中[0,0]是左上角。 </param>
        /// <param name="palette">调色板</param>
        /// <returns>图像数据</returns>
        byte[] Encode(byte[,] pixels, OxyColor[] palette);
    }
}