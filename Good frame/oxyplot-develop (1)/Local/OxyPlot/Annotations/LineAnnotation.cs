namespace OxyPlot.Annotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot.Axes;

    public class LineAnnotation : PathAnnotation
    {
        public LineAnnotation()
        {
            this.Type = LineAnnotationType.LinearEquation;
        }

        public double Intercept { get; set; }

        public double Slope { get; set; }

        public LineAnnotationType Type { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        protected override IList<ScreenPoint> GetScreenPoints()
        {
            // y=f(x)
            Func<double, double> fx = null;

            // x=f(y)
            Func<double, double> fy = null;

            switch (this.Type)
            {
                case LineAnnotationType.Horizontal:
                    fx = x => this.Y;
                    break;
                case LineAnnotationType.Vertical:
                    fy = y => this.X;
                    break;
                default:
                    fx = x => (this.Slope * x) + this.Intercept;
                    break;
            }

            List<DataPoint> points = new List<DataPoint>();

            bool isCurvedLine = !(this.XAxis is LinearAxis && this.YAxis is LinearAxis);

            if (!isCurvedLine)
            {
                if (fx != null)
                {
                    points.Add(new DataPoint(this.ActualMinimumX, fx(this.ActualMinimumX)));
                    points.Add(new DataPoint(this.ActualMaximumX, fx(this.ActualMaximumX)));
                }
                else
                {
                    points.Add(new DataPoint(fy(this.ActualMinimumY), this.ActualMinimumY));
                    points.Add(new DataPoint(fy(this.ActualMaximumY), this.ActualMaximumY));
                }
            }
            else
            {
                if (fx != null)
                {
                    int n = 100;
                    double dx = (this.ActualMaximumX - this.ActualMinimumX) / 100;
                    for (int i = 0; i <= n; i++)
                    {
                        double x = this.ActualMinimumX + i * dx;
                        points.Add(new DataPoint(x, fx(x)));
                    }
                }
                else
                {
                    int n = 100;
                    double dy = (this.ActualMaximumY - this.ActualMinimumY) / n;
                    for (int i = 0; i <= n; i++)
                    {
                        double y = this.ActualMinimumY + i * dy;
                        points.Add(new DataPoint(fy(y), y));
                    }
                }
            }

            return points.Select(this.Transform).ToArray();
        }
    }
}
