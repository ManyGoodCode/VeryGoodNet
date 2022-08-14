namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;

    using OxyPlot.Axes;

    public class HighLowSeries : XYAxisSeries
    {
        public new const string DefaultTrackerFormatString = "{0}\n{1}: {2}\nHigh: {3:0.###}\nLow: {4:0.###}\nOpen: {5:0.###}\nClose: {6:0.###}";
        private readonly List<HighLowItem> items = new List<HighLowItem>();
        private OxyColor defaultColor;
        public HighLowSeries()
        {
            this.Color = OxyColors.Automatic;
            this.TickLength = 4;
            this.StrokeThickness = 1;
            this.TrackerFormatString = DefaultTrackerFormatString;
        }

        public OxyColor Color { get; set; }

        public OxyColor ActualColor
        {
            get { return this.Color.GetActualColor(this.defaultColor); }
        }

        public double[] Dashes { get; set; }

        public string DataFieldClose { get; set; }

        public string DataFieldHigh { get; set; }

        public string DataFieldLow { get; set; }

        public string DataFieldOpen { get; set; }
        public string DataFieldX { get; set; }
        public List<HighLowItem> Items
        {
            get
            {
                return this.items;
            }
        }

        public LineJoin LineJoin { get; set; }

        public LineStyle LineStyle { get; set; }

        public Func<object, HighLowItem> Mapping { get; set; }

        public double StrokeThickness { get; set; }
        public double TickLength { get; set; }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if (this.XAxis == null || this.YAxis == null)
            {
                return null;
            }

            if (interpolate)
            {
                return null;
            }

            double minimumDistance = double.MaxValue;

            TrackerHitResult result = null;
            Action<DataPoint, HighLowItem, int> check = (p, item, index) =>
                {
                    var sp = this.Transform(p);
                    double dx = sp.x - point.x;
                    double dy = sp.y - point.y;
                    double d2 = (dx * dx) + (dy * dy);

                    if (d2 < minimumDistance)
                    {
                        result = new TrackerHitResult
                        {
                            Series = this,
                            DataPoint = p,
                            Position = sp,
                            Item = item,
                            Index = index,
                            Text =
                                StringHelper.Format(
                                    this.ActualCulture,
                                    this.TrackerFormatString,
                                    item,
                                    this.Title,
                                    this.XAxis.Title ?? DefaultXAxisTitle,
                                    this.XAxis.GetValue(p.X),
                                    this.YAxis.GetValue(item.High),
                                    this.YAxis.GetValue(item.Low),
                                    this.YAxis.GetValue(item.Open),
                                    this.YAxis.GetValue(item.Close))
                        };

                        minimumDistance = d2;
                    }
                };
            int i = 0;
            foreach (var item in this.items)
            {
                check(new DataPoint(item.X, item.High), item, i);
                check(new DataPoint(item.X, item.Low), item, i);
                check(new DataPoint(item.X, item.Open), item, i);
                check(new DataPoint(item.X, item.Close), item, i++);
            }

            if (minimumDistance < double.MaxValue)
            {
                return result;
            }

            return null;
        }

        public virtual bool IsValidItem(HighLowItem pt, Axis xaxis, Axis yaxis)
        {
            return !double.IsNaN(pt.X) && !double.IsInfinity(pt.X) && !double.IsNaN(pt.High)
                   && !double.IsInfinity(pt.High) && !double.IsNaN(pt.Low) && !double.IsInfinity(pt.Low);
        }

        public override void Render(IRenderContext rc)
        {
            if (this.items.Count == 0)
            {
                return;
            }

            this.VerifyAxes();

            var dashArray = this.LineStyle.GetDashArray();
            var actualColor = this.GetSelectableColor(this.ActualColor);
            foreach (var v in this.items)
            {
                if (!this.IsValidItem(v, this.XAxis, this.YAxis))
                {
                    continue;
                }

                if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
                {
                    var high = this.Transform(v.X, v.High);
                    var low = this.Transform(v.X, v.Low);

                    rc.DrawLine(
                        new[] { low, high },
                        actualColor,
                        this.StrokeThickness,
                        this.EdgeRenderingMode,
                        dashArray,
                        this.LineJoin);

                    var tickVector = this.Orientate(new ScreenVector(this.TickLength, 0));
                    if (!double.IsNaN(v.Open))
                    {
                        var open = this.Transform(v.X, v.Open);
                        var openTick = open - tickVector;
                        rc.DrawLine(
                            new[] { open, openTick },
                            actualColor,
                            this.StrokeThickness,
                            this.EdgeRenderingMode,
                            dashArray,
                            this.LineJoin);
                    }

                    if (!double.IsNaN(v.Close))
                    {
                        var close = this.Transform(v.X, v.Close);
                        var closeTick = close + tickVector;
                        rc.DrawLine(
                            new[] { close, closeTick },
                            actualColor,
                            this.StrokeThickness,
                            this.EdgeRenderingMode,
                            dashArray,
                            this.LineJoin);
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
            var color = this.GetSelectableColor(this.ActualColor);

            if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
            {
                rc.DrawLine(
                    new[] { new ScreenPoint(xmid, legendBox.Top), new ScreenPoint(xmid, legendBox.Bottom) },
                    color,
                    this.StrokeThickness,
                    this.EdgeRenderingMode,
                    dashArray,
                    LineJoin.Miter);
                rc.DrawLine(
                    new[] { new ScreenPoint(xmid - this.TickLength, yopen), new ScreenPoint(xmid, yopen) },
                    color,
                    this.StrokeThickness,
                    this.EdgeRenderingMode,
                    dashArray,
                    LineJoin.Miter);
                rc.DrawLine(
                    new[] { new ScreenPoint(xmid + this.TickLength, yclose), new ScreenPoint(xmid, yclose) },
                    color,
                    this.StrokeThickness,
                    this.EdgeRenderingMode,
                    dashArray,
                    LineJoin.Miter);
            }
        }

        protected internal override void SetDefaultValues()
        {
            if (this.Color.IsAutomatic())
            {
                this.LineStyle = this.PlotModel.GetDefaultLineStyle();
                this.defaultColor = this.PlotModel.GetDefaultColor();
            }
        }

        protected internal override void UpdateData()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            this.items.Clear();

            if (this.Mapping != null)
            {
                foreach (var item in this.ItemsSource)
                {
                    this.items.Add(this.Mapping(item));
                }

                return;
            }

            var sequenceOfHighLowItems = this.ItemsSource as IEnumerable<HighLowItem>;
            if (sequenceOfHighLowItems != null)
            {
                this.items.AddRange(sequenceOfHighLowItems);
                return;
            }

            var filler = new ListBuilder<HighLowItem>();
            filler.Add(this.DataFieldX, double.NaN);
            filler.Add(this.DataFieldHigh, double.NaN);
            filler.Add(this.DataFieldLow, double.NaN);
            filler.Add(this.DataFieldOpen, double.NaN);
            filler.Add(this.DataFieldClose, double.NaN);
            filler.FillT(this.items, this.ItemsSource, args => new HighLowItem(Axis.ToDouble(args[0]), Convert.ToDouble(args[1]), Convert.ToDouble(args[2]), Convert.ToDouble(args[3]), Convert.ToDouble(args[4])));
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            this.InternalUpdateMaxMin(this.items, i => i.X, i => i.X, i => i.Low, i => i.High);
        }
    }
}
