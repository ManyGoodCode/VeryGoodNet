namespace OxyPlot.Annotations
{
    public class PointAnnotation : ShapeAnnotation
    {
        private ScreenPoint screenPosition;

        public PointAnnotation()
        {
            this.Size = 4;
            this.TextMargin = 2;
            this.Shape = MarkerType.Circle;
            this.TextVerticalAlignment = VerticalAlignment.Top;
        }
        
        public double X { get; set; }
        public double Y { get; set; }
        public double Size { get; set; }
        public double TextMargin { get; set; }
        public MarkerType Shape { get; set; }

        public ScreenPoint[] CustomOutline { get; set; }
        public override void Render(IRenderContext rc)
        {
            base.Render(rc);

            this.screenPosition = this.Transform(this.X, this.Y);

            rc.DrawMarker(this.screenPosition, this.Shape, this.CustomOutline, this.Size, this.Fill, this.Stroke, this.StrokeThickness, this.EdgeRenderingMode);

            if (!string.IsNullOrEmpty(this.Text))
            {
                this.GetActualTextAlignment(out var ha, out var va);
                var dx = -(int)ha * (this.Size + this.TextMargin);
                var dy = -(int)va * (this.Size + this.TextMargin);
                var textPosition = this.screenPosition + new ScreenVector(dx, dy);
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
            if (this.screenPosition.DistanceTo(args.Point) < this.Size)
            {
                return new HitTestResult(this, this.screenPosition);
            }

            return null;
        }
    }
}
