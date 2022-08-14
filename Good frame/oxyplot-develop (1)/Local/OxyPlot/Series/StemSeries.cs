namespace OxyPlot.Series
{
    using System.Collections.Generic;

    public class StemSeries : LineSeries
    {
        public StemSeries()
        {
            this.Base = 0;
        }

        public double Base { get; set; }
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if (this.XAxis == null || this.YAxis == null)
            {
                return null;
            }

            if (interpolate)
            {
                return null;
            }

            TrackerHitResult result = null;

            double minimumDistance = double.MaxValue;
            var points = this.ActualPoints;

            for (int i = 0; i < points.Count; i++)
            {
                var p1 = points[i];
                var basePoint = new DataPoint(p1.X, this.Base);
                var sp1 = this.Transform(p1);
                var sp2 = this.Transform(basePoint);
                var u = ScreenPointHelper.FindPositionOnLine(point, sp1, sp2);

                if (double.IsNaN(u))
                {
                    u = 1; // we are a tiny line, snap to the end
                }

                if (u < 0 || u > 1)
                {
                    u = 1; // we are outside the line, snap to the end
                }

                var sp = sp1 + ((sp2 - sp1) * u);
                double distance = (point - sp).LengthSquared;

                if (distance < minimumDistance)
                {
                    var item = this.GetItem(i);
                    result = new TrackerHitResult
                    {
                        Series = this,
                        DataPoint = new DataPoint(p1.X, p1.Y),
                        Position = new ScreenPoint(sp1.x, sp1.y),
                        Item = this.GetItem(i),
                        Index = i,
                        Text =
                            StringHelper.Format(
                                this.ActualCulture, 
                                this.TrackerFormatString,
                                item,
                                this.Title,
                                this.XAxis.Title ?? XYAxisSeries.DefaultXAxisTitle,
                                this.XAxis.GetValue(p1.X),
                                this.YAxis.Title ?? XYAxisSeries.DefaultYAxisTitle,
                                this.YAxis.GetValue(p1.Y))
                    };
                    minimumDistance = distance;
                }
            }

            return result;
        }


        public override void Render(IRenderContext rc)
        {
            if (this.ActualPoints.Count == 0)
            {
                return;
            }

            this.VerifyAxes();

            double minDistSquared = this.MinimumSegmentLength * this.MinimumSegmentLength;

            // Transform all points to screen coordinates
            // Render the line when invalid points occur
            var dashArray = this.ActualDashArray;
            var actualColor = this.GetSelectableColor(this.ActualColor);
            var points = new ScreenPoint[2];
            var markerPoints = this.MarkerType != MarkerType.None ? new List<ScreenPoint>(this.ActualPoints.Count) : null;
            foreach (var point in this.ActualPoints)
            {
                if (!this.IsValidPoint(point))
                {
                    continue;
                }

                points[0] = this.Transform(point.X, this.Base);
                points[1] = this.Transform(point.X, point.Y);

                if (this.StrokeThickness > 0 && this.ActualLineStyle != LineStyle.None)
                {
                    rc.DrawReducedLine(
                        points,
                        minDistSquared,
                        actualColor,
                        this.StrokeThickness,
                        this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                        dashArray,
                        this.LineJoin);
                }

                if (markerPoints != null)
                {
                    markerPoints.Add(points[1]);
                }
            }

            if (this.MarkerType != MarkerType.None)
            {
                rc.DrawMarkers(
                    markerPoints,
                    this.MarkerType,
                    this.MarkerOutline,
                    new[] { this.MarkerSize },
                    this.ActualMarkerFill,
                    this.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode);
            }
        }
    }
}
