namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ScatterErrorSeries : ScatterSeries<ScatterErrorPoint>
    {
        public ScatterErrorSeries()
        {
            this.ErrorBarColor = OxyColors.Black;
            this.ErrorBarStrokeThickness = 1;
            this.ErrorBarStopWidth = 4.0;
            this.MinimumErrorSize = 0;
        }

        public string DataFieldErrorX { get; set; }
        public string DataFieldErrorY { get; set; }
        public OxyColor ErrorBarColor { get; set; }
        public double ErrorBarStopWidth { get; set; }
        public double ErrorBarStrokeThickness { get; set; }
        public double MinimumErrorSize { get; set; }
        public override void Render(IRenderContext rc)
        {
            base.Render(rc);

            var actualPoints = this.ActualPointsList;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            var segments = new List<ScreenPoint>();
            foreach (var point in actualPoints)
            {
                if (point == null)
                {
                    continue;
                }

                var errorBarVectorX = this.Orientate(new ScreenVector(0, this.ErrorBarStopWidth));
                var errorBarVectorY = this.Orientate(new ScreenVector(this.ErrorBarStopWidth, 0));

                if (point.ErrorX > 0.0)
                {
                    var leftErrorPoint = this.Transform(point.X - (point.ErrorX * 0.5), point.Y);
                    var rightErrorPoint = this.Transform(point.X + (point.ErrorX * 0.5), point.Y);

                    if (rightErrorPoint.DistanceTo(leftErrorPoint) > this.MarkerSize * this.MinimumErrorSize)
                    {
                        segments.Add(leftErrorPoint);
                        segments.Add(rightErrorPoint);
                        segments.Add(leftErrorPoint - errorBarVectorX);
                        segments.Add(leftErrorPoint + errorBarVectorX);
                        segments.Add(rightErrorPoint - errorBarVectorX);
                        segments.Add(rightErrorPoint + errorBarVectorX);
                    }
                }

                if (point.ErrorY > 0.0)
                {
                    var topErrorPoint = this.Transform(point.X, point.Y - (point.ErrorY * 0.5));
                    var bottomErrorPoint = this.Transform(point.X, point.Y + (point.ErrorY * 0.5));

                    if (topErrorPoint.DistanceTo(bottomErrorPoint) > this.MarkerSize * this.MinimumErrorSize)
                    {
                        segments.Add(topErrorPoint);
                        segments.Add(bottomErrorPoint);
                        segments.Add(topErrorPoint - errorBarVectorY);
                        segments.Add(topErrorPoint + errorBarVectorY);
                        segments.Add(bottomErrorPoint - errorBarVectorY);
                        segments.Add(bottomErrorPoint + errorBarVectorY);
                    }
                }
            }

            rc.DrawLineSegments(
                segments, 
                this.GetSelectableColor(this.ErrorBarColor), 
                this.ErrorBarStrokeThickness, 
                this.EdgeRenderingMode,
                null, 
                LineJoin.Bevel);
        }


        public void SelectAll(Func<ScatterErrorPoint, bool> func)
        {
            foreach (var dataPoint in this.Points.Where(func))
            {
                this.SelectItem(this.Points.IndexOf(dataPoint));
            }
        }

        protected override void UpdateFromDataFields()
        {
            var filler = new ListBuilder<ScatterErrorPoint>();
            filler.Add(this.DataFieldX, double.NaN);
            filler.Add(this.DataFieldY, double.NaN);
            filler.Add(this.DataFieldErrorX, double.NaN);
            filler.Add(this.DataFieldErrorY, double.NaN);
            filler.Add(this.DataFieldSize, double.NaN);
            filler.Add(this.DataFieldValue, double.NaN);
            filler.Add(this.DataFieldTag, (object)null);
            filler.FillT(this.ItemsSourcePoints, this.ItemsSource, args => new ScatterErrorPoint(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]), Convert.ToDouble(args[2]), Convert.ToDouble(args[3]), Convert.ToDouble(args[4]), Convert.ToDouble(args[5]), args[6]));
        }
    }
}
