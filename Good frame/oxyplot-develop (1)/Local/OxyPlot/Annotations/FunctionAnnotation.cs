namespace OxyPlot.Annotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FunctionAnnotation : PathAnnotation
    {
        public FunctionAnnotation()
        {
            this.Resolution = 400;
            this.Type = FunctionAnnotationType.EquationX;
        }

        public FunctionAnnotationType Type { get; set; }

        public Func<double, double> Equation { get; set; }
        public int Resolution { get; set; }

        protected override IList<ScreenPoint> GetScreenPoints()
        {
            Func<double, double> fx = null;
            Func<double, double> fy = null;

            switch (this.Type)
            {
                case FunctionAnnotationType.EquationX:
                    fx = this.Equation;
                    break;
                case FunctionAnnotationType.EquationY:
                    fy = this.Equation;
                    break;
            }

            List<DataPoint> points = new List<DataPoint>();
            if (fx != null)
            {
                double x = this.ActualMinimumX;

                double dx = (this.ActualMaximumX - this.ActualMinimumX) / this.Resolution;
                while (true)
                {
                    points.Add(new DataPoint(x, fx(x)));
                    if (x > this.ActualMaximumX)
                    {
                        break;
                    }

                    x += dx;
                }
            }
            else if (fy != null)
            {
                double y = this.ActualMinimumY;

                double dy = (this.ActualMaximumY - this.ActualMinimumY) / this.Resolution;
                while (true)
                {
                    points.Add(new DataPoint(fy(y), y));
                    if (y > this.ActualMaximumY)
                    {
                        break;
                    }

                    y += dy;
                }
            }

            return points.Select(this.Transform).ToList();
        }
    }
}