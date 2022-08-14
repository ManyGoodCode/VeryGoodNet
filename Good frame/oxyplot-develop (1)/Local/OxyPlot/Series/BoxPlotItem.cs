namespace OxyPlot.Series
{
    using System.Collections.Generic;

    public class BoxPlotItem
    {
        public BoxPlotItem(
            double x,
            double lowerWhisker,
            double boxBottom,
            double median,
            double boxTop,
            double upperWhisker)
        {
            this.X = x;
            this.LowerWhisker = lowerWhisker;
            this.BoxBottom = boxBottom;
            this.Median = median;
            this.BoxTop = boxTop;
            this.UpperWhisker = upperWhisker;
            this.Mean = double.NaN;
            this.Outliers = new List<double>();
        }

        public double BoxBottom { get; set; }
        public double BoxTop { get; set; }
        public double LowerWhisker { get; set; }
        public double Median { get; set; }
        public double Mean { get; set; }
        public IList<double> Outliers { get; set; }
        public object Tag { get; set; }
        public double UpperWhisker { get; set; }
        public IList<double> Values
        {
            get
            {
                var values = new List<double> { this.LowerWhisker, this.BoxBottom, this.Median, this.BoxTop, this.UpperWhisker };
                if (!double.IsNaN(this.Mean))
                {
                    values.Add(this.Mean);
                }

                values.AddRange(this.Outliers);
                return values;
            }
        }

        public double X { get; set; }
        public override string ToString()
        {
            return string.Format(
                "{0} {1} {2} {3} {4} {5} {6} ",
                this.X,
                this.LowerWhisker,
                this.BoxBottom,
                this.Median,
                this.Mean,
                this.BoxTop,
                this.UpperWhisker);
        }
    }
}