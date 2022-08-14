namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;

    public class RectangleBarSeries : XYAxisSeries
    {
        public new const string DefaultTrackerFormatString = "{0}\n{1}: {2} {3}\n{4}: {5} {6}";

        private OxyColor defaultFillColor;
        public RectangleBarSeries()
        {
            this.Items = new List<RectangleBarItem>();

            this.FillColor = OxyColors.Automatic;
            this.LabelColor = OxyColors.Automatic;
            this.StrokeColor = OxyColors.Black;
            this.StrokeThickness = 1;

            this.TrackerFormatString = DefaultTrackerFormatString;

            this.LabelFormatString = "{4}"; // title
        }


        public OxyColor FillColor { get; set; }
        public OxyColor ActualFillColor
        {
            get { return this.FillColor.GetActualColor(this.defaultFillColor); }
        }

        public IList<RectangleBarItem> Items { get; private set; }
        public OxyColor LabelColor { get; set; }
        public string LabelFormatString { get; set; }
        public OxyColor StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        internal IList<OxyRect> ActualBarRectangles { get; set; }
        internal IList<RectangleBarItem> ActualItems { get; set; }
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if (this.ActualBarRectangles == null)
            {
                return null;
            }

            for (int i = 0; i < this.ActualBarRectangles.Count; i++)
            {
                var r = this.ActualBarRectangles[i];
                if (r.Contains(point))
                {
                    double value = (this.ActualItems[i].Y0 + this.ActualItems[i].Y1) / 2;
                    var sp = point;
                    var dp = new DataPoint(i, value);
                    var item = this.ActualItems[i];
                    return new TrackerHitResult
                    {
                        Series = this,
                        DataPoint = dp,
                        Position = sp,
                        Item = item,
                        Index = i,
                        Text = StringHelper.Format(
                        this.ActualCulture,
                        this.TrackerFormatString,
                        item,
                        this.Title,
                        this.XAxis.Title ?? DefaultXAxisTitle,
                        this.XAxis.GetValue(this.ActualItems[i].X0),
                        this.XAxis.GetValue(this.ActualItems[i].X1),
                        this.YAxis.Title ?? DefaultYAxisTitle,
                        this.YAxis.GetValue(this.ActualItems[i].Y0),
                        this.YAxis.GetValue(this.ActualItems[i].Y1),
                        this.ActualItems[i].Title)
                    };
                }
            }

            return null;
        }

        public override void Render(IRenderContext rc)
        {
            if (this.Items.Count == 0)
            {
                return;
            }

            int startIdx = 0;
            double xmax = double.MaxValue;

            this.ActualBarRectangles = new List<OxyRect>();
            this.ActualItems = new List<RectangleBarItem>();

            if (this.IsXMonotonic)
            {
                var xmin = this.XAxis.ClipMinimum;
                xmax = this.XAxis.ClipMaximum;
                this.WindowStartIndex = this.UpdateWindowStartIndex(this.Items, rect => rect.X0, xmin, this.WindowStartIndex);

                startIdx = this.WindowStartIndex;
            }

            int clipCount = 0;
            for (int i = startIdx; i < this.Items.Count; i++){
                var item = this.Items[i];
                if (!this.IsValid(item.X0) || !this.IsValid(item.X1)
                    || !this.IsValid(item.Y0) || !this.IsValid(item.Y1))
                {
                    continue;
                }

                var p0 = this.Transform(item.X0, item.Y0);
                var p1 = this.Transform(item.X1, item.Y1);

                var rectangle = OxyRect.Create(p0.X, p0.Y, p1.X, p1.Y);

                this.ActualBarRectangles.Add(rectangle);
                this.ActualItems.Add(item);

                rc.DrawRectangle(
                    rectangle,
                    this.GetSelectableFillColor(item.Color.GetActualColor(this.ActualFillColor)),
                    this.StrokeColor,
                    this.StrokeThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));

                if (this.LabelFormatString != null)
                {
                    var s = StringHelper.Format(
                        this.ActualCulture,
                        this.LabelFormatString,
                        this.GetItem(i),
                        item.X0,
                        item.X1,
                        item.Y0,
                        item.Y1,
                        item.Title);

                    var pt = new ScreenPoint(
                        (rectangle.Left + rectangle.Right) / 2, (rectangle.Top + rectangle.Bottom) / 2);

                    rc.DrawText(
                        pt,
                        s,
                        this.ActualTextColor,
                        this.ActualFont,
                        this.ActualFontSize,
                        this.ActualFontWeight,
                        0,
                        HorizontalAlignment.Center,
                        VerticalAlignment.Middle);
                }

                clipCount += item.X0 > xmax ? 1 : 0;
                if (clipCount > 1)
                {
                    break;
                }
            }
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double ymid = (legendBox.Top + legendBox.Bottom) / 2;
            double height = (legendBox.Bottom - legendBox.Top) * 0.8;
            double width = height;
            rc.DrawRectangle(
                new OxyRect(xmid - (0.5 * width), ymid - (0.5 * height), width, height),
                this.GetSelectableFillColor(this.ActualFillColor),
                this.StrokeColor,
                this.StrokeThickness,
                this.EdgeRenderingMode);
        }

        protected internal override void SetDefaultValues()
        {
            if (this.FillColor.IsAutomatic())
            {
                this.defaultFillColor = this.PlotModel.GetDefaultColor();
            }
        }

        protected internal override void UpdateData()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            this.Items.Clear();
            throw new NotImplementedException();
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();

            this.IsXMonotonic = true;

            if (this.Items == null || this.Items.Count == 0)
            {
                return;
            }

            double minValueX = double.MaxValue;
            double maxValueX = double.MinValue;
            double minValueY = double.MaxValue;
            double maxValueY = double.MinValue;

            double lastX0 = double.MinValue;
            double lastX1 = double.MinValue;
            foreach (var item in this.Items)
            {
                if (item.X0 < lastX0 || item.X1 < lastX1)
                {
                    this.IsXMonotonic = false;
                }

                minValueX = Math.Min(minValueX, Math.Min(item.X0, item.X1));
                maxValueX = Math.Max(maxValueX, Math.Max(item.X1, item.X0));
                minValueY = Math.Min(minValueY, Math.Min(item.Y0, item.Y1));
                maxValueY = Math.Max(maxValueY, Math.Max(item.Y0, item.Y1));

                lastX0 = item.X0;
                lastX1 = item.X1;
            }

            this.MinX = minValueX;
            this.MaxX = maxValueX;
            this.MinY = minValueY;
            this.MaxY = maxValueY;
        }

        protected virtual bool IsValid(double v)
        {
            return !double.IsNaN(v) && !double.IsInfinity(v);
        }
    }
}
