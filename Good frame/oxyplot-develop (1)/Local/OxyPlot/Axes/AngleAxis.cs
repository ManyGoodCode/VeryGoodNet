
namespace OxyPlot.Axes
{
    using System;
    using System.Collections.Generic;

    public class AngleAxis : LinearAxis
    {
        public AngleAxis()
        {
            this.Position = AxisPosition.All;
            this.TickStyle = TickStyle.None;
            this.IsPanEnabled = false;
            this.IsZoomEnabled = false;
            this.MajorGridlineStyle = LineStyle.Solid;
            this.MinorGridlineStyle = LineStyle.Solid;
            this.StartAngle = 0;
            this.EndAngle = 360;
        }

        public double StartAngle { get; set; }
        public double EndAngle { get; set; }

        public override void GetTickValues(
            out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            var minimum = this.StartAngle / this.Scale;
            var maximum = this.EndAngle / this.Scale;

            minorTickValues = this.CreateTickValues(minimum, maximum, this.ActualMinorStep);
            majorTickValues = this.CreateTickValues(minimum, maximum, this.ActualMajorStep);
            majorLabelValues = this.CreateTickValues(this.Minimum, this.Maximum, this.ActualMajorStep);

            minorTickValues = AxisUtilities.FilterRedundantMinorTicks(majorTickValues, minorTickValues);
        }

        public override DataPoint InverseTransform(double x, double y, Axis yaxis)
        {
            throw new InvalidOperationException("Angle axis should always be the y-axis.");
        }
        
        public override bool IsXyAxis()
        {
            return false;
        }

        public override void Render(IRenderContext rc, int pass)
        {
            var r = new AngleAxisRenderer(rc, this.PlotModel);
            r.Render(this, pass);
        }

        public override ScreenPoint Transform(double x, double y, Axis yaxis)
        {
            throw new InvalidOperationException("Angle axis should always be the y-axis.");
        }

        internal override void UpdateTransform(OxyRect bounds)
        {
            var x0 = bounds.Left;
            var x1 = bounds.Right;
            var y0 = bounds.Bottom;
            var y1 = bounds.Top;

            this.ScreenMin = new ScreenPoint(x0, y1);
            this.ScreenMax = new ScreenPoint(x1, y0);

            var newScale = (this.EndAngle - this.StartAngle) / (this.ActualMaximum - this.ActualMinimum);
            var newOffset = this.ActualMinimum - (this.StartAngle / newScale);
            this.SetTransform(newScale, newOffset);

            this.ClipMinimum = this.ActualMinimum;
            this.ClipMaximum = this.ActualMaximum;
        }
    }
}
