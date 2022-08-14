namespace OxyPlot.Series
{
    using System;

    [Obsolete("use replacement CandleStickSeries instead")]
    public class OldCandleStickSeries : HighLowSeries
    {
        public OldCandleStickSeries()
        {
            this.CandleWidth = 10;
            this.IncreasingFill = OxyColors.Automatic;
            this.DecreasingFill = OxyColors.Undefined;
            this.ShadowEndColor = OxyColors.Undefined;
            this.ShadowEndLength = 1.0;
        }

        public double CandleWidth { get; set; }
        public OxyColor IncreasingFill { get; set; }
        public OxyColor DecreasingFill { get; set; }
        public OxyColor ShadowEndColor { get; set; }
        public double ShadowEndLength { get; set; }
        public OxyColor ActualIncreasingFill
        {
            get
            {
                return this.IncreasingFill.GetActualColor(this.ActualColor);
            }
        }

        public override void Render(IRenderContext rc)
        {
            if (this.IsTransposed())
            {
                throw new Exception("OldCandleStickSeries does not support transposed mode. It can only be used with horizontal X axis and vertical Y axis.");
            }

            if (this.Items.Count == 0)
            {
                return;
            }

            this.VerifyAxes();

            var dashArray = this.LineStyle.GetDashArray();
            var actualColor = this.GetSelectableColor(this.ActualColor);
            var shadowEndColor = this.GetSelectableColor(this.ShadowEndColor);

            if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
            {
                foreach (var v in this.Items)
                {
                    if (!this.IsValidItem(v, this.XAxis, this.YAxis))
                    {
                        continue;
                    }

                    if (v.X <= this.XAxis.ClipMinimum || v.X >= this.XAxis.ClipMaximum)
                    {
                        continue;
                    }

                    var high = this.Transform(v.X, v.High);
                    var low = this.Transform(v.X, v.Low);

                    if (double.IsNaN(v.Open) || double.IsNaN(v.Close))
                    {
                        rc.DrawLine(
                            new[] { low, high },
                            actualColor,
                            this.StrokeThickness,
                            this.EdgeRenderingMode,
                            dashArray,
                            this.LineJoin);
                    }
                    else
                    {
                        var open = this.Transform(v.X, v.Open);
                        var close = this.Transform(v.X, v.Close);
                        var max = new ScreenPoint(open.X, Math.Max(open.Y, close.Y));
                        var min = new ScreenPoint(open.X, Math.Min(open.Y, close.Y));

                        // Upper shadow
                        rc.DrawLine(
                            new[] { high, min },
                            actualColor,
                            this.StrokeThickness,
                            this.EdgeRenderingMode,
                            dashArray,
                            this.LineJoin);

                        // Lower shadow
                        rc.DrawLine(
                            new[] { max, low },
                            actualColor,
                            this.StrokeThickness,
                            this.EdgeRenderingMode,
                            dashArray,
                            this.LineJoin);

                        // Shadow ends
                        if (this.ShadowEndColor.IsVisible() && this.ShadowEndLength > 0)
                        {
                            var highLeft = new ScreenPoint(high.X - (this.CandleWidth * 0.5 * this.ShadowEndLength) - 1, high.Y);
                            var highRight = new ScreenPoint(high.X + (this.CandleWidth * 0.5 * this.ShadowEndLength), high.Y);
                            rc.DrawLine(
                                 new[] { highLeft, highRight },
                                 shadowEndColor,
                                 this.StrokeThickness,
                                 this.EdgeRenderingMode,
                                 dashArray,
                                 this.LineJoin);

                            var lowLeft = new ScreenPoint(low.X - (this.CandleWidth * 0.5 * this.ShadowEndLength) - 1, low.Y);
                            var lowRight = new ScreenPoint(low.X + (this.CandleWidth * 0.5 * this.ShadowEndLength), low.Y);
                            rc.DrawLine(
                                new[] { lowLeft, lowRight },
                                shadowEndColor,
                                this.StrokeThickness,
                                this.EdgeRenderingMode,
                                dashArray,
                                this.LineJoin);
                        }

                        // Body
                        var openLeft = open + new ScreenVector(-this.CandleWidth * 0.5, 0);
                        var rect = new OxyRect(openLeft.X, min.Y, this.CandleWidth, max.Y - min.Y);
                        var fillColor = v.Close > v.Open
                                            ? this.GetSelectableFillColor(this.ActualIncreasingFill)
                                            : this.GetSelectableFillColor(this.DecreasingFill);
                        rc.DrawRectangle(rect, fillColor, actualColor, this.StrokeThickness, this.EdgeRenderingMode);
                    }
                }
            }
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double yopen = legendBox.Top + ((legendBox.Bottom - legendBox.Top) * 0.7);
            double yclose = legendBox.Top + ((legendBox.Bottom - legendBox.Top) * 0.3);
            double[] dashArray = this.LineStyle.GetDashArray();

            if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
            {
                rc.DrawLine(
                    new[] { new ScreenPoint(xmid, legendBox.Top), new ScreenPoint(xmid, legendBox.Bottom) },
                    this.GetSelectableColor(this.ActualColor),
                    this.StrokeThickness,
                    this.EdgeRenderingMode,
                    dashArray,
                    LineJoin.Miter);

                // Shadow ends
                if (this.ShadowEndColor.IsVisible() && this.ShadowEndLength > 0)
                {
                    var highLeft = new ScreenPoint(xmid - (this.CandleWidth * 0.5 * this.ShadowEndLength) - 1, legendBox.Top);
                    var highRight = new ScreenPoint(xmid + (this.CandleWidth * 0.5 * this.ShadowEndLength), legendBox.Top);
                    rc.DrawLine(
                         new[] { highLeft, highRight },
                         this.GetSelectableColor(this.ShadowEndColor),
                         this.StrokeThickness,
                         this.EdgeRenderingMode,
                         dashArray,
                         this.LineJoin);

                    var lowLeft = new ScreenPoint(xmid - (this.CandleWidth * 0.5 * this.ShadowEndLength) - 1, legendBox.Bottom);
                    var lowRight = new ScreenPoint(xmid + (this.CandleWidth * 0.5 * this.ShadowEndLength), legendBox.Bottom);
                    rc.DrawLine(
                        new[] { lowLeft, lowRight },
                        this.GetSelectableColor(this.ShadowEndColor),
                        this.StrokeThickness,
                        this.EdgeRenderingMode,
                        dashArray,
                        this.LineJoin);
                }
            }

            rc.DrawRectangle(
                new OxyRect(xmid - (this.CandleWidth * 0.5), yclose, this.CandleWidth, yopen - yclose),
                this.GetSelectableFillColor(this.ActualIncreasingFill),
                this.GetSelectableColor(this.ActualColor),
                this.StrokeThickness,
                this.EdgeRenderingMode);
        }
    }
}
