
namespace OxyPlot.Annotations
{
    using System;
    using System.Collections.Generic;

    public class TextAnnotation : TextualAnnotation
    {
        private IList<ScreenPoint> actualBounds;
        public TextAnnotation()
        {
            this.Stroke = OxyColors.Black;
            this.Background = OxyColors.Undefined;
            this.StrokeThickness = 1;
            this.TextVerticalAlignment = VerticalAlignment.Bottom;
            this.Padding = new OxyThickness(4);
        }

        public OxyColor Background { get; set; }
        public ScreenVector Offset { get; set; }
        public OxyThickness Padding { get; set; }
        public OxyColor Stroke { get; set; }
        public double StrokeThickness { get; set; }
        public override void Render(IRenderContext rc)
        {
            base.Render(rc);

            var position = this.Transform(this.TextPosition) + this.Orientate(this.Offset);

            var textSize = rc.MeasureText(this.Text, this.ActualFont, this.ActualFontSize, this.ActualFontWeight);
            this.GetActualTextAlignment(out var ha, out var va);

            this.actualBounds = GetTextBounds(position, textSize, this.Padding, this.TextRotation, ha, va);

            if ((this.TextRotation % 90).Equals(0))
            {
                var actualRect = new OxyRect(this.actualBounds[0], this.actualBounds[2]);
                rc.DrawRectangle(actualRect, this.Background, this.Stroke, this.StrokeThickness, this.EdgeRenderingMode);
            }
            else
            {
                rc.DrawPolygon(this.actualBounds, this.Background, this.Stroke, this.StrokeThickness, this.EdgeRenderingMode);
            }

            rc.DrawMathText(
                position,
                this.Text,
                this.GetSelectableFillColor(this.ActualTextColor),
                this.ActualFont,
                this.ActualFontSize,
                this.ActualFontWeight,
                this.TextRotation,
                ha,
                va);
        }

        protected override HitTestResult HitTestOverride(HitTestArguments args)
        {
            if (this.actualBounds == null)
            {
                return null;
            }

            return ScreenPointHelper.IsPointInPolygon(args.Point, this.actualBounds) ? new HitTestResult(this, args.Point) : null;
        }

        private static IList<ScreenPoint> GetTextBounds(
            ScreenPoint position,
            OxySize size,
            OxyThickness padding,
            double rotation,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment)
        {
            double left, right, top, bottom;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    left = -size.Width * 0.5;
                    right = -left;
                    break;
                case HorizontalAlignment.Right:
                    left = -size.Width;
                    right = 0;
                    break;
                default:
                    left = 0;
                    right = size.Width;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Middle:
                    top = -size.Height * 0.5;
                    bottom = -top;
                    break;
                case VerticalAlignment.Bottom:
                    top = -size.Height;
                    bottom = 0;
                    break;
                default:
                    top = 0;
                    bottom = size.Height;
                    break;
            }

            double cost = Math.Cos(rotation / 180 * Math.PI);
            double sint = Math.Sin(rotation / 180 * Math.PI);
            var u = new ScreenVector(cost, sint);
            var v = new ScreenVector(-sint, cost);
            var polygon = new ScreenPoint[4];
            polygon[0] = position + (u * (left - padding.Left)) + (v * (top - padding.Top));
            polygon[1] = position + (u * (right + padding.Right)) + (v * (top - padding.Top));
            polygon[2] = position + (u * (right + padding.Right)) + (v * (bottom + padding.Bottom));
            polygon[3] = position + (u * (left - padding.Left)) + (v * (bottom + padding.Bottom));
            return polygon;
        }
    }
}
