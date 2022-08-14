namespace OxyPlot
{
    using OxyPlot.Series;

    public class TrackerHitResult
    {
        public DataPoint DataPoint { get; set; }

        public object Item { get; set; }

        public double Index { get; set; }
        public OxyRect LineExtents { get; set; }
        public PlotModel PlotModel { get; set; }
        public ScreenPoint Position { get; set; }
        public Series.Series Series { get; set; }
        public string Text { get; set; }
        public Axes.Axis XAxis
        {
            get
            {
                XYAxisSeries xyas = this.Series as XYAxisSeries;
                return xyas != null ? xyas.XAxis : null;
            }
        }

        public Axes.Axis YAxis
        {
            get
            {
                XYAxisSeries xyas = this.Series as XYAxisSeries;
                return xyas != null ? xyas.YAxis : null;
            }
        }

        public override string ToString()
        {
            return this.Text != null ? this.Text.Trim() : string.Empty;
        }
    }
}