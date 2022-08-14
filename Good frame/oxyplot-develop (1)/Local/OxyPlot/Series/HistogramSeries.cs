namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class HistogramSeries : XYAxisSeries
    {
        public new const string DefaultTrackerFormatString = "Start: {5}\nEnd: {6}\nValue: {7}\nArea: {8}\nCount: {9}";
        private OxyColor defaultFillColor;
        private List<HistogramItem> actualItems;
        private bool ownsActualItems;
        public HistogramSeries()
        {
            this.FillColor = OxyColors.Automatic;
            this.StrokeColor = OxyColors.Black;
            this.StrokeThickness = 0;
            this.TrackerFormatString = DefaultTrackerFormatString;
            this.LabelFormatString = null;
            this.LabelPlacement = LabelPlacement.Outside;
            this.ColorMapping = this.GetDefaultColor;
        }


        public OxyColor FillColor { get; set; }
        public OxyColor ActualFillColor => this.FillColor.GetActualColor(this.defaultFillColor);
        public OxyColor StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public string LabelFormatString { get; set; }
        public double LabelMargin { get; set; }
        public LabelPlacement LabelPlacement { get; set; }
        public Func<HistogramItem, OxyColor> ColorMapping { get; set; }
        public Func<object, HistogramItem> Mapping { get; set; }
        public List<HistogramItem> Items { get; } = new List<HistogramItem>();
        protected List<HistogramItem> ActualItems => this.ItemsSource != null ? this.actualItems : this.Items;
        public override void Render(IRenderContext rc)
        {
            this.VerifyAxes();
            this.RenderBins(rc, this.ActualItems);
        }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            var p = this.InverseTransform(point);

            if (!this.IsPointInRange(p))
            {
                return null;
            }

            if (this.ActualItems == null)
            {
                return null;
            }

            for (var i = 0; i < this.ActualItems.Count; i++)
            {
                var item = this.ActualItems[i];
                if (item.Contains(p))
                {
                    var itemsSourceItem = this.GetItem(i);
                    return new TrackerHitResult
                    {
                        Series = this,
                        DataPoint = p,
                        Position = point,
                        Item = itemsSourceItem,
                        Index = i,
                        Text = StringHelper.Format(
                            this.ActualCulture,
                            this.TrackerFormatString,
                            itemsSourceItem,
                            this.Title,
                            this.XAxis.Title ?? DefaultXAxisTitle,
                            this.XAxis.GetValue(p.X),
                            this.YAxis.Title ?? DefaultYAxisTitle,
                            this.YAxis.GetValue(p.Y),
                            item.RangeStart,
                            item.RangeEnd,
                            item.Value,
                            item.Area,
                            item.Count),
                    };
                }
            }

            return null;
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            var xmid = (legendBox.Left + legendBox.Right) / 2;
            var ymid = (legendBox.Top + legendBox.Bottom) / 2;
            var height = (legendBox.Bottom - legendBox.Top) * 0.8;
            var width = height;
            rc.DrawRectangle(
                new OxyRect(xmid - (0.5 * width), ymid - (0.5 * height), width, height),
                this.GetSelectableColor(this.ActualFillColor),
                this.StrokeColor,
                this.StrokeThickness,
                this.EdgeRenderingMode);
        }

        protected internal override void UpdateData()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            this.UpdateActualItems();
        }

        protected internal override void SetDefaultValues()
        {
            if (this.FillColor.IsAutomatic())
            {
                this.defaultFillColor = this.PlotModel.GetDefaultColor();
            }
        }

        protected internal void UpdateMaxMinXY()
        {
            if (this.ActualItems != null && this.ActualItems.Count > 0)
            {
                this.MinX = Math.Min(this.ActualItems.Min(r => r.RangeStart), this.ActualItems.Min(r => r.RangeEnd));
                this.MaxX = Math.Max(this.ActualItems.Max(r => r.RangeStart), this.ActualItems.Max(r => r.RangeEnd));
                this.MinY = Math.Min(this.ActualItems.Min(r => 0), this.ActualItems.Min(r => r.Height));
                this.MaxY = Math.Max(this.ActualItems.Max(r => 0), this.ActualItems.Max(r => r.Height));
            }
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();

            var allDataPoints = new List<DataPoint>();
            allDataPoints.AddRange(this.ActualItems.Select(item => new DataPoint(item.RangeStart, 0.0)));
            allDataPoints.AddRange(this.ActualItems.Select(item => new DataPoint(item.RangeEnd, item.Height)));
            this.InternalUpdateMaxMin(allDataPoints);

            this.UpdateMaxMinXY();

            if (this.ActualItems != null && this.ActualItems.Count > 0)
            {
                this.MinValue = this.ActualItems.Min(r => r.Value);
                this.MaxValue = this.ActualItems.Max(r => r.Value);
            }
        }


        protected override object GetItem(int i)
        {
            var items = this.ActualItems;
            if (this.ItemsSource == null && items != null && i < items.Count)
            {
                return items[i];
            }

            return base.GetItem(i);
        }

        protected void RenderBins(IRenderContext rc, ICollection<HistogramItem> items)
        {
            foreach (var item in items)
            {
                var actualFillColor = this.GetItemFillColor(item);

                // transform the data points to screen points
                var p1 = this.Transform(item.RangeStart, 0);
                var p2 = this.Transform(item.RangeEnd, item.Height);

                var rectrect = new OxyRect(p1, p2);

                rc.DrawRectangle(
                    rectrect, 
                    actualFillColor, 
                    this.StrokeColor, 
                    this.StrokeThickness, 
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));

                if (this.LabelFormatString != null)
                {
                    this.RenderLabel(rc, rectrect, item);
                }
            }
        }

        protected OxyColor GetItemFillColor(HistogramItem item)
        {
            return item.Color.IsAutomatic() ? this.ColorMapping(item) : item.Color;
        }

        protected void RenderLabel(IRenderContext rc, OxyRect rect, HistogramItem item)
        {
            var s = StringHelper.Format(this.ActualCulture, this.LabelFormatString, item, item.Value, item.RangeStart, item.RangeEnd, item.Area, item.Count);
            DataPoint dp;
            VerticalAlignment va;
            var ha = HorizontalAlignment.Center;

            var midX = (item.RangeStart + item.RangeEnd) / 2;
            var sign = Math.Sign(item.Value);
            var dy = sign * this.LabelMargin;

            switch (this.LabelPlacement)
            {
                case LabelPlacement.Inside:
                    dp = new DataPoint(midX, item.Value);
                    va = (VerticalAlignment)(-sign);
                    break;
                case LabelPlacement.Middle:
                    var p1 = this.InverseTransform(rect.TopLeft);
                    var p2 = this.InverseTransform(rect.BottomRight);
                    dp = new DataPoint(midX, (p1.Y + p2.Y) / 2);
                    va = VerticalAlignment.Middle;
                    break;
                case LabelPlacement.Base:
                    dp = new DataPoint(midX, 0);
                    dy = -dy;
                    va = (VerticalAlignment)sign;
                    break;
                case LabelPlacement.Outside:
                    dp = new DataPoint(midX, item.Value);
                    dy = -dy;
                    va = (VerticalAlignment)sign;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.Orientate(ref ha, ref va);
            var sp = this.Transform(dp) + this.Orientate(new ScreenVector(0, dy));

            rc.DrawText(
                sp,
                s,
                this.ActualTextColor,
                this.ActualFont,
                this.ActualFontSize,
                this.ActualFontWeight,
                0,
                ha,
                va);
        }

        private bool IsPointInRange(DataPoint p)
        {
            this.UpdateMaxMinXY();

            return p.X >= this.MinX && p.X <= this.MaxX && p.Y >= this.MinY && p.Y <= this.MaxY;
        }

        private void ClearActualItems()
        {
            if (!this.ownsActualItems || this.actualItems == null)
            {
                this.actualItems = new List<HistogramItem>();
            }
            else
            {
                this.actualItems.Clear();
            }

            this.ownsActualItems = true;
        }

        private OxyColor GetDefaultColor(HistogramItem item)
        {
            return this.ActualFillColor;
        }

        private void UpdateActualItems()
        {
            if (this.Mapping != null)
            {
                this.ClearActualItems();
                foreach (var item in this.ItemsSource)
                {
                    this.actualItems.Add(this.Mapping(item));
                }

                return;
            }

            if (this.ItemsSource is List<HistogramItem> sourceAsListOfHistogramItems)
            {
                this.actualItems = sourceAsListOfHistogramItems;
                this.ownsActualItems = false;
                return;
            }

            this.ClearActualItems();

            if (this.ItemsSource is IEnumerable<HistogramItem> sourceAsEnumerableHistogramItems)
            {
                this.actualItems.AddRange(sourceAsEnumerableHistogramItems);
            }
        }
    }
}
