namespace OxyPlot.Series
{
    public class IntervalBarItem : BarItemBase, ICodeGenerating
    {
        public IntervalBarItem()
        {
            this.Color = OxyColors.Automatic;
        }


        public IntervalBarItem(double start, double end, string title = null)
            : this()
        {
            this.Start = start;
            this.End = end;
            this.Title = title;
        }


        public OxyColor Color { get; set; }

        public double End { get; set; }

        public double Start { get; set; }
        public string Title { get; set; }
        public string ToCode()
        {
            if (this.Color.IsUndefined())
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(), "{0},{1},{2},{3}", this.Start, this.End, this.Title, this.Color.ToCode());
            }

            if (this.Title != null)
            {
                return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1},{2}", this.Start, this.End, this.Title);
            }

            return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1}", this.Start, this.End);
        }
    }
}