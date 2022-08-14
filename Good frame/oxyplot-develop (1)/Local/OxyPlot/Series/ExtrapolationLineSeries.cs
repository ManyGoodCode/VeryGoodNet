namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OxyPlot;
    using OxyPlot.Series;

    public class ExtrapolationLineSeries : LineSeries
    {
        private readonly OxyColor defaultExtrapolationColor = OxyColors.Black;
        private readonly LineStyle defaultExtrapolationLineStyle = LineStyle.Dash;
        private List<DataRange> orderedIntervals;
        public ExtrapolationLineSeries()
        {
            this.ExtrapolationColor = OxyColors.Black;
            this.LineStyle = LineStyle.Dot;
        }

        public OxyColor ExtrapolationColor { get; set; }
        public OxyColor ActualExtrapolationColor => this.ExtrapolationColor.GetActualColor(this.defaultExtrapolationColor);

        public double[] ExtrapolationDashes { get; set; }
        public LineStyle ExtrapolationLineStyle { get; set; }
        public LineStyle ActualExtrapolationLineStyle
        {
            get
            {
                return this.ExtrapolationLineStyle != LineStyle.Automatic ?
                    this.ExtrapolationLineStyle :
                    this.defaultExtrapolationLineStyle;
            }
        }

        public bool IgnoreExtraplotationForScaling { get; set; }
        public IList<DataRange> Intervals { get; } = new List<DataRange>();
        protected double[] ActualExtrapolationDashArray
        {
            get
            {
                return this.ExtrapolationDashes ?? this.ActualExtrapolationLineStyle.GetDashArray();
            }
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            var xmid = (legendBox.Left + legendBox.Right) / 2;
            var ymid = (legendBox.Top + legendBox.Bottom) / 2;

            var pts = new[] { new ScreenPoint(legendBox.Left, ymid), new ScreenPoint(xmid, ymid) };

            rc.DrawLine(
                pts,
                this.GetSelectableColor(this.ActualColor),
                this.StrokeThickness,
                this.EdgeRenderingMode,
                this.ActualDashArray);

            pts = new[] { new ScreenPoint(xmid, ymid), new ScreenPoint(legendBox.Right, ymid) };

            rc.DrawLine(
                pts,
                this.GetSelectableColor(this.ActualExtrapolationColor),
                this.StrokeThickness,
                this.EdgeRenderingMode,
                this.ActualExtrapolationDashArray);

            var midpt = new ScreenPoint(xmid, ymid);

            rc.DrawMarker(
                midpt,
                this.MarkerType,
                this.MarkerOutline,
                this.MarkerSize,
                this.ActualMarkerFill,
                this.MarkerStroke,
                this.MarkerStrokeThickness,
                this.EdgeRenderingMode);
        }

        protected internal override void UpdateData()
        {
            base.UpdateData();

            this.orderedIntervals = this.MergeOverlaps(this.Intervals);
        }

        protected internal override void UpdateMaxMin()
        {
            if (this.IgnoreExtraplotationForScaling && this.orderedIntervals.Any())
            {
                this.MinX = this.Points
                    .Where(p => !this.InAnyInterval(p.X))
                    .Select(p => p.X)
                    .Where(x => !double.IsNaN(x))
                    .MinOrDefault(double.NaN);

                this.MinY = this.Points
                    .Where(p => !this.InAnyInterval(p.X))
                    .Select(p => p.Y)
                    .Where(y => !double.IsNaN(y))
                    .MinOrDefault(double.NaN);

                this.MaxX = this.Points
                    .Where(p => !this.InAnyInterval(p.X))
                    .Select(p => p.X)
                    .Where(x => !double.IsNaN(x))
                    .MaxOrDefault(double.NaN);

                this.MaxY = this.Points
                    .Where(p => !this.InAnyInterval(p.X))
                    .Select(p => p.Y)
                    .Where(y => !double.IsNaN(y))
                    .MaxOrDefault(double.NaN);
            }
            else
            {
                base.UpdateMaxMin();
            }
        }

        protected override void RenderLine(IRenderContext rc, IList<ScreenPoint> pointsToRender)
        {
            if (this.StrokeThickness <= 0 || this.ActualLineStyle == LineStyle.None)
            {
                return;
            }

            var clippingRect = this.GetClippingRect();

            var p1 = this.InverseTransform(clippingRect.BottomLeft);
            var p2 = this.InverseTransform(clippingRect.TopRight);

            var minX = Math.Min(p1.X, p2.X);
            var maxX = Math.Max(p1.X, p2.X);

            var minY = Math.Min(p1.Y, p2.Y);
            var maxY = Math.Max(p1.Y, p2.Y);

            var clippingRectangles = this.CreateClippingRectangles(clippingRect, minX, maxX, minY, maxY);

            foreach (var rect in clippingRectangles)
            {
                var centerX = this.InverseTransform(rect.Center).X;

                bool isInterval = this.orderedIntervals != null
                    && this.orderedIntervals.Any(i => i.Contains(centerX));

                using (rc.AutoResetClip(rect))
                {
                    this.RenderLinePart(rc, pointsToRender, isInterval);
                }
            }
        }

        private IEnumerable<OxyRect> CreateClippingRectangles(
            OxyRect clippingRect, double minX, double maxX, double minY, double maxY)
        {
            var previous = minX;

            if (this.orderedIntervals != null && this.orderedIntervals.Any())
            {
                IEnumerable<double> flatLimits
                    = this.Flatten(this.orderedIntervals).Where(l => l >= minX && l <= maxX);

                foreach (var limiter in flatLimits)
                {
                    yield return new OxyRect(
                        this.Transform(previous, minY),
                        this.Transform(limiter, maxY))
                        .Clip(clippingRect);

                    previous = limiter;
                }
            }

            yield return new OxyRect(
                this.Transform(previous, minY),
                this.Transform(maxX, maxY))
                .Clip(clippingRect);
        }

        private IEnumerable<double> Flatten(IEnumerable<DataRange> intervals)
        {
            foreach (var interval in intervals)
            {
                yield return interval.Minimum;
                yield return interval.Maximum;
            }
        }

        private void RenderLinePart(IRenderContext rc, IList<ScreenPoint> pointsToRender, bool isInterval)
        {
            OxyColor color = isInterval ? this.ExtrapolationColor : this.Color;

            var dashes = isInterval ?
                this.ActualExtrapolationDashArray :
                this.ActualDashArray;

            rc.DrawReducedLine(
                pointsToRender,
                this.MinimumSegmentLength * this.MinimumSegmentLength,
                this.GetSelectableColor(color),
                this.StrokeThickness,
                this.EdgeRenderingMode,
                dashes,
                this.LineJoin);
        }

        private List<DataRange> MergeOverlaps(IEnumerable<DataRange> intervals)
        {
            var orderedList = new List<DataRange>();

            if (intervals != null)
            {
                IOrderedEnumerable<DataRange> ordered = intervals.OrderBy(i => i.Minimum);

                foreach (var current in ordered)
                {
                    DataRange previous = orderedList.LastOrDefault();

                    if (current.IntersectsWith(previous))
                    {
                        orderedList[orderedList.Count - 1]
                            = new DataRange(previous.Minimum, Math.Max(previous.Maximum, current.Maximum));
                    }
                    else
                    {
                        orderedList.Add(current);
                    }
                }
            }

            return orderedList;
        }

        private bool InAnyInterval(double x)
        {
            var min = 0;
            var max = this.orderedIntervals.Count - 1;

            while (min <= max)
            {
                var mid = (min + max) / 2;
                var comparison = this.Compare(this.orderedIntervals[mid], x);

                if (comparison == 0)
                {
                    return true;
                }
                else if (comparison < 0)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            return false;
        }

        private int Compare(DataRange interval, double x)
        {
            if (x < interval.Minimum)
            {
                return -1;
            }

            if (x > interval.Maximum)
            {
                return 1;
            }

            return 0;
        }
    }
}
