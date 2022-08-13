namespace OxyPlot.Annotations
{
    using System;

    /// <summary>
    /// 表示显示箭头的注释
    /// </summary>
    public class ArrowAnnotation : TextualAnnotation
    {
        /// <summary>
        /// 屏幕坐标中的终点
        /// </summary>
        private ScreenPoint screenEndPoint;

        /// <summary>
        /// 屏幕坐标中的起点
        /// </summary>
        private ScreenPoint screenStartPoint;
        public ArrowAnnotation()
        {
            this.HeadLength = 10;
            this.HeadWidth = 3;
            this.Color = OxyColors.Blue;
            this.StrokeThickness = 2;
            this.LineStyle = LineStyle.Solid;
            this.LineJoin = LineJoin.Miter;
        }

        public ScreenVector ArrowDirection { get; set; }
        public OxyColor Color { get; set; }
        public DataPoint EndPoint { get; set; }
        public double HeadLength { get; set; }
        public double HeadWidth { get; set; }
        public LineJoin LineJoin { get; set; }
        public LineStyle LineStyle { get; set; }
        public DataPoint StartPoint { get; set; }
        public double StrokeThickness { get; set; }
        public double Veeness { get; set; }

        public override void Render(IRenderContext rc)
        {
            base.Render(rc);

            this.screenEndPoint = this.Transform(this.EndPoint);

            if (this.ArrowDirection.LengthSquared > 0)
            {
                this.screenStartPoint = this.screenEndPoint - this.Orientate(this.ArrowDirection);
            }
            else
            {
                this.screenStartPoint = this.Transform(this.StartPoint);
            }

            ScreenVector d = this.screenEndPoint - this.screenStartPoint;
            d.Normalize();
            ScreenVector n = new ScreenVector(d.Y, -d.X);

            ScreenPoint p1 = this.screenEndPoint - (d * this.HeadLength * this.StrokeThickness);
            ScreenPoint p2 = p1 + (n * this.HeadWidth * this.StrokeThickness);
            ScreenPoint p3 = p1 - (n * this.HeadWidth * this.StrokeThickness);
            ScreenPoint p4 = p1 + (d * this.Veeness * this.StrokeThickness);

            const double MinimumSegmentLength = 0;

            double[] dashArray = this.LineStyle.GetDashArray();

            if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
            {
                rc.DrawReducedLine(
                    new[] { this.screenStartPoint, p4 },
                    MinimumSegmentLength * MinimumSegmentLength,
                    this.GetSelectableColor(this.Color),
                    this.StrokeThickness,
                    this.EdgeRenderingMode,
                    dashArray,
                    this.LineJoin);

                rc.DrawReducedPolygon(
                    new[] { p3, this.screenEndPoint, p2, p4 },
                    MinimumSegmentLength * MinimumSegmentLength,
                    this.GetSelectableColor(this.Color),
                    OxyColors.Undefined,
                    0,
                    this.EdgeRenderingMode);
            }

            if (string.IsNullOrEmpty(this.Text))
            {
                return;
            }

            HorizontalAlignment ha;
            VerticalAlignment va;

            if (this.TextPosition.IsDefined())
            {
                this.GetActualTextAlignment(out ha, out va);
            }
            else
            {
                double angle = Math.Atan2(d.Y, d.X);
                double piOver8 = Math.PI / 8;
                if (angle < 3 * piOver8 && angle > -3 * piOver8)
                {
                    ha = HorizontalAlignment.Right;
                }
                else if (angle > 5 * piOver8 || angle < -5 * piOver8)
                {
                    ha = HorizontalAlignment.Left;
                }
                else
                {
                    ha = HorizontalAlignment.Center;
                }

                if (angle > piOver8 && angle < 7 * piOver8)
                {
                    va = VerticalAlignment.Bottom;
                }
                else if (angle < -piOver8 && angle > -7 * piOver8)
                {
                    va = VerticalAlignment.Top;
                }
                else
                {
                    va = VerticalAlignment.Middle;
                }
            }

            ScreenPoint textPoint = this.GetActualTextPosition(() => this.screenStartPoint);
            rc.DrawText(
                textPoint,
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
            if ((args.Point - this.screenStartPoint).Length < args.Tolerance)
            {
                return new HitTestResult(this, this.screenStartPoint, null, 1);
            }

            if ((args.Point - this.screenEndPoint).Length < args.Tolerance)
            {
                return new HitTestResult(this, this.screenEndPoint, null, 2);
            }

            ScreenPoint p = ScreenPointHelper.FindPointOnLine(args.Point, this.screenStartPoint, this.screenEndPoint);
            if ((p - args.Point).Length < args.Tolerance)
            {
                return new HitTestResult(this, p);
            }

            return null;
        }
    }
}
