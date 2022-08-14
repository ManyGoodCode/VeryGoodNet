namespace OxyPlot.Series
{
    public class ErrorBarItem : BarItem
    {
        public ErrorBarItem()
        {
            this.Color = OxyColors.Automatic;
        }

        public ErrorBarItem(double value, double error, int categoryIndex = -1)
            : this()
        {
            this.Value = value;
            this.Error = error;
            this.CategoryIndex = categoryIndex;
        }


        public double Error { get; set; }

        public override string ToCode()
        {
            if (!this.Color.IsUndefined())
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(), "{0},{1},{2},{3}", this.Value, this.Error, this.CategoryIndex, this.Color.ToCode());
            }

            if (this.CategoryIndex != -1)
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(), "{0},{1},{2}", this.Value, this.Error, this.CategoryIndex);
            }

            return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1}", this.Value, this.Error);
        }
    }
}
