namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;

    public class TwoColorLineSeries : LineSeries
    {
        private OxyColor defaultColor2;
        public TwoColorLineSeries()
        {
            this.Limit = 0.0;
            this.Color2 = OxyColors.Blue;
            this.LineStyle2 = LineStyle.Solid;
        }

        public OxyColor Color2 { get; set; }

        public OxyColor ActualColor2
        {
            get { return this.Color2.GetActualColor(this.defaultColor2); }
        }

        public double Limit { get; set; }

        public double[] Dashes2 { get; set; }

        public LineStyle LineStyle2 { get; set; }

        public LineStyle ActualLineStyle2
        {
            get
            {
                return this.LineStyle2 != LineStyle.Automatic ? this.LineStyle2 : LineStyle.Solid;
            }
        }

        protected double[] ActualDashArray2
        {
            get
            {
                return this.Dashes2 ?? this.ActualLineStyle2.GetDashArray();
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

        protected override void RenderLine(IRenderContext rc, IList<ScreenPoint> pointsToRender)
        {
            var clippingRect = this.GetClippingRect();
            var p1 = this.InverseTransform(clippingRect.BottomLeft);
            var p2 = this.InverseTransform(clippingRect.TopRight);
            
            var clippingRectLo = new OxyRect(
                this.Transform(p1.X, Math.Min(p1.Y, p2.Y)),
                this.Transform(p2.X, this.Limit)).Clip(clippingRect);

            var clippingRectHi = new OxyRect(
                this.Transform(p1.X, Math.Max(p1.Y, p2.Y)),
                this.Transform(p2.X, this.Limit)).Clip(clippingRect);

            if (this.StrokeThickness <= 0 || this.ActualLineStyle == LineStyle.None)
            {
                return;
            }

            void RenderLine(OxyColor color)
            {
                rc.DrawReducedLine(
                    pointsToRender,
                    this.MinimumSegmentLength * this.MinimumSegmentLength,
                    this.GetSelectableColor(color),
                    this.StrokeThickness,
                    this.EdgeRenderingMode,
                    this.ActualDashArray,
                    this.LineJoin);
            }

            using (rc.AutoResetClip(clippingRectLo))
            {
                RenderLine(this.ActualColor);
            }

            using (rc.AutoResetClip(clippingRectHi))
            {
                RenderLine(this.ActualColor2);
            }
        }
    }
}
