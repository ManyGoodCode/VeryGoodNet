
namespace OxyPlot.Axes
{
    using System;

    public class MagnitudeAxis : LinearAxis
    {
        public MagnitudeAxis()
        {
            this.Position = AxisPosition.None;
            this.IsPanEnabled = false;
            this.IsZoomEnabled = false;

            this.MajorGridlineStyle = LineStyle.Solid;
            this.MinorGridlineStyle = LineStyle.Solid;
        }

        internal ScreenPoint MidPoint { get; set; }
        public override DataPoint InverseTransform(double x, double y, Axis yaxis)
        {
            var angleAxis = yaxis as AngleAxis;
            if (angleAxis == null)
            {
                throw new InvalidOperationException("Polar angle axis not defined!");
            }

            x -= this.MidPoint.x;
            y -= this.MidPoint.y;
            y *= -1;
            double th = Math.Atan2(y, x);
            double r = Math.Sqrt((x * x) + (y * y));
            x = (r / this.Scale) + this.Offset;
            y = (th / angleAxis.Scale) + angleAxis.Offset*Math.PI/180;
            return new DataPoint(x, y);
        }


        public override bool IsXyAxis()
        {
            return false;
        }

        public override void Render(IRenderContext rc, int pass)
        {
            var r = new MagnitudeAxisRenderer(rc, this.PlotModel);
            r.Render(this, pass);
        }

        public override ScreenPoint Transform(double x, double y, Axis yaxis)
        {
            var angleAxis = yaxis as AngleAxis;
            if (angleAxis == null)
            {
                throw new InvalidOperationException("Polar angle axis not defined!");
            }

            var r = (x - this.Offset) * this.Scale;
            var theta = (y - angleAxis.Offset) * angleAxis.Scale;

            return new ScreenPoint(this.MidPoint.x + (r * Math.Cos(theta / 180 * Math.PI)), this.MidPoint.y - (r * Math.Sin(theta / 180 * Math.PI)));
        }

        internal override void UpdateTransform(OxyRect bounds)
        {
            double x0 = bounds.Left;
            double x1 = bounds.Right;
            double y0 = bounds.Bottom;
            double y1 = bounds.Top;

            this.ScreenMin = new ScreenPoint(x0, y1);
            this.ScreenMax = new ScreenPoint(x1, y0);

            this.MidPoint = new ScreenPoint((x0 + x1) / 2, (y0 + y1) / 2);

            double r = Math.Min(Math.Abs(x1 - x0), Math.Abs(y1 - y0));

            var a0 = 0.0;
            var a1 = r * 0.5;

            double dx = a1 - a0;
            a1 = a0 + (this.EndPosition * dx);
            a0 = a0 + (this.StartPosition * dx);

            double marginSign = this.IsReversed ? -1.0 : 1.0;

            if (this.MinimumMargin > 0)
            {
                a0 += this.MinimumMargin * marginSign;
            }

            if (this.MaximumMargin > 0)
            {
                a1 -= this.MaximumMargin * marginSign;
            }

            if (this.MinimumDataMargin > 0)
            {
                a0 += this.MinimumDataMargin * marginSign;
            }

            if (this.MaximumDataMargin > 0)
            {
                a1 -= this.MaximumDataMargin * marginSign;
            }

            if (this.ActualMaximum - this.ActualMinimum < double.Epsilon)
            {
                this.ActualMaximum = this.ActualMinimum + 1;
            }

            double max = this.PreTransform(this.ActualMaximum);
            double min = this.PreTransform(this.ActualMinimum);

            double da = a0 - a1;
            double newOffset, newScale;
            if (Math.Abs(da) > double.Epsilon)
            {
                newOffset = (a0 / da * max) - (a1 / da * min);
            }
            else
            {
                newOffset = 0;
            }

            double range = max - min;
            if (Math.Abs(range) > double.Epsilon)
            {
                newScale = (a1 - a0) / range;
            }
            else
            {
                newScale = 1;
            }

            this.SetTransform(newScale, newOffset);

            if (this.MinimumDataMargin > 0)
            {
                this.ClipMinimum = this.InverseTransform(0.0);
            }
            else
            {
                this.ClipMinimum = this.ActualMinimum;
            }

            if (this.MaximumDataMargin > 0)
            {
                this.ClipMaximum = this.InverseTransform(r * 0.5);
            }
            else
            {
                this.ClipMaximum = this.ActualMaximum;
            }

            this.ActualMaximumAndMinimumChangedOverride();
        }
    }
}
