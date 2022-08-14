namespace OxyPlot.Series
{
    public class HistogramItem : ICodeGenerating
    {
        public HistogramItem(double rangeStart, double rangeEnd, double area, int count)
            : this(rangeStart, rangeEnd, area, count, OxyColors.Automatic)
        {
        }

        public HistogramItem(double rangeStart, double rangeEnd, double area, int count, OxyColor color)
        {
            this.RangeStart = rangeStart;
            this.RangeEnd = rangeEnd;
            this.Area = area;
            this.Count = count;
            this.Color = color;
        }

        public double RangeStart { get; set; }
        public double RangeEnd { get; set; }
        public double Area { get; set; }
        public double RangeCenter => this.RangeStart + ((this.RangeEnd - this.RangeStart) / 2);
        public int Count { get; set; }
        public OxyColor Color { get; set; }
        public double Width => this.RangeEnd - this.RangeStart;
        public double Height => this.Area / this.Width;
        public double Value => this.Height;
        public bool Contains(DataPoint p)
        {
            if (this.Height < 0)
            {
                return (p.X <= this.RangeEnd && p.X >= this.RangeStart && p.Y >= this.Height && p.Y <= 0) ||
                       (p.X <= this.RangeStart && p.X >= this.RangeEnd && p.Y >= this.Height && p.Y <= 0);
            }
            else
            {
                return (p.X <= this.RangeEnd && p.X >= this.RangeStart && p.Y <= this.Height && p.Y >= 0) ||
                       (p.X <= this.RangeStart && p.X >= this.RangeEnd && p.Y <= this.Height && p.Y >= 0);
            }
        }

        public string ToCode()
        {
            if (!this.Color.IsAutomatic())
            {
                return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1},{2},{3},{4}", this.RangeStart, this.RangeEnd, this.Area, this.Count, this.Color);
            }
            else
            {
                return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1},{2},{3}", this.RangeStart, this.RangeEnd, this.Area, this.Count);
            }
        }

        public override string ToString()
        {
            return string.Format(
                "{0} {1} {2} {3}",
                this.RangeStart,
                this.RangeEnd,
                this.Area,
                this.Count);
        }
    }
}
