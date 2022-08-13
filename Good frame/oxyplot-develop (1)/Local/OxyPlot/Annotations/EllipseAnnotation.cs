namespace OxyPlot.Annotations
{
    public class EllipseAnnotation : ShapeAnnotation
    {
        private OxyRect screenRectangle;

        public EllipseAnnotation()
        {
            this.Width = double.NaN;
            this.Height = double.NaN;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }

        public double Height { get; set; }

        public override void Render(IRenderContext rc)
        {
            base.Render(rc);

            this.screenRectangle = new OxyRect(this.Transform(this.X - (this.Width / 2), this.Y - (this.Height / 2)), this.Transform(this.X + (this.Width / 2), this.Y + (this.Height / 2)));

            rc.DrawEllipse(
                this.screenRectangle,
                this.GetSelectableFillColor(this.Fill),
                this.GetSelectableColor(this.Stroke),
                this.StrokeThickness,
                this.EdgeRenderingMode);

            if (!string.IsNullOrEmpty(this.Text))
            {
                ScreenPoint textPosition = this.GetActualTextPosition(() => this.screenRectangle.Center);
                this.GetActualTextAlignment(out var ha, out var va);
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
            if (this.screenRectangle.Contains(args.Point))
            {
                return new HitTestResult(this, args.Point);
            }

            return null;
        }
    }
}
