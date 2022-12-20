namespace OxyPlot.Annotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PolygonAnnotation : ShapeAnnotation
    {
        private IList<ScreenPoint> screenPoints;
        public PolygonAnnotation()
        {
            this.LineStyle = LineStyle.Solid;
            this.LineJoin = LineJoin.Miter;
            this.MinimumSegmentLength = 2;

            this.Points = new List<DataPoint>();
        }

        public LineJoin LineJoin { get; set; }
        public LineStyle LineStyle { get; set; }
        public double MinimumSegmentLength { get; set; }
        public List<DataPoint> Points { get; private set; }
        
        public override void Render(IRenderContext rc)
        {
            base.Render(rc);
            if (this.Points == null)
            {
                return;
            }

            this.screenPoints = this.Points.Select(this.Transform).ToList();
            if (this.screenPoints.Count == 0)
            {
                return;
            }

            rc.DrawReducedPolygon(
                this.screenPoints,
                this.MinimumSegmentLength * this.MinimumSegmentLength,
                this.GetSelectableFillColor(this.Fill),
                this.GetSelectableColor(this.Stroke),
                this.StrokeThickness,
                this.EdgeRenderingMode,
                this.LineStyle,
                this.LineJoin);

            if (!string.IsNullOrEmpty(this.Text))
            {
                this.GetActualTextAlignment(out var ha, out var va);
                var textPosition = this.GetActualTextPosition(() => ScreenPointHelper.GetCentroid(this.screenPoints));

                rc.DrawText(
                    textPosition,
                    this.Text,
                    this.ActualTextColor,
                    this.ActualFont,
                    this.ActualFontSize,
                    this.ActualFontWeight,
                    this.TextRotation,
                    ha,
                    va);
            }
        }

        protected override HitTestResult HitTestOverride(HitTestArguments args)
        {
            if (this.screenPoints == null)
            {
                return null;
            }

            return ScreenPointHelper.IsPointInPolygon(args.Point, this.screenPoints) ? new HitTestResult(this, args.Point) : null;
        }
    }
}
