namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot.Axes;

    public class BoxPlotSeries : XYAxisSeries
    {
        public new const string DefaultTrackerFormatString = "{0}\n{1}: {2}\nUpper Whisker: {3:N2}\nThird Quartil: {4:N2}\nMedian: {5:N2}\nFirst Quartil: {6:N2}\nLower Whisker: {7:N2}\nMean: {8:N2}";
        private List<BoxPlotItem> itemsSourceItems;
        private bool ownsItemsSourceItems;
        public BoxPlotSeries()
        {
            this.Items = new List<BoxPlotItem>();
            this.TrackerFormatString = DefaultTrackerFormatString;
            this.OutlierTrackerFormatString = "{0}\n{1}: {2}\nY: {3:0.00}";
            this.Title = null;
            this.Fill = OxyColors.Automatic;
            this.Stroke = OxyColors.Black;
            this.BoxWidth = 0.3;
            this.StrokeThickness = 1;
            this.MedianThickness = 2;
            this.MeanThickness = 2;
            this.OutlierSize = 2;
            this.OutlierType = MarkerType.Circle;
            this.MedianPointSize = 2;
            this.MeanPointSize = 2;
            this.WhiskerWidth = 0.5;
            this.LineStyle = LineStyle.Solid;
            this.ShowMedianAsDot = false;
            this.ShowMeanAsDot = false;
            this.ShowBox = true;
        }

        public double BoxWidth { get; set; }
        public OxyColor Fill { get; set; }
        public IList<BoxPlotItem> Items { get; set; }
        public LineStyle LineStyle { get; set; }
        public double MedianPointSize { get; set; }
        public double MedianThickness { get; set; }
        public double MeanPointSize { get; set; }
        public double MeanThickness { get; set; }
        public double OutlierSize { get; set; }
        public string OutlierTrackerFormatString { get; set; }
        public MarkerType OutlierType { get; set; }
        public ScreenPoint[] OutlierOutline { get; set; }
        public bool ShowBox { get; set; }
        public bool ShowMedianAsDot { get; set; }
        public bool ShowMeanAsDot { get; set; }
        public OxyColor Stroke { get; set; }
        public double StrokeThickness { get; set; }
        public double WhiskerWidth { get; set; }
        protected IList<BoxPlotItem> ActualItems
        {
            get
            {
                return this.ItemsSource != null ? this.itemsSourceItems : this.Items;
            }
        }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if (this.XAxis == null || this.YAxis == null)
            {
                return null;
            }

            double minimumDistance = double.MaxValue;
            TrackerHitResult result = null;
            foreach (var item in this.ActualItems)
            {
                foreach (var outlier in item.Outliers)
                {
                    var sp = this.Transform(item.X, outlier);
                    double d = (sp - point).LengthSquared;
                    if (d < minimumDistance)
                    {
                        result = new TrackerHitResult
                        {
                            Series = this,
                            DataPoint = new DataPoint(item.X, outlier),
                            Position = sp,
                            Item = item,
                            Text =
                                StringHelper.Format(
                                    this.ActualCulture,
                                    this.OutlierTrackerFormatString,
                                    item,
                                    this.Title,
                                    this.XAxis.Title ?? DefaultXAxisTitle,
                                    this.XAxis.GetValue(item.X),
                                    outlier)
                        };
                        minimumDistance = d;
                    }
                }

                var hitPoint = DataPoint.Undefined;

                // check if we are inside the box rectangle
                var rect = this.GetBoxRect(item);
                if (rect.Contains(point))
                {
                    var dp = this.InverseTransform(point);
                    hitPoint = new DataPoint(item.X, dp.Y);
                    minimumDistance = 0;
                }

                var topWhisker = this.Transform(item.X, item.UpperWhisker);
                var bottomWhisker = this.Transform(item.X, item.LowerWhisker);

                // check if we are near the line
                var p = ScreenPointHelper.FindPointOnLine(point, topWhisker, bottomWhisker);
                double d2 = (p - point).LengthSquared;
                if (d2 < minimumDistance)
                {
                    hitPoint = this.InverseTransform(p);
                    minimumDistance = d2;
                }

                if (hitPoint.IsDefined())
                {
                    result = new TrackerHitResult
                    {
                        Series = this,
                        DataPoint = hitPoint,
                        Position = this.Transform(hitPoint),
                        Item = item,
                        Text =
                            StringHelper.Format(
                                this.ActualCulture,
                                this.TrackerFormatString,
                                item,
                                this.Title,
                                this.XAxis.Title ?? DefaultXAxisTitle,
                                this.XAxis.GetValue(item.X),
                                this.YAxis.GetValue(item.UpperWhisker),
                                this.YAxis.GetValue(item.BoxTop),
                                this.YAxis.GetValue(item.Median),
                                this.YAxis.GetValue(item.BoxBottom),
                                this.YAxis.GetValue(item.LowerWhisker),
                                this.YAxis.GetValue(item.Mean))
                    };
                }
            }

            if (minimumDistance < double.MaxValue)
            {
                return result;
            }

            return null;
        }

        public virtual bool IsValidPoint(BoxPlotItem item, Axis xaxis, Axis yaxis)
        {
            return !double.IsNaN(item.X) && !double.IsInfinity(item.X) && !item.Values.Any(double.IsNaN)
                   && !item.Values.Any(double.IsInfinity) && (xaxis != null && xaxis.IsValidValue(item.X))
                   && (yaxis != null && item.Values.All(yaxis.IsValidValue));
        }

        public override void Render(IRenderContext rc)
        {
            if (this.ActualItems.Count == 0)
            {
                return;
            }

            var clippingRect = this.GetClippingRect();

            var outlierScreenPoints = new List<ScreenPoint>();
            var halfBoxWidth = this.BoxWidth * 0.5;
            var halfWhiskerWidth = halfBoxWidth * this.WhiskerWidth;
            var strokeColor = this.GetSelectableColor(this.Stroke);
            var fillColor = this.GetSelectableFillColor(this.Fill);

            var dashArray = this.LineStyle.GetDashArray();

            foreach (var item in this.ActualItems)
            {
                // Add the outlier points
                outlierScreenPoints.AddRange(item.Outliers.Select(outlier => this.Transform(item.X, outlier)));

                var topWhiskerTop = this.Transform(item.X, item.UpperWhisker);
                var topWhiskerBottom = this.Transform(item.X, item.BoxTop);
                var bottomWhiskerTop = this.Transform(item.X, item.BoxBottom);
                var bottomWhiskerBottom = this.Transform(item.X, item.LowerWhisker);

                if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
                {
                    rc.DrawLine(
                        new[] { topWhiskerTop, topWhiskerBottom },
                        strokeColor,
                        this.StrokeThickness,
                        this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                        dashArray,
                        LineJoin.Miter);
                    rc.DrawLine(
                        new[] { bottomWhiskerTop, bottomWhiskerBottom },
                        strokeColor,
                        this.StrokeThickness,
                        this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                        dashArray,
                        LineJoin.Miter);
                }

                // Draw the whiskers
                if (this.WhiskerWidth > 0)
                {
                    var topWhiskerLine1 = this.Transform(item.X - halfWhiskerWidth, item.UpperWhisker);
                    var topWhiskerLine2 = this.Transform(item.X + halfWhiskerWidth, item.UpperWhisker);
                    var bottomWhiskerLine1 = this.Transform(item.X - halfWhiskerWidth, item.LowerWhisker);
                    var bottomWhiskerLine2 = this.Transform(item.X + halfWhiskerWidth, item.LowerWhisker);

                    rc.DrawLine(
                        new[] { topWhiskerLine1, topWhiskerLine2 },
                        strokeColor,
                        this.StrokeThickness,
                        this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                        null,
                        LineJoin.Miter);
                    rc.DrawLine(
                        new[] { bottomWhiskerLine1, bottomWhiskerLine2 },
                        strokeColor,
                        this.StrokeThickness,
                        this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                        null,
                        LineJoin.Miter);
                }

                if (this.ShowBox)
                {
                    // Draw the box
                    var rect = this.GetBoxRect(item);
                    rc.DrawRectangle(
                        rect, 
                        fillColor, 
                        strokeColor, 
                        this.StrokeThickness, 
                        this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));
                }

                if (!this.ShowMedianAsDot)
                {
                    // Draw the median line
                    var medianLeft = this.Transform(item.X - halfBoxWidth, item.Median);
                    var medianRight = this.Transform(item.X + halfBoxWidth, item.Median);
                    rc.DrawLine(
                        new[] { medianLeft, medianRight },
                        strokeColor,
                        this.StrokeThickness * this.MedianThickness,
                        this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                        null,
                        LineJoin.Miter);
                }
                else
                {
                    var mc = this.Transform(item.X, item.Median);
                    if (clippingRect.Contains(mc))
                    {
                        var ellipseRect = new OxyRect(
                            mc.X - this.MedianPointSize,
                            mc.Y - this.MedianPointSize,
                            this.MedianPointSize * 2,
                            this.MedianPointSize * 2);
                        rc.DrawEllipse(ellipseRect, fillColor, OxyColors.Undefined, 0, this.EdgeRenderingMode);
                    }
                }

                if (!this.ShowMeanAsDot && !double.IsNaN(item.Mean))
                {
                    // Draw the median line
                    var meanLeft = this.Transform(item.X - halfBoxWidth, item.Mean);
                    var meanRight = this.Transform(item.X + halfBoxWidth, item.Mean);
                    rc.DrawLine(
                        new[] { meanLeft, meanRight },
                        strokeColor,
                        this.StrokeThickness * this.MeanThickness,
                        this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                        LineStyle.Dash.GetDashArray(),
                        LineJoin.Miter);
                }
                else if (!double.IsNaN(item.Mean))
                {
                    var mc = this.Transform(item.X, item.Mean);
                    if (clippingRect.Contains(mc))
                    {
                        var ellipseRect = new OxyRect(
                            mc.X - this.MeanPointSize,
                            mc.Y - this.MeanPointSize,
                            this.MeanPointSize * 2,
                            this.MeanPointSize * 2);
                        rc.DrawEllipse(ellipseRect, fillColor, OxyColors.Undefined, 0, this.EdgeRenderingMode);
                    }
                }
            }

            if (this.OutlierType != MarkerType.None)
            {
                // Draw the outlier(s)
                var markerSizes = outlierScreenPoints.Select(o => this.OutlierSize).ToList();
                rc.DrawMarkers(
                    outlierScreenPoints,
                    this.OutlierType,
                    this.OutlierOutline,
                    markerSizes,
                    fillColor,
                    strokeColor,
                    this.StrokeThickness,
                    this.EdgeRenderingMode);
            }
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double ybottom = legendBox.Top + ((legendBox.Bottom - legendBox.Top) * 0.7);
            double ytop = legendBox.Top + ((legendBox.Bottom - legendBox.Top) * 0.3);
            double ymid = (ybottom + ytop) * 0.5;

            var halfBoxWidth = legendBox.Width * 0.24;
            var halfWhiskerWidth = halfBoxWidth * this.WhiskerWidth;
            const double LegendStrokeThickness = 1;
            var strokeColor = this.GetSelectableColor(this.Stroke);
            var fillColor = this.GetSelectableFillColor(this.Fill);

            // render the legend with EdgeRenderingMode.PreferGeometricAccuracy, because otherwise the fine geometry can look 'weird'
            rc.DrawLine(
                new[] { new ScreenPoint(xmid, legendBox.Top), new ScreenPoint(xmid, ytop) },
                strokeColor,
                LegendStrokeThickness,
                this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferGeometricAccuracy),
                LineStyle.Solid.GetDashArray(),
                LineJoin.Miter);

            rc.DrawLine(
                new[] { new ScreenPoint(xmid, ybottom), new ScreenPoint(xmid, legendBox.Bottom) },
                strokeColor,
                LegendStrokeThickness,
                this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferGeometricAccuracy),
                LineStyle.Solid.GetDashArray(),
                LineJoin.Miter);

            if (this.WhiskerWidth > 0)
            {
                // top whisker
                rc.DrawLine(
                    new[]
                        {
                            new ScreenPoint(xmid - halfWhiskerWidth, legendBox.Bottom),
                            new ScreenPoint(xmid + halfWhiskerWidth, legendBox.Bottom)
                        },
                    strokeColor,
                    LegendStrokeThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferGeometricAccuracy),
                    LineStyle.Solid.GetDashArray(),
                    LineJoin.Miter);

                // bottom whisker
                rc.DrawLine(
                    new[]
                        {
                            new ScreenPoint(xmid - halfWhiskerWidth, legendBox.Top),
                            new ScreenPoint(xmid + halfWhiskerWidth, legendBox.Top)
                        },
                    strokeColor,
                    LegendStrokeThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferGeometricAccuracy),
                    LineStyle.Solid.GetDashArray(),
                    LineJoin.Miter);
            }

            if (this.ShowBox)
            {
                // box
                rc.DrawRectangle(
                    new OxyRect(xmid - halfBoxWidth, ytop, 2 * halfBoxWidth, ybottom - ytop),
                    fillColor,
                    strokeColor,
                    LegendStrokeThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferGeometricAccuracy));
            }

            // median
            if (!this.ShowMedianAsDot)
            {
                rc.DrawLine(
                    new[] { new ScreenPoint(xmid - halfBoxWidth, ymid), new ScreenPoint(xmid + halfBoxWidth, ymid) },
                    strokeColor,
                    LegendStrokeThickness * this.MedianThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferGeometricAccuracy),
                    LineStyle.Solid.GetDashArray(),
                    LineJoin.Miter);
            }
            else
            {
                var ellipseRect = new OxyRect(
                    xmid - this.MedianPointSize,
                    ymid - this.MedianPointSize,
                    this.MedianPointSize * 2,
                    this.MedianPointSize * 2);
                rc.DrawEllipse(ellipseRect, fillColor, OxyColors.Undefined, 0, this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferGeometricAccuracy));
            }
        }

        protected internal override void UpdateData()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            var sourceAsListOfT = this.ItemsSource as IEnumerable<BoxPlotItem>;
            if (sourceAsListOfT != null)
            {
                this.itemsSourceItems = sourceAsListOfT.ToList();
                this.ownsItemsSourceItems = false;
                return;
            }

            this.ClearItemsSourceItems();

            this.itemsSourceItems.AddRange(this.ItemsSource.OfType<BoxPlotItem>());
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            this.InternalUpdateMaxMin(this.ActualItems);
        }

        protected void InternalUpdateMaxMin(IList<BoxPlotItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            double minx = this.MinX;
            double miny = this.MinY;
            double maxx = this.MaxX;
            double maxy = this.MaxY;

            foreach (var pt in items)
            {
                if (!this.IsValidPoint(pt, this.XAxis, this.YAxis))
                {
                    continue;
                }

                var x = pt.X;
                if (x < minx || double.IsNaN(minx))
                {
                    minx = x;
                }

                if (x > maxx || double.IsNaN(maxx))
                {
                    maxx = x;
                }

                foreach (var y in pt.Values)
                {
                    if (y < miny || double.IsNaN(miny))
                    {
                        miny = y;
                    }

                    if (y > maxy || double.IsNaN(maxy))
                    {
                        maxy = y;
                    }
                }
            }

            this.MinX = minx;
            this.MinY = miny;
            this.MaxX = maxx;
            this.MaxY = maxy;
        }

        protected override object GetItem(int i)
        {
            if (this.ItemsSource != null || this.ActualItems == null || this.ActualItems.Count == 0)
            {
                return base.GetItem(i);
            }

            return this.ActualItems[i];
        }

        private OxyRect GetBoxRect(BoxPlotItem item)
        {
            var halfBoxWidth = this.BoxWidth * 0.5;

            var p1 = this.Transform(item.X - halfBoxWidth, item.BoxTop);
            var p2 = this.Transform(item.X + halfBoxWidth, item.BoxBottom);

            return new OxyRect(p1, p2);
        }

        private void ClearItemsSourceItems()
        {
            if (!this.ownsItemsSourceItems || this.itemsSourceItems == null)
            {
                this.itemsSourceItems = new List<BoxPlotItem>();
            }
            else
            {
                this.itemsSourceItems.Clear();
            }

            this.ownsItemsSourceItems = true;
        }
    }
}
