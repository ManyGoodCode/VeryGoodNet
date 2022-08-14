namespace OxyPlot.Series
{
    public class HighLowItem : ICodeGenerating
    {
        public static readonly HighLowItem Undefined = new HighLowItem(double.NaN, double.NaN, double.NaN);

        public HighLowItem()
        {
        }

        public HighLowItem(double x, double high, double low, double open = double.NaN, double close = double.NaN)
        {
            this.X = x;
            this.High = high;
            this.Low = low;
            this.Open = open;
            this.Close = close;
        }

        public double Close { get; set; }

        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public double X { get; set; }
        public string ToCode()
        {
            return CodeGenerator.FormatConstructor(
                this.GetType(), "{0},{1},{2},{3},{4}", this.X, this.High, this.Low, this.Open, this.Close);
        }
    }
}