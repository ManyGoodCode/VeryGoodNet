namespace OxyPlot.Annotations
{
    using System;
    using System.Collections.Generic;
    using OxyPlot;

    public abstract class PathAnnotation : TextualAnnotation
    {
        private IList<ScreenPoint> screenPoints;
        protected PathAnnotation()
        {
            this.MinimumX = double.MinValue;
            this.MaximumX = double.MaxValue;
            this.MinimumY = double.MinValue;
            this.MaximumY = double.MaxValue;
            this.Color = OxyColors.Blue;
            this.StrokeThickness = 1;
            this.LineStyle = LineStyle.Dash;
            this.LineJoin = LineJoin.Miter;

            this.TextLinePosition = 1;
            this.TextOrientation = AnnotationTextOrientation.AlongLine;
            this.TextMargin = 12;
            this.TextHorizontalAlignment = HorizontalAlignment.Right;
            this.TextVerticalAlignment = VerticalAlignment.Top;
            this.MinimumSegmentLength = 2;
        }

        public OxyColor Color { get; set; }

        public LineJoin LineJoin { get; set; }

        public LineStyle LineStyle { get; set; }
        public double MaximumX { get; set; }
        public double MaximumY { get; set; }
        public double MinimumX { get; set; }
        public double MinimumY { get; set; }
        public double StrokeThickness { get; set; }
        public double TextMargin { get; set; }
        public double TextPadding { get; set; }
        public AnnotationTextOrientation TextOrientation { get; set; }
        public double TextLinePosition { get; set; }
        public double MinimumSegmentLength { get; set; }
        protected double ActualMinimumX { get; set; }
        protected double ActualMinimumY { get; set; }
        protected double ActualMaximumX { get; set; }
        protected double ActualMaximumY { get; set; }

        public override void Render(IRenderContext rc)
        {
            base.Render(rc);

            this.CalculateActualMinimumsMaximums();

            if (this.ActualMinimumX > this.ActualMaximumX || this.ActualMinimumY > this.ActualMaximumY)
            {
                return;
            }

            this.screenPoints = this.GetScreenPoints();

            List<ScreenPoint> clippedPoints = new List<ScreenPoint>();
            double[] dashArray = this.LineStyle.GetDashArray();

            if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
            {
                rc.DrawReducedLine(
                   this.screenPoints,
                   this.MinimumSegmentLength * this.MinimumSegmentLength,
                   this.GetSelectableColor(this.Color),
                   this.StrokeThickness,
                   this.EdgeRenderingMode,
                   dashArray,
                   this.LineJoin,
                   null,
                   clippedPoints.AddRange);
            }

            double margin = this.TextMargin;
            this.GetActualTextAlignment(out var ha, out var va);

            double effectiveTextLinePosition = this.IsTransposed()
                ? (this.YAxis.IsReversed ? 1 - this.TextLinePosition : this.TextLinePosition)
                : (this.XAxis.IsReversed ? 1 - this.TextLinePosition : this.TextLinePosition);

            if (ha == HorizontalAlignment.Center)
            {
                margin = 0;
            }
            else
            {
                margin *= effectiveTextLinePosition < 0.5 ? 1 : -1;
            }

            if (GetPointAtRelativeDistance(clippedPoints, effectiveTextLinePosition, margin, out var position, out var angle))
            {
                if (angle < -90)
                {
                    angle += 180;
                }

                if (angle >= 90)
                {
                    angle -= 180;
                }

                switch (this.TextOrientation)
                {
                    case AnnotationTextOrientation.Horizontal:
                        angle = 0;
                        break;
                    case AnnotationTextOrientation.Vertical:
                        angle = -90;
                        break;
                }

                double angleInRadians = angle / 180 * Math.PI;
                int f = 1;

                if (ha == HorizontalAlignment.Right)
                {
                    f = -1;
                }

                if (ha == HorizontalAlignment.Center)
                {
                    f = 0;
                }

                position += new ScreenVector(f * this.TextPadding * Math.Cos(angleInRadians), f * this.TextPadding * Math.Sin(angleInRadians));

                if (!string.IsNullOrEmpty(this.Text))
                {
                    var textPosition = this.GetActualTextPosition(() => position);

                    if (this.TextPosition.IsDefined())
                    {
                        angle = this.TextRotation;
                    }

                    rc.DrawText(
                        textPosition,
                        this.Text,
                        this.ActualTextColor,
                        this.ActualFont,
                        this.ActualFontSize,
                        this.ActualFontWeight,
                        angle,
                        ha,
                        va);
                }
            }
        }

        protected override HitTestResult HitTestOverride(HitTestArguments args)
        {
            if (this.screenPoints == null)
            {
                return null;
            }

            ScreenPoint nearestPoint = ScreenPointHelper.FindNearestPointOnPolyline(args.Point, this.screenPoints);
            double dist = (args.Point - nearestPoint).Length;
            if (dist < args.Tolerance)
            {
                return new HitTestResult(this, nearestPoint);
            }

            return null;
        }

        protected abstract IList<ScreenPoint> GetScreenPoints();

        protected virtual void CalculateActualMinimumsMaximums()
        {
            this.ActualMinimumX = Math.Max(this.MinimumX, this.XAxis.ClipMinimum);
            this.ActualMaximumX = Math.Min(this.MaximumX, this.XAxis.ClipMaximum);
            this.ActualMinimumY = Math.Max(this.MinimumY, this.YAxis.ClipMinimum);
            this.ActualMaximumY = Math.Min(this.MaximumY, this.YAxis.ClipMaximum);

            var topLeft = this.InverseTransform(this.PlotModel.PlotArea.TopLeft);
            var bottomRight = this.InverseTransform(this.PlotModel.PlotArea.BottomRight);

            if (!this.ClipByXAxis)
            {
                this.ActualMaximumX = Math.Max(topLeft.X, bottomRight.X);
                this.ActualMinimumX = Math.Min(topLeft.X, bottomRight.X);
            }

            if (!this.ClipByYAxis)
            {
                this.ActualMaximumY = Math.Max(topLeft.Y, bottomRight.Y);
                this.ActualMinimumY = Math.Min(topLeft.Y, bottomRight.Y);
            }
        }

        private static bool GetPointAtRelativeDistance(
            IList<ScreenPoint> pts, double p, double margin, out ScreenPoint position, out double angle)
        {
            if (pts == null || pts.Count == 0)
            {
                position = new ScreenPoint();
                angle = 0;
                return false;
            }

            double length = 0;
            for (int i = 1; i < pts.Count; i++)
            {
                length += (pts[i] - pts[i - 1]).Length;
            }

            double l = (length * p) + margin;
            double eps = 1e-8;
            length = 0;
            for (int i = 1; i < pts.Count; i++)
            {
                double dl = (pts[i] - pts[i - 1]).Length;
                if (l >= length - eps && l <= length + dl + eps)
                {
                    double f = (l - length) / dl;
                    double x = (pts[i].X * f) + (pts[i - 1].X * (1 - f));
                    double y = (pts[i].Y * f) + (pts[i - 1].Y * (1 - f));
                    position = new ScreenPoint(x, y);
                    double dx = pts[i].X - pts[i - 1].X;
                    double dy = pts[i].Y - pts[i - 1].Y;
                    angle = Math.Atan2(dy, dx) / Math.PI * 180;
                    return true;
                }

                length += dl;
            }

            position = pts[0];
            angle = 0;
            return false;
        }
    }
}
