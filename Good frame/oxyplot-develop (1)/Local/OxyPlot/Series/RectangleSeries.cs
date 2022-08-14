namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Axes;

    public class RectangleSeries : XYAxisSeries
    {
        private List<RectangleItem> actualItems;
        private bool ownsActualItems;
        public new const string DefaultTrackerFormatString = "{0}\n{1}: {2}\n{3}: {4}\n{5}: {6}";
        private const string DefaultColorAxisTitle = "Value";
        public RectangleSeries()
        {
            this.TrackerFormatString = DefaultTrackerFormatString;
            this.LabelFormatString = "0.00";
            this.LabelFontSize = 0;
        }

        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public IColorAxis ColorAxis { get; protected set; }
        public string ColorAxisKey { get; set; }
        public string LabelFormatString { get; set; }
        public double LabelFontSize { get; set; }
        public bool CanTrackerInterpolatePoints { get; set; }
        public Func<object, RectangleItem> Mapping { get; set; }
        public List<RectangleItem> Items { get; } = new List<RectangleItem>();
        protected List<RectangleItem> ActualItems => this.ItemsSource != null ? this.actualItems : this.Items;
        public override void Render(IRenderContext rc)
        {
            this.VerifyAxes();
            this.RenderRectangles(rc, this.ActualItems);
        }

        protected internal override void UpdateData()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            this.UpdateActualItems();
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

        private void ClearActualItems()
        {
            if (!this.ownsActualItems || this.actualItems == null)
            {
                this.actualItems = new List<RectangleItem>();
            }
            else
            {
                this.actualItems.Clear();
            }

            this.ownsActualItems = true;
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

            var sourceAsListOfDataRects = this.ItemsSource as List<RectangleItem>;
            if (sourceAsListOfDataRects != null)
            {
                this.actualItems = sourceAsListOfDataRects;
                this.ownsActualItems = false;
                return;
            }

            this.ClearActualItems();

            var sourceAsEnumerableDataRects = this.ItemsSource as IEnumerable<RectangleItem>;
            if (sourceAsEnumerableDataRects != null)
            {
                this.actualItems.AddRange(sourceAsEnumerableDataRects);
            }
        }


        protected void RenderRectangles(IRenderContext rc, ICollection<RectangleItem> items)
        {
            foreach (var item in items)
            {
                var rectcolor = this.ColorAxis.GetColor(item.Value);

                var p1 = this.Transform(item.A.X, item.A.Y);
                var p2 = this.Transform(item.B.X, item.B.Y);

                var rectrect = new OxyRect(p1, p2);

                rc.DrawRectangle(
                    rectrect, 
                    rectcolor, 
                    OxyColors.Undefined,
                    0, 
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));

                if (this.LabelFontSize > 0)
                {
                    rc.DrawText(
                        rectrect.Center, 
                        item.Value.ToString(this.LabelFormatString), 
                        this.ActualTextColor, 
                        this.ActualFont, 
                        this.LabelFontSize, 
                        this.ActualFontWeight, 
                        0, 
                        HorizontalAlignment.Center, 
                        VerticalAlignment.Middle);
                }
            }
        }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            var p = this.InverseTransform(point);

            if (!this.IsPointInRange(p))
            {
                return null;
            }

            var colorAxis = this.ColorAxis as Axis;
            var colorAxisTitle = colorAxis?.Title ?? DefaultColorAxisTitle;

            if (this.ActualItems != null)
            {
                foreach (var item in this.ActualItems)
                {
                    if (item.Contains(p))
                    {
                        return new TrackerHitResult
                        {
                            Series = this,
                            DataPoint = p,
                            Position = point,
                            Item = null,
                            Index = -1,
                            Text = StringHelper.Format(
                            this.ActualCulture,
                            this.TrackerFormatString,
                            item,
                            this.Title,
                            this.XAxis.Title ?? DefaultXAxisTitle,
                            this.XAxis.GetValue(p.X),
                            this.YAxis.Title ?? DefaultYAxisTitle,
                            this.YAxis.GetValue(p.Y),
                            colorAxisTitle,
                            item.Value)
                        };
                    }
                }
            }

            return null;
        }

        protected internal override void EnsureAxes()
        {
            base.EnsureAxes();

            this.ColorAxis = this.ColorAxisKey != null ?
                             this.PlotModel.GetAxis(this.ColorAxisKey) as IColorAxis :
                             this.PlotModel.DefaultColorAxis as IColorAxis;
        }

        protected internal void UpdateMaxMinXY()
        {
            if (this.ActualItems != null && this.ActualItems.Count > 0)
            {
                this.MinX = Math.Min(this.ActualItems.Min(r => r.A.X), this.ActualItems.Min(r => r.B.X));
                this.MaxX = Math.Max(this.ActualItems.Max(r => r.A.X), this.ActualItems.Max(r => r.B.X));
                this.MinY = Math.Min(this.ActualItems.Min(r => r.A.Y), this.ActualItems.Min(r => r.B.Y));
                this.MaxY = Math.Max(this.ActualItems.Max(r => r.A.Y), this.ActualItems.Max(r => r.B.Y));
            }
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();

            var allDataPoints = new List<DataPoint>();
            allDataPoints.AddRange(this.ActualItems.Select(rect => rect.A));
            allDataPoints.AddRange(this.ActualItems.Select(rect => rect.B));
            this.InternalUpdateMaxMin(allDataPoints);

            this.UpdateMaxMinXY();

            if (this.ActualItems != null && this.ActualItems.Count > 0)
            {
                this.MinValue = this.ActualItems.Min(r => r.Value);
                this.MaxValue = this.ActualItems.Max(r => r.Value);
            }
        }

        /// <summary>
        /// Updates the axes to include the max and min of this series.
        /// </summary>
        protected internal override void UpdateAxisMaxMin()
        {
            base.UpdateAxisMaxMin();
            var colorAxis = this.ColorAxis as Axis;
            if (colorAxis != null)
            {
                colorAxis.Include(this.MinValue);
                colorAxis.Include(this.MaxValue);
            }
        }

        private bool IsPointInRange(DataPoint p)
        {
            this.UpdateMaxMinXY();

            return p.X >= this.MinX && p.X <= this.MaxX && p.Y >= this.MinY && p.Y <= this.MaxY;
        }
    }
}
