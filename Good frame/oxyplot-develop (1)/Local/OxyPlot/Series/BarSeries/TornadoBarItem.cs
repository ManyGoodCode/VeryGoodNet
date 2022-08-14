namespace OxyPlot.Series
{
    public class TornadoBarItem : BarItemBase, ICodeGenerating
    {
        public TornadoBarItem()
        {
            this.Minimum = double.NaN;
            this.Maximum = double.NaN;
            this.BaseValue = double.NaN;
            this.MinimumColor = OxyColors.Automatic;
            this.MaximumColor = OxyColors.Automatic;
        }


        public double BaseValue { get; set; }
        public double Maximum { get; set; }
        public OxyColor MaximumColor { get; set; }
        public double Minimum { get; set; }
        public OxyColor MinimumColor { get; set; }
        public string ToCode()
        {
            if (!this.MaximumColor.IsUndefined())
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(),
                    "{0},{1},{2},{3},{4}",
                    this.Minimum,
                    this.Maximum,
                    this.BaseValue,
                    this.MinimumColor.ToCode(),
                    this.MaximumColor.ToCode());
            }

            if (!this.MinimumColor.IsUndefined())
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(),
                    "{0},{1},{2},{3}",
                    this.Minimum,
                    this.Maximum,
                    this.BaseValue,
                    this.MinimumColor.ToCode());
            }

            if (!double.IsNaN(this.BaseValue))
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(), "{0},{1},{2}", this.Minimum, this.Maximum, this.BaseValue);
            }

            return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1}", this.Minimum, this.Maximum);
        }
    }
}