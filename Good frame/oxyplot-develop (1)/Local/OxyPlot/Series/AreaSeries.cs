namespace OxyPlot.Series
{
    using System.Collections.Generic;
    using System.Linq;

    public class AreaSeries : LineSeries
    {
        private readonly List<DataPoint> points2 = new List<DataPoint>();
        private readonly List<DataPoint> itemsSourcePoints2 = new List<DataPoint>();
        private List<DataPoint> actualPoints2;
        public AreaSeries()
        {
            this.Reverse2 = true;
            this.Color2 = OxyColors.Automatic;
            this.Fill = OxyColors.Automatic;
        }

        public double ConstantY2 { get; set; }
        public string DataFieldX2 { get; set; }
        public string DataFieldY2 { get; set; }
        public OxyColor Color2 { get; set; }
        public virtual OxyColor ActualColor2
        {
            get
            {
                return this.Color2.GetActualColor(this.ActualColor);
            }
        }

        public OxyColor Fill { get; set; }
        public OxyColor ActualFill
        {
            get
            {
                return this.Fill.GetActualColor(OxyColor.FromAColor(100, this.ActualColor));
            }
        }

        public List<DataPoint> Points2
        {
            get
            {
                return this.points2;
            }
        }

        public bool Reverse2 { get; set; }
        protected List<DataPoint> ActualPoints2
        {
            get
            {
                return this.ItemsSource != null ? this.itemsSourcePoints2 : this.actualPoints2;
            }
        }

        protected int WindowStartIndex2 { get; set; }
        protected bool IsPoints2Defined { get; private set; }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            var xy = this.InverseTransform(point);
            var targetX = xy.X;
            int startIdx = this.IsXMonotonic 
                ? this.FindWindowStartIndex(this.ActualPoints, p => p.x, targetX, this.WindowStartIndex)
                : 0;
            int startIdx2 = this.IsXMonotonic
                ? this.FindWindowStartIndex(this.ActualPoints2, p => p.x, targetX, this.WindowStartIndex2)
                : 0;

            TrackerHitResult result1, result2;
            if (interpolate && this.CanTrackerInterpolatePoints)
            {
                result1 = this.GetNearestInterpolatedPointInternal(this.ActualPoints, startIdx, point);
                result2 = this.GetNearestInterpolatedPointInternal(this.ActualPoints2, startIdx2, point);
            }
            else
            {
                result1 = this.GetNearestPointInternal(this.ActualPoints, startIdx, point);
                result2 = this.GetNearestPointInternal(this.ActualPoints2, startIdx2, point);
            }

            TrackerHitResult result;
            if (result1 != null && result2 != null)
            {
                double dist1 = result1.Position.DistanceTo(point);
                double dist2 = result2.Position.DistanceTo(point);
                result = dist1 < dist2 ? result1 : result2;
            }
            else
            {
                result = result1 ?? result2;
            }

            if (result != null)
            {
                result.Text = StringHelper.Format(
                    this.ActualCulture,
                    this.TrackerFormatString,
                    result.Item,
                    this.Title,
                    this.XAxis.Title ?? XYAxisSeries.DefaultXAxisTitle,
                    this.XAxis.GetValue(result.DataPoint.X),
                    this.YAxis.Title ?? XYAxisSeries.DefaultYAxisTitle,
                    this.YAxis.GetValue(result.DataPoint.Y));
            }

            return result;
        }

        public override void Render(IRenderContext rc)
        {
            this.VerifyAxes();

            var actualPoints = this.ActualPoints;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            var actualPoints2 = this.ActualPoints2;
            if (actualPoints2 == null || actualPoints2.Count == 0)
            {
                return;
            }

            int startIdx = 0;
            int startIdx2 = 0;
            double xmax = double.MaxValue;

            if (this.IsXMonotonic)
            {
                // determine render range
                var xmin = this.XAxis.ClipMinimum;
                xmax = this.XAxis.ClipMaximum;
                this.WindowStartIndex = this.UpdateWindowStartIndex(actualPoints, point => point.X, xmin, this.WindowStartIndex);
                this.WindowStartIndex2 = this.UpdateWindowStartIndex(actualPoints2, point => point.X, xmin, this.WindowStartIndex2);

                startIdx = this.WindowStartIndex;
                startIdx2 = this.WindowStartIndex2;
            }

            double minDistSquared = this.MinimumSegmentLength * this.MinimumSegmentLength;

            var areaContext = new AreaRenderContext
            {
                Points = actualPoints,
                WindowStartIndex = startIdx,
                XMax = xmax,
                RenderContext = rc,
                MinDistSquared = minDistSquared,
                Reverse = false,
                Color = this.ActualColor,
                DashArray = this.ActualDashArray
            };

            var chunksOfPoints = this.RenderChunkedPoints(areaContext);

            areaContext.Points = actualPoints2;
            areaContext.WindowStartIndex = startIdx2;
            areaContext.Reverse = this.Reverse2;
            areaContext.Color = this.ActualColor2;
            
            var chunksOfPoints2 = this.RenderChunkedPoints(areaContext);

            if (chunksOfPoints.Count != chunksOfPoints2.Count)
            {
                return;
            }

            for (int chunkIndex = 0; chunkIndex < chunksOfPoints.Count; chunkIndex++)
            {
                var pts = chunksOfPoints[chunkIndex];
                var pts2 = chunksOfPoints2[chunkIndex];

                // combine the two lines and draw the clipped area
                var allPts = new List<ScreenPoint>();
                allPts.AddRange(pts2);
                allPts.AddRange(pts);
                rc.DrawReducedPolygon(
                    allPts,
                    minDistSquared,
                    this.GetSelectableFillColor(this.ActualFill),
                    OxyColors.Undefined,
                    0,
                    this.EdgeRenderingMode);

                var markerSizes = new[] { this.MarkerSize };

                // draw the markers on top
                rc.DrawMarkers(
                    pts,
                    this.MarkerType,
                    null,
                    markerSizes,
                    this.ActualMarkerFill,
                    this.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode,
                    1);
                rc.DrawMarkers(
                    pts2,
                    this.MarkerType,
                    null,
                    markerSizes,
                    this.MarkerFill,
                    this.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode,
                    1);
            }
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double y0 = (legendBox.Top * 0.2) + (legendBox.Bottom * 0.8);
            double y1 = (legendBox.Top * 0.4) + (legendBox.Bottom * 0.6);
            double y2 = (legendBox.Top * 0.8) + (legendBox.Bottom * 0.2);

            var pts0 = new[] { new ScreenPoint(legendBox.Left, y0), new ScreenPoint(legendBox.Right, y0) };
            var pts1 = new[] { new ScreenPoint(legendBox.Right, y2), new ScreenPoint(legendBox.Left, y1) };
            var pts = new List<ScreenPoint>();
            pts.AddRange(pts0);
            pts.AddRange(pts1);

            if (this.StrokeThickness > 0 && this.ActualLineStyle != LineStyle.None)
            {
                rc.DrawLine(pts0, this.GetSelectableColor(this.ActualColor), this.StrokeThickness, this.EdgeRenderingMode, this.ActualLineStyle.GetDashArray());
                rc.DrawLine(pts1, this.GetSelectableColor(this.ActualColor2), this.StrokeThickness, this.EdgeRenderingMode, this.ActualLineStyle.GetDashArray());
            }
            rc.DrawPolygon(pts, this.GetSelectableFillColor(this.ActualFill), OxyColors.Undefined, 0, this.EdgeRenderingMode);
        }

        protected internal override void UpdateData()
        {
            base.UpdateData();

            if (this.ItemsSource == null)
            {
                this.IsPoints2Defined = this.points2.Count > 0;

                if (this.IsPoints2Defined)
                {
                    this.actualPoints2 = this.points2;
                }
                else
                {
                    this.actualPoints2 = this.GetConstantPoints2().ToList();
                }

                return;
            }

            this.itemsSourcePoints2.Clear();

            // TODO: make it consistent with DataPointSeries.UpdateItemsSourcePoints
            // Using reflection on DataFieldX2 and DataFieldY2
            this.IsPoints2Defined = this.DataFieldX2 != null && this.DataFieldY2 != null;

            if (this.IsPoints2Defined)
            {
                var filler = new ListBuilder<DataPoint>();
                filler.Add(this.DataFieldX2, double.NaN);
                filler.Add(this.DataFieldY2, double.NaN);
                filler.Fill(this.itemsSourcePoints2, this.ItemsSource, args => new DataPoint(Axes.Axis.ToDouble(args[0]), Axes.Axis.ToDouble(args[1])));
            }
            else
            {
                this.itemsSourcePoints2.AddRange(this.GetConstantPoints2());
            }
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            this.InternalUpdateMaxMin(this.ActualPoints2);
        }

        protected List<List<ScreenPoint>> RenderChunkedPoints(AreaRenderContext context)
        {
            var result = new List<List<ScreenPoint>>();
            var screenPoints = new List<ScreenPoint>();

            int clipCount = 0;
            var actualPoints = context.Points;
            for (int i = context.WindowStartIndex; i < actualPoints.Count; i++)
            {
                var point = actualPoints[i];

                if (double.IsNaN(point.Y))
                {
                    if (screenPoints.Count == 0)
                    {
                        continue;
                    }

                    result.Add(this.RenderScreenPoints(context, screenPoints));
                    screenPoints = new List<ScreenPoint>();
                }
                else
                {
                    var sp = this.Transform(point.X, point.Y);
                    screenPoints.Add(sp);
                }

                // We break after two points were seen beyond xMax to ensure glitch-free rendering.
                clipCount += point.x > context.XMax ? 1 : 0;
                if (clipCount > 1)
                {
                    break;
                }
            }

            if (screenPoints.Count > 0)
            {
                result.Add(this.RenderScreenPoints(context, screenPoints));
            }

            return result;
        }

        protected virtual List<ScreenPoint> RenderScreenPoints(AreaRenderContext context, List<ScreenPoint> points)
        {
            var final = points;

            if (context.Reverse)
            {
                final.Reverse();
            }

            if (this.InterpolationAlgorithm != null)
            {
                var resampled = ScreenPointHelper.ResamplePoints(final, this.MinimumSegmentLength);
                final = this.InterpolationAlgorithm.CreateSpline(resampled, false, 0.25);
            }

            context.RenderContext.DrawReducedLine(
                final,
                context.MinDistSquared,
                this.GetSelectableColor(context.Color),
                this.StrokeThickness,
                this.EdgeRenderingMode,
                context.DashArray,
                this.LineJoin);

            return final;
        }

        protected double GetPointX(DataPoint point)
        {
            return point.x;
        }

        private IEnumerable<DataPoint> GetConstantPoints2()
        {
            var actualPoints = this.ActualPoints;
            if (!double.IsNaN(this.ConstantY2) && actualPoints.Count > 0)
            {
                // Use ConstantY2
                var x0 = actualPoints[0].X;
                var x1 = actualPoints[actualPoints.Count - 1].X;
                yield return new DataPoint(x0, this.ConstantY2);
                yield return new DataPoint(x1, this.ConstantY2);
            }
        }

        protected class AreaRenderContext
        {
            public List<DataPoint> Points { get; set; }
            public int WindowStartIndex { get; set; }
            public double XMax { get; set; }
            public IRenderContext RenderContext { get; set; }
            public double MinDistSquared { get; set; }
            public bool Reverse { get; set; }
            public OxyColor Color { get; set; }
            public double[] DashArray { get; set; }
        }
    }
}
