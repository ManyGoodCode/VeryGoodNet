namespace OxyPlot.Series
{
    using System.Collections.Generic;
    using System.Linq;

    public class TwoColorAreaSeries : AreaSeries
    {
        private OxyColor defaultColor2;
        private List<DataPoint> abovePoints;
        private List<DataPoint> belowPoints;
        private int markerStartIndex;
        public TwoColorAreaSeries()
        {
            this.Color2 = OxyColors.Blue;

            this.Fill = OxyColors.Automatic;
            this.Fill2 = OxyColors.Automatic;

            this.MarkerFill2 = OxyColors.Automatic;
            this.MarkerStroke2 = OxyColors.Automatic;
            this.LineStyle2 = LineStyle.Solid;
        }

        public OxyColor Fill2 { get; set; }
        public OxyColor ActualFill2
        {
            get
            {
                return this.Fill2.GetActualColor(OxyColor.FromAColor(100, this.ActualColor2));
            }
        }

        public override OxyColor ActualColor2
        {
            get { return this.Color2.GetActualColor(this.defaultColor2); }
        }

        public double[] Dashes2 { get; set; }
        public LineStyle LineStyle2 { get; set; }
        public LineStyle ActualLineStyle2
        {
            get
            {
                return this.LineStyle2 != LineStyle.Automatic ? this.LineStyle2 : LineStyle.Solid;
            }
        }


        public double[] ActualDashArray2
        {
            get
            {
                return this.Dashes2 ?? this.ActualLineStyle2.GetDashArray();
            }
        }

        public OxyColor MarkerFill2 { get; set; }
        public OxyColor MarkerStroke2 { get; set; }
        public double Limit { get; set; }
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            TrackerHitResult result;

            if (interpolate && this.CanTrackerInterpolatePoints)
            {
                result = this.GetNearestInterpolatedPointInternal(this.ActualPoints, point);
            }
            else
            {
                result = this.GetNearestPointInternal(this.ActualPoints, point);
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
            var xmin = this.XAxis.ClipMinimum;
            var xmax = this.XAxis.ClipMaximum;
            this.WindowStartIndex = this.UpdateWindowStartIndex(this.abovePoints, this.GetPointX, xmin, this.WindowStartIndex);
            this.WindowStartIndex2 = this.UpdateWindowStartIndex(this.belowPoints, this.GetPointX, xmin, this.WindowStartIndex2);

            double minDistSquared = this.MinimumSegmentLength * this.MinimumSegmentLength;

            var areaContext = new TwoColorAreaRenderContext
            {
                Points = this.abovePoints,
                WindowStartIndex = this.WindowStartIndex,
                XMax = xmax,
                RenderContext = rc,
                MinDistSquared = minDistSquared,
                Reverse = false,
                Color = this.ActualColor,
                Fill = this.ActualFill,
                MarkerFill = this.ActualMarkerFill,
                MarkerStroke = this.MarkerStroke,
                DashArray = this.ActualDashArray,
                Baseline = this.Limit
            };

            this.RenderChunkedPoints(areaContext);

            areaContext.Points = this.belowPoints;
            areaContext.WindowStartIndex = this.WindowStartIndex2;
            areaContext.Reverse = this.Reverse2;
            areaContext.Color = this.ActualColor2;
            areaContext.Fill = this.ActualFill2;
            areaContext.MarkerFill = this.MarkerFill2;
            areaContext.MarkerStroke = this.MarkerStroke2;
            areaContext.DashArray = this.ActualDashArray2;
            if (this.IsPoints2Defined)
            {
                areaContext.Baseline = this.ConstantY2;
            }

            this.RenderChunkedPoints(areaContext);

            if (!this.IsPoints2Defined)
            {
                var markerSizes = new[] { this.MarkerSize };
                double limit = this.Limit;
                var points = this.ActualPoints;
                var aboveMarkers = new List<ScreenPoint>();
                var belowMarkers = new List<ScreenPoint>();
                this.markerStartIndex = this.UpdateWindowStartIndex(points, this.GetPointX, xmin, this.markerStartIndex);

                int markerClipCount = 0;
                for (int i = this.markerStartIndex; i < points.Count; i++)
                {
                    var point = points[i];
                    (point.y >= limit ? aboveMarkers : belowMarkers).Add(this.Transform(point.x, point.y));

                    markerClipCount += point.x > xmax ? 1 : 0;
                    if (markerClipCount > 1)
                    {
                        break;
                    }
                }

                rc.DrawMarkers(
                    aboveMarkers,
                    this.MarkerType,
                    null,
                    markerSizes, 
                    this.ActualMarkerFill, 
                    this.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode,
                    1);
                rc.DrawMarkers(
                    belowMarkers,
                    this.MarkerType,
                    null,
                    markerSizes,
                    this.MarkerFill2,
                    this.MarkerStroke2,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode,
                    1);
            }
        }


        protected internal override void SetDefaultValues()
        {
            base.SetDefaultValues();

            if (this.Color2.IsAutomatic())
            {
                this.defaultColor2 = this.PlotModel.GetDefaultColor();
            }

            if (this.LineStyle2 == LineStyle.Automatic)
            {
                this.LineStyle2 = this.PlotModel.GetDefaultLineStyle();
            }
        }

        protected internal override void UpdateData()
        {
            base.UpdateData();

            if (this.IsPoints2Defined)
            {
                this.abovePoints = this.ActualPoints;
                this.belowPoints = this.ActualPoints2;
            }
            else
            {
                this.SplitPoints(this.ActualPoints);
            }
        }

        protected override List<ScreenPoint> RenderScreenPoints(AreaRenderContext context, List<ScreenPoint> points)
        {
            var result = base.RenderScreenPoints(context, points);
            var twoColorContext = (TwoColorAreaRenderContext)context;

            var baseline = this.GetConstantScreenPoints2(result, twoColorContext.Baseline);
            var poligon = new List<ScreenPoint>(baseline);
            poligon.AddRange(result);

            context.RenderContext.DrawReducedPolygon(
                poligon,
                context.MinDistSquared,
                this.GetSelectableFillColor(twoColorContext.Fill),
                OxyColors.Undefined,
                0,
                this.EdgeRenderingMode);

            if (this.IsPoints2Defined)
            {
                var markerSizes = new[] { this.MarkerSize };

                // draw the markers on top
                context.RenderContext.DrawMarkers(
                    result,
                    this.MarkerType,
                    null,
                    markerSizes,
                    twoColorContext.MarkerFill,
                    twoColorContext.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode,
                    1);
            }

            return result;
        }

        private void SplitPoints(List<DataPoint> source)
        {
            var nan = new DataPoint(double.NaN, double.NaN);
            double limit = this.Limit;
            this.abovePoints = new List<DataPoint>(source.Count);
            this.belowPoints = new List<DataPoint>(source.Count);

            bool lastAbove = false;
            DataPoint? lastPoint = null;
            foreach (var point in source)
            {
                bool isAbove = point.y >= limit;

                if (lastPoint != null && isAbove != lastAbove)
                {
                    var shared = new DataPoint(this.GetInterpolatedX(lastPoint.Value, point, limit), limit);
                    this.abovePoints.Add(isAbove ? nan : shared);
                    this.abovePoints.Add(isAbove ? shared : nan);

                    this.belowPoints.Add(isAbove ? shared : nan);
                    this.belowPoints.Add(isAbove ? nan : shared);
                }

                (isAbove ? this.abovePoints : this.belowPoints).Add(point);

                lastPoint = point;
                lastAbove = isAbove;
            }
        }

        private List<ScreenPoint> GetConstantScreenPoints2(List<ScreenPoint> source, double baseline)
        {
            var result = new List<ScreenPoint>();

            if (double.IsNaN(baseline) || source.Count <= 0)
            {
                return result;
            }

            var p1 = this.InverseTransform(source[0]);
            p1 = new DataPoint(p1.X, baseline);
            result.Add(this.Transform(p1));

            var p2 = this.InverseTransform(source[source.Count - 1]);
            p2 = new DataPoint(p2.X, baseline);
            result.Add(this.Transform(p2));

            if (this.Reverse2)
            {
                result.Reverse();
            }

            return result;
        }

        private double GetInterpolatedX(DataPoint a, DataPoint b, double y)
        {
            return (((y - a.y) / (b.y - a.y)) * (b.x - a.x)) + a.x;
        }

        protected class TwoColorAreaRenderContext : AreaRenderContext
        {
            public double Baseline { get; set; }
            public OxyColor Fill { get; set; }
            public OxyColor MarkerFill { get; set; }
            public OxyColor MarkerStroke { get; set; }
        }
    }
}
