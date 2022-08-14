namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;

    using OxyPlot.Axes;

    public abstract class ScatterSeries<T> : XYAxisSeries where T : ScatterPoint
    {
        private const string DefaultColorAxisTitle = "Value";
        private readonly List<T> points = new List<T>();
        private OxyColor defaultMarkerFillColor;
        protected ScatterSeries()
        {
            this.MarkerFill = OxyColors.Automatic;
            this.MarkerSize = 5;
            this.MarkerType = MarkerType.Square;
            this.MarkerStroke = OxyColors.Automatic;
            this.MarkerStrokeThickness = 1;
            this.LabelMargin = 6;
        }

        public List<T> Points
        {
            get
            {
                return this.points;
            }
        }

        public string LabelFormatString { get; set; }
        public double LabelMargin { get; set; }
        public Func<object, T> Mapping { get; set; }
        public int BinSize { get; set; }
        public IColorAxis ColorAxis { get; private set; }
        public string ColorAxisKey { get; set; }
        public string DataFieldX { get; set; }
        public string DataFieldY { get; set; }
        public string DataFieldSize { get; set; }
        public string DataFieldTag { get; set; }
        public string DataFieldValue { get; set; }
        public OxyColor MarkerFill { get; set; }
        public OxyColor ActualMarkerFillColor
        {
            get { return this.MarkerFill.GetActualColor(this.defaultMarkerFillColor); }
        }

        public ScreenPoint[] MarkerOutline { get; set; }
        public double MarkerSize { get; set; }
        public OxyColor MarkerStroke { get; set; }
        public double MarkerStrokeThickness { get; set; }
        public MarkerType MarkerType { get; set; }
        public double MaxValue { get; private set; }
        public double MinValue { get; private set; }
        public System.Collections.ObjectModel.ReadOnlyCollection<T> ActualPoints
        {
            get
            {
                return this.ActualPointsList != null ? new System.Collections.ObjectModel.ReadOnlyCollection<T>(this.ActualPointsList) : null;
            }
        }

        protected List<T> ActualPointsList
        {
            get
            {
                return this.ItemsSource != null ? this.ItemsSourcePoints : this.points;
            }
        }

        protected List<T> ItemsSourcePoints { get; set; }
        protected bool OwnsItemsSourcePoints { get; set; }
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

            var actualPoints = this.ActualPointsList;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return null;
            }

            TrackerHitResult result = null;
            double minimumDistance = double.MaxValue;
            int i = 0;

            var xaxisTitle = this.XAxis.Title ?? DefaultXAxisTitle;
            var yaxisTitle = this.YAxis.Title ?? DefaultYAxisTitle;
            var colorAxisTitle = (this.ColorAxis != null ? ((Axis)this.ColorAxis).Title : null) ?? DefaultColorAxisTitle;

            var xmin = this.XAxis.ClipMinimum;
            var xmax = this.XAxis.ClipMaximum;
            var ymin = this.YAxis.ClipMinimum;
            var ymax = this.YAxis.ClipMaximum;
            foreach (var p in actualPoints)
            {
                if (p.X < xmin || p.X > xmax || p.Y < ymin|| p.Y > ymax)
                {
                    i++;
                    continue;
                }

                var sp = this.Transform(p.X, p.Y);
                double dx = sp.x - point.x;
                double dy = sp.y - point.y;
                double d2 = (dx * dx) + (dy * dy);

                if (d2 < minimumDistance)
                {
                    var item = this.GetItem(i) ?? p;

                    object zvalue = null;

                    if (!double.IsNaN(p.Value) && !double.IsInfinity(p.Value))
                    {
                        zvalue = p.Value;
                    }

                    result = new TrackerHitResult
                    {
                        Series = this,
                        DataPoint = new DataPoint(p.X, p.Y),
                        Position = sp,
                        Item = item,
                        Index = i,
                        Text =
                            StringHelper.Format(
                                this.ActualCulture,
                                this.TrackerFormatString,
                                item,
                                this.Title,
                                xaxisTitle,
                                this.XAxis.GetValue(p.X),
                                yaxisTitle,
                                this.YAxis.GetValue(p.Y),
                                colorAxisTitle,
                                zvalue),
                    };

                    minimumDistance = d2;
                }

                i++;
            }

            return result;
        }

        public override void Render(IRenderContext rc)
        {
            var actualPoints = this.ActualPointsList;

            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            var clippingRect = this.GetClippingRect();

            int n = actualPoints.Count;
            var allPoints = new List<ScreenPoint>(n);
            var allMarkerSizes = new List<double>(n);
            var selectedPoints = new List<ScreenPoint>();
            var selectedMarkerSizes = new List<double>(n);
            var groupPoints = new Dictionary<int, IList<ScreenPoint>>();
            var groupSizes = new Dictionary<int, IList<double>>();

            bool isSelected = this.IsSelected();
            for (int i = 0; i < n; i++)
            {
                var dp = new DataPoint(actualPoints[i].X, actualPoints[i].Y);

                // Skip invalid points
                if (!this.IsValidPoint(dp))
                {
                    continue;
                }

                double size = double.NaN;
                double value = double.NaN;

                var scatterPoint = actualPoints[i];
                if (scatterPoint != null)
                {
                    size = scatterPoint.Size;
                    value = scatterPoint.Value;
                }

                if (double.IsNaN(size))
                {
                    size = this.MarkerSize;
                }

                var screenPoint = this.Transform(dp.X, dp.Y);

                if (isSelected && this.IsItemSelected(i))
                {
                    selectedPoints.Add(screenPoint);
                    selectedMarkerSizes.Add(size);
                    continue;
                }

                if (this.ColorAxis != null)
                {
                    if (double.IsNaN(value))
                    {
                        continue;
                    }

                    int group = this.ColorAxis.GetPaletteIndex(value);
                    if (!groupPoints.ContainsKey(group))
                    {
                        groupPoints.Add(group, new List<ScreenPoint>());
                        groupSizes.Add(group, new List<double>());
                    }

                    groupPoints[group].Add(screenPoint);
                    groupSizes[group].Add(size);
                }
                else
                {
                    allPoints.Add(screenPoint);
                    allMarkerSizes.Add(size);
                }
            }

            var binOffset = this.Transform(this.MinX, this.MaxY);

            if (this.ColorAxis != null)
            {
                // Draw the grouped (by color defined in ColorAxis) markers
                var markerIsStrokedOnly = this.MarkerType == MarkerType.Plus || this.MarkerType == MarkerType.Star || this.MarkerType == MarkerType.Cross;
                foreach (var group in groupPoints)
                {
                    var color = this.ColorAxis.GetColor(group.Key);
                    rc.DrawMarkers(
                        group.Value,
                        this.MarkerType,
                        this.MarkerOutline,
                        groupSizes[group.Key],
                        this.MarkerFill.GetActualColor(color),
                        markerIsStrokedOnly ? color : this.MarkerStroke,
                        this.MarkerStrokeThickness,
                        this.EdgeRenderingMode,
                        this.BinSize,
                        binOffset);
                }
            }

            rc.DrawMarkers(
                allPoints,
                this.MarkerType,
                this.MarkerOutline,
                allMarkerSizes,
                this.ActualMarkerFillColor,
                this.MarkerStroke,
                this.MarkerStrokeThickness,
                this.EdgeRenderingMode,
                this.BinSize,
                binOffset);

            rc.DrawMarkers(
                selectedPoints,
                this.MarkerType,
                this.MarkerOutline,
                selectedMarkerSizes,
                this.PlotModel.SelectionColor,
                this.PlotModel.SelectionColor,
                this.MarkerStrokeThickness,
                this.EdgeRenderingMode,
                this.BinSize,
                binOffset);

            if (this.LabelFormatString != null)
            {
                // render point labels (not optimized for performance)
                this.RenderPointLabels(rc, clippingRect);
            }
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double ymid = (legendBox.Top + legendBox.Bottom) / 2;

            var midpt = new ScreenPoint(xmid, ymid);

            rc.DrawMarker(
                midpt,
                this.MarkerType,
                this.MarkerOutline,
                this.MarkerSize,
                this.IsSelected() ? this.PlotModel.SelectionColor : this.ActualMarkerFillColor,
                this.IsSelected() ? this.PlotModel.SelectionColor : this.MarkerStroke,
                this.MarkerStrokeThickness,
                this.EdgeRenderingMode);
        }

        protected internal override void EnsureAxes()
        {
            base.EnsureAxes();

            this.ColorAxis = this.ColorAxisKey != null ?
                             this.PlotModel.GetAxis(this.ColorAxisKey) as IColorAxis :
                             this.PlotModel.DefaultColorAxis as IColorAxis;
        }

        protected internal override void SetDefaultValues()
        {
            if (this.MarkerFill.IsAutomatic())
            {
                this.defaultMarkerFillColor = this.PlotModel.GetDefaultColor();
            }
        }

        protected internal override void UpdateData()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            this.UpdateItemsSourcePoints();
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            this.InternalUpdateMaxMinValue(this.ActualPointsList);
        }

        protected void RenderPointLabels(IRenderContext rc, OxyRect clippingRect)
        {
            var actualPoints = this.ActualPointsList;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            // TODO: share code with LineSeries
            int index = -1;
            foreach (var point in actualPoints)
            {
                index++;
                var dataPoint = new DataPoint(point.X, point.Y);
                if (!this.IsValidPoint(dataPoint))
                {
                    continue;
                }

                var pt = this.Transform(dataPoint) + new ScreenVector(0, -this.LabelMargin);

                if (!clippingRect.Contains(pt))
                {
                    continue;
                }

                var item = this.GetItem(index);
                var s = StringHelper.Format(this.ActualCulture, this.LabelFormatString, item, point.X, point.Y);

                rc.DrawText(
                    pt,
                    s,
                    this.ActualTextColor,
                    this.ActualFont,
                    this.ActualFontSize,
                    this.ActualFontWeight,
                    0,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Bottom);
            }
        }

        protected void InternalUpdateMaxMinValue(List<T> pts)
        {
            if (pts == null || pts.Count == 0)
            {
                return;
            }

            double minx = double.MaxValue;
            double miny = double.MaxValue;
            double minvalue = double.MaxValue;
            double maxx = double.MinValue;
            double maxy = double.MinValue;
            double maxvalue = double.MinValue;

            if (double.IsNaN(minx))
            {
                minx = double.MaxValue;
            }

            if (double.IsNaN(miny))
            {
                miny = double.MaxValue;
            }

            if (double.IsNaN(maxx))
            {
                maxx = double.MinValue;
            }

            if (double.IsNaN(maxy))
            {
                maxy = double.MinValue;
            }

            if (double.IsNaN(minvalue))
            {
                minvalue = double.MinValue;
            }

            if (double.IsNaN(maxvalue))
            {
                maxvalue = double.MinValue;
            }

            foreach (var pt in pts)
            {
                double x = pt.X;
                double y = pt.Y;

#pragma warning disable 1718
                if (x != x || y != y)
                {
                    continue;
                }

                double value = pt.Value;

                if (x < minx)
                {
                    minx = x;
                }

                if (x > maxx)
                {
                    maxx = x;
                }

                if (y < miny)
                {
                    miny = y;
                }

                if (y > maxy)
                {
                    maxy = y;
                }

                if (value < minvalue)
                {
                    minvalue = value;
                }

                if (value > maxvalue)
                {
                    maxvalue = value;
                }
            }

            if (minx < double.MaxValue)
            {
                this.MinX = minx;
            }

            if (miny < double.MaxValue)
            {
                this.MinY = miny;
            }

            if (maxx > double.MinValue)
            {
                this.MaxX = maxx;
            }

            if (maxy > double.MinValue)
            {
                this.MaxY = maxy;
            }

            if (minvalue < double.MaxValue)
            {
                this.MinValue = minvalue;
            }

            if (maxvalue > double.MinValue)
            {
                this.MaxValue = maxvalue;
            }

            var colorAxis = this.ColorAxis as Axis;
            if (colorAxis != null)
            {
                colorAxis.Include(this.MinValue);
                colorAxis.Include(this.MaxValue);
            }
        }

        protected void InternalUpdateMaxMinValue(IList<ScatterPoint> pts)
        {
            if (pts == null || pts.Count == 0)
            {
                return;
            }

            double minvalue = double.NaN;
            double maxvalue = double.NaN;

            foreach (var pt in pts)
            {
                double value = pt.Value;

                if (value < minvalue || double.IsNaN(minvalue))
                {
                    minvalue = value;
                }

                if (value > maxvalue || double.IsNaN(maxvalue))
                {
                    maxvalue = value;
                }
            }

            this.MinValue = minvalue;
            this.MaxValue = maxvalue;

            var colorAxis = this.ColorAxis as Axis;
            if (colorAxis != null)
            {
                colorAxis.Include(this.MinValue);
                colorAxis.Include(this.MaxValue);
            }
        }

        protected void ClearItemsSourcePoints()
        {
            if (!this.OwnsItemsSourcePoints || this.ItemsSourcePoints == null)
            {
                this.ItemsSourcePoints = new List<T>();
            }
            else
            {
                this.ItemsSourcePoints.Clear();
            }

            this.OwnsItemsSourcePoints = true;
        }


        protected abstract void UpdateFromDataFields();
        private void UpdateItemsSourcePoints()
        {
            if (this.Mapping != null)
            {
                this.ClearItemsSourcePoints();
                foreach (var item in this.ItemsSource)
                {
                    this.ItemsSourcePoints.Add(this.Mapping(item));
                }

                return;
            }

            var sourceAsListOfScatterPoints = this.ItemsSource as List<T>;
            if (sourceAsListOfScatterPoints != null)
            {
                this.ItemsSourcePoints = sourceAsListOfScatterPoints;
                this.OwnsItemsSourcePoints = false;
                return;
            }

            this.ClearItemsSourcePoints();

            var sourceAsEnumerableScatterPoints = this.ItemsSource as IEnumerable<T>;
            if (sourceAsEnumerableScatterPoints != null)
            {
                this.ItemsSourcePoints.AddRange(sourceAsEnumerableScatterPoints);
                return;
            }

            if (this.DataFieldX == null || this.DataFieldY == null)
            {
                foreach (var item in this.ItemsSource)
                {
                    if (item is T)
                    {
                        this.ItemsSourcePoints.Add((T)item);
                        continue;
                    }

                    var idpp = item as IScatterPointProvider;
                    if (idpp != null)
                    {
                        this.ItemsSourcePoints.Add((T)idpp.GetScatterPoint());
                    }
                }

                return;
            }

            this.UpdateFromDataFields();
        }
    }
}
