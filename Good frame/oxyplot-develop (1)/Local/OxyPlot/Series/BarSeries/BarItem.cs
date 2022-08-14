namespace OxyPlot.Series
{
    public class BarItem : BarItemBase, ICodeGenerating
    {
        public BarItem()
        {
            this.Value = double.NaN;
            this.Color = OxyColors.Automatic;
        }

        public BarItem(double value, int categoryIndex = -1)
        {
            this.Color = OxyColors.Automatic;
            this.Value = value;
            this.CategoryIndex = categoryIndex;
        }

        public OxyColor Color { get; set; }

        public double Value { get; set; }

        public virtual string ToCode()
        {
            if (!this.Color.IsUndefined())
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(), "{0},{1},{2}", this.Value, this.CategoryIndex, this.Color.ToCode());
            }

            if (this.CategoryIndex != -1)
            {
                return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1}", this.Value, this.CategoryIndex);
            }

            return CodeGenerator.FormatConstructor(this.GetType(), "{0}", this.Value);
        }
    }
}
