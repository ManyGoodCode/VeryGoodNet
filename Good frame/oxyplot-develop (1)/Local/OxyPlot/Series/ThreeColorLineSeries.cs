namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;

    public class ThreeColorLineSeries : LineSeries
    {
        private OxyColor defaultColorLo;
        private OxyColor defaultColorHi;
        public ThreeColorLineSeries()
        {
            this.LimitLo = -5.0;
            this.ColorLo = OxyColors.Blue;
            this.LineStyleLo = LineStyle.Solid;
            this.LimitHi = 5.0;
            this.ColorHi = OxyColors.Red;
            this.LineStyleHi = LineStyle.Solid;
        }


        public OxyColor ColorLo { get; set; }
        public OxyColor ColorHi { get; set; }
        public OxyColor ActualColorLo
        {
            get { return this.ColorLo.GetActualColor(this.defaultColorLo); }
        }

        public OxyColor ActualColorHi
        {
            get { return this.ColorHi.GetActualColor(this.defaultColorHi); }
        }


        public double LimitHi { get; set; }
        public double LimitLo { get; set; }
        public double[] DashesHi { get; set; }
        public double[] DashesLo { get; set; }
        public LineStyle LineStyleHi { get; set; }
        public LineStyle LineStyleLo { get; set; }
        public LineStyle ActualLineStyleHi
        {
            get
            {
                return this.LineStyleHi != LineStyle.Automatic ? this.LineStyleHi : LineStyle.Solid;
            }
        }

        public LineStyle ActualLineStyleLo
        {
            get
            {
                return this.LineStyleLo != LineStyle.Automatic ? this.LineStyleLo : LineStyle.Solid;
            }
        }

        protected double[] ActualDashArrayHi
        {
            get
            {
                return this.DashesHi ?? this.ActualLineStyleHi.GetDashArray();
            }
        }

        protected double[] ActualDashArrayLo
        {
            get
            {
                return this.DashesLo ?? this.ActualLineStyleLo.GetDashArray();
            }
        }

        protected internal override void SetDefaultValues()
        {
            base.SetDefaultValues();

            if (this.ColorLo.IsAutomatic())
            {
                this.defaultColorLo = this.PlotModel.GetDefaultColor();
            }

            if (this.LineStyleLo == LineStyle.Automatic)
            {
                this.LineStyleLo = this.PlotModel.GetDefaultLineStyle();
            }

            if (this.ColorHi.IsAutomatic())
            {
                this.defaultColorHi = this.PlotModel.GetDefaultColor();
            }

            if (this.LineStyleHi == LineStyle.Automatic)
            {
                this.LineStyleHi = this.PlotModel.GetDefaultLineStyle();
            }
        }

        protected override void RenderLine(IRenderContext rc, IList<ScreenPoint> pointsToRender)
        {
            var clippingRect = this.GetClippingRect();
            var p1 = this.InverseTransform(clippingRect.BottomLeft);
            var p2 = this.InverseTransform(clippingRect.TopRight);

            var clippingRectLo = new OxyRect(
                this.Transform(p1.X, Math.Min(p1.Y, p2.Y)),
                this.Transform(p2.X, this.LimitLo)).Clip(clippingRect);

            var clippingRectMid = new OxyRect(
                this.Transform(p1.X, this.LimitLo),
                this.Transform(p2.X, this.LimitHi)).Clip(clippingRect);

            var clippingRectHi = new OxyRect(
                this.Transform(p1.X, Math.Max(p1.Y, p2.Y)),
                this.Transform(p2.X, this.LimitHi)).Clip(clippingRect);

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

            using (rc.AutoResetClip(clippingRectMid))
            {
                RenderLine(this.ActualColor);
            }

            using (rc.AutoResetClip(clippingRectLo))
            {
                RenderLine(this.ActualColorLo);
            }

            using (rc.AutoResetClip(clippingRectHi))
            {
                RenderLine(this.ActualColorHi);
            }
        }
    }
}
