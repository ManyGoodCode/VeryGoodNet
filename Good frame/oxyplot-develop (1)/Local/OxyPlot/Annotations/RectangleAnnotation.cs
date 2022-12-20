

namespace OxyPlot.Annotations
{
    using System;
    public class RectangleAnnotation : ShapeAnnotation
    {
        private OxyRect screenRectangle;
        public RectangleAnnotation()
        {
            this.MinimumX = double.NegativeInfinity;
            this.MaximumX = double.PositiveInfinity;
            this.MinimumY = double.NegativeInfinity;
            this.MaximumY = double.PositiveInfinity;
            this.TextRotation = 0;
            this.TextHorizontalAlignment = HorizontalAlignment.Center;
            this.TextVerticalAlignment = VerticalAlignment.Middle;
        }

        public double MinimumX { get; set; }
        public double MaximumX { get; set; }
        public double MinimumY { get; set; }
        public double MaximumY { get; set; }

        public override void Render(IRenderContext rc)
        {
            base.Render(rc);

            var clippingRectangle = this.GetClippingRect();

            var p1 = this.InverseTransform(clippingRectangle.TopLeft);
            var p2 = this.InverseTransform(clippingRectangle.BottomRight);

            var x1 = double.IsNegativeInfinity(this.MinimumX) || double.IsNaN(this.MinimumX) ? Math.Min(p1.X, p2.X) : this.MinimumX;
            var x2 = double.IsPositiveInfinity(this.MaximumX) || double.IsNaN(this.MaximumX) ? Math.Max(p1.X, p2.X) : this.MaximumX;
            var y1 = double.IsNegativeInfinity(this.MinimumY) || double.IsNaN(this.MinimumY) ? Math.Min(p1.Y, p2.Y) : this.MinimumY;
            var y2 = double.IsPositiveInfinity(this.MaximumY) || double.IsNaN(this.MaximumY) ? Math.Max(p1.Y, p2.Y) : this.MaximumY;

            this.screenRectangle = new OxyRect(this.Transform(x1, y1), this.Transform(x2, y2));
            
            rc.DrawRectangle(
                this.screenRectangle,
                this.GetSelectableFillColor(this.Fill),
                this.GetSelectableColor(this.Stroke),
                this.StrokeThickness,
                this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));

            if (string.IsNullOrEmpty(this.Text))
            {
                return;
            }

            this.GetActualTextAlignment(out var ha, out var va);
            var textPosition = this.GetActualTextPosition(() => this.screenRectangle.Center);
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

        protected override HitTestResult HitTestOverride(HitTestArguments args)
        {
            if (this.screenRectangle.Contains(args.Point))
            {
                return new HitTestResult(this, args.Point);
            }

            return null;
        }
    }
}
