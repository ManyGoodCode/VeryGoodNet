namespace OxyPlot
{
    public interface ICodeGenerating
    {
        /// <summary>
        /// 返回C#语言创建该实例的代码
        /// 例如:new DataPoint(1,2) ; new DataPoint(double.NaN,2)
        /// </summary>
        string ToCode();
    }
}