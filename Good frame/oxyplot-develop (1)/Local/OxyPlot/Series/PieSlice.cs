namespace OxyPlot.Series
{
    public class PieSlice : ICodeGenerating
    {
        public PieSlice(string label, double value)
        {
            this.Fill = OxyColors.Automatic;
            this.Label = label;
            this.Value = value;
        }

        public OxyColor Fill { get; set; }
        public OxyColor ActualFillColor
        {
            get { return this.Fill.GetActualColor(this.DefaultFillColor); }
        }

        public bool IsExploded { get; set; }
        public string Label { get; private set; }
        public double Value { get; private set; }
        internal OxyColor DefaultFillColor { get; set; }
        public string ToCode()
        {
            return CodeGenerator.FormatConstructor(
                this.GetType(), "{0}, {1}", this.Label, this.Value);
        }
    }
}