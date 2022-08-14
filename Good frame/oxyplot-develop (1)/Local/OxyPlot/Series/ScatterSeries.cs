namespace OxyPlot.Series
{
    using System;

    public class ScatterSeries : ScatterSeries<ScatterPoint>
    {
        protected override void UpdateFromDataFields()
        {
            var filler = new ListBuilder<ScatterPoint>();
            filler.Add(this.DataFieldX, double.NaN);
            filler.Add(this.DataFieldY, double.NaN);
            filler.Add(this.DataFieldSize, double.NaN);
            filler.Add(this.DataFieldValue, double.NaN);
            filler.Add(this.DataFieldTag, (object)null);
            filler.FillT(this.ItemsSourcePoints, this.ItemsSource, args => new ScatterPoint(Axes.Axis.ToDouble(args[0]), Axes.Axis.ToDouble(args[1]), Axes.Axis.ToDouble(args[2]), Axes.Axis.ToDouble(args[3]), args[4]));
        }
    }
}