namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;

    public class TornadoBarSeries : BarSeriesBase<TornadoBarItem>
    {
        public new const string DefaultTrackerFormatString = "{0}\n{1}: {2}\n{3}: {4}";
        private OxyColor defaultMaximumFillColor;
        private OxyColor defaultMinimumFillColor;
        public TornadoBarSeries()
        {
            this.MaximumFillColor = OxyColor.FromRgb(216, 82, 85);
            this.MinimumFillColor = OxyColor.FromRgb(84, 138, 209);

            this.LabelColor = OxyColors.Automatic;
            this.StrokeColor = OxyColors.Black;
            this.StrokeThickness = 1;

            this.TrackerFormatString = DefaultTrackerFormatString;
            this.LabelMargin = 4;

            this.MinimumLabelFormatString = "{0}";
            this.MaximumLabelFormatString = "{0}";
        }
        public OxyColor ActualMaximumFillColor => this.MaximumFillColor.GetActualColor(this.defaultMaximumFillColor);
        public OxyColor ActualMinimumFillColor => this.MinimumFillColor.GetActualColor(this.defaultMinimumFillColor);
        public string BaseField { get; set; }
        public double BaseValue { get; set; }
        public OxyColor LabelColor { get; set; }
        public double LabelMargin { get; set; }
        public string MaximumColorField { get; set; }
        public string MaximumField { get; set; }
        public OxyColor MaximumFillColor { get; set; }
        public string MaximumLabelFormatString { get; set; }
        public string MinimumColorField { get; set; }
        public string MinimumField { get; set; }
        public OxyColor MinimumFillColor { get; set; }
        public string MinimumLabelFormatString { get; set; }

        protected internal IList<OxyRect> ActualMaximumBarRectangles { get; set; }

        protected internal IList<OxyRect> ActualMinimumBarRectangles { get; set; }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            for (var i = 0; i < this.ActualMinimumBarRectangles.Count; i++)
            {
                var insideMinimumRectangle = this.ActualMinimumBarRectangles[i].Contains(point);
                var insideMaximumRectangle = this.ActualMaximumBarRectangles[i].Contains(point);
                if (insideMinimumRectangle || insideMaximumRectangle)
                {
                    var item = (TornadoBarItem)this.GetItem(this.ValidItemsIndexInversion[i]);
                    var categoryIndex = item.GetCategoryIndex(i);
                    var value = insideMaximumRectangle ? this.ValidItems[i].Maximum : this.ValidItems[i].Minimum;
                    var dp = new DataPoint(categoryIndex, value);
                    var categoryAxis = this.GetCategoryAxis();
                    return new TrackerHitResult
                    {
                        Series = this,
                        DataPoint = dp,
                        Position = point,
                        Item = item,
                        Index = i,
                        Text =
                            StringHelper.Format(
                                this.ActualCulture,
                                this.TrackerFormatString,
                                item,
                                this.Title,
                                categoryAxis.Title ?? DefaultCategoryAxisTitle,
                                categoryAxis.FormatValue(categoryIndex),
                                this.XAxis.Title ?? DefaultValueAxisTitle,
                                this.XAxis.GetValue(value))
                    };
                }
            }

            return null;
        }

        public override void Render(IRenderContext rc)
        {
            this.ActualMinimumBarRectangles = new List<OxyRect>();
            this.ActualMaximumBarRectangles = new List<OxyRect>();

            if (this.ValidItems.Count == 0)
            {
                return;
            }

            var actualBarWidth = this.GetActualBarWidth();

            for (var i = 0; i < this.ValidItems.Count; i++)
            {
                var item = this.ValidItems[i];

                var categoryIndex = item.GetCategoryIndex(i);

                var baseValue = double.IsNaN(item.BaseValue) ? this.BaseValue : item.BaseValue;
                var barOffset = this.Manager.GetCurrentBarOffset(categoryIndex);
                var barStart = categoryIndex - 0.5 + barOffset;
                var barEnd = barStart + actualBarWidth;
                var barMid = (barStart + barEnd) / 2;

                var pMin = this.Transform(item.Minimum, barStart);
                var pMax = this.Transform(item.Maximum, barStart);
                var pBase = this.Transform(baseValue, barStart + actualBarWidth);

                var minimumRectangle = new OxyRect(pMin, pBase);
                var maximumRectangle = new OxyRect(pMax, pBase);

                this.ActualMinimumBarRectangles.Add(minimumRectangle);
                this.ActualMaximumBarRectangles.Add(maximumRectangle);

                rc.DrawRectangle(
                    minimumRectangle,
                    item.MinimumColor.GetActualColor(this.ActualMinimumFillColor),
                    this.StrokeColor,
                    this.StrokeThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));
                rc.DrawRectangle(
                    maximumRectangle,
                    item.MaximumColor.GetActualColor(this.ActualMaximumFillColor),
                    this.StrokeColor,
                    this.StrokeThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));

                var marginVector = this.Orientate(new ScreenVector(this.LabelMargin, 0));

                if (this.MinimumLabelFormatString != null)
                {
                    var s = StringHelper.Format(
                        this.ActualCulture,
                        this.MinimumLabelFormatString,
                        this.GetItem(this.ValidItemsIndexInversion[i]),
                        item.Minimum);

                    var pt = this.Transform(item.Minimum, barMid) - marginVector;
                    var ha = HorizontalAlignment.Right;
                    var va = VerticalAlignment.Middle;
                    this.Orientate(ref ha, ref va);

                    rc.DrawText(
                        pt,
                        s,
                        this.ActualTextColor,
                        this.ActualFont,
                        this.ActualFontSize,
                        this.ActualFontWeight,
                        0,
                        ha,
                        va);
                }

                if (this.MaximumLabelFormatString != null)
                {
                    var s = StringHelper.Format(
                        this.ActualCulture,
                        this.MaximumLabelFormatString,
                        this.GetItem(this.ValidItemsIndexInversion[i]),
                        item.Maximum);

                    var pt = this.Transform(item.Maximum, barMid) + marginVector;
                    var ha = HorizontalAlignment.Left;
                    var va = VerticalAlignment.Middle;
                    this.Orientate(ref ha, ref va);

                    rc.DrawText(
                        pt,
                        s,
                        this.ActualTextColor,
                        this.ActualFont,
                        this.ActualFontSize,
                        this.ActualFontWeight,
                        0,
                        ha,
                        va);
                }
            }
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            var xmid = (legendBox.Left + legendBox.Right) / 2;
            var ymid = (legendBox.Top + legendBox.Bottom) / 2;
            var height = (legendBox.Bottom - legendBox.Top) * 0.8;
            var width = height;
            rc.DrawRectangle(
                new OxyRect(xmid - (0.5 * width), ymid - (0.5 * height), 0.5 * width, height),
                this.ActualMinimumFillColor,
                this.StrokeColor,
                this.StrokeThickness,
                this.EdgeRenderingMode);
            rc.DrawRectangle(
                new OxyRect(xmid, ymid - (0.5 * height), 0.5 * width, height),
                this.ActualMaximumFillColor,
                this.StrokeColor,
                this.StrokeThickness,
                this.EdgeRenderingMode);
        }

        protected internal override void SetDefaultValues()
        {
            if (this.MaximumFillColor.IsAutomatic())
            {
                this.defaultMaximumFillColor = this.PlotModel.GetDefaultColor();
            }

            if (this.MinimumFillColor.IsAutomatic())
            {
                this.defaultMinimumFillColor = this.PlotModel.GetDefaultColor();
            }
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();

            if (this.ValidItems.Count == 0)
            {
                return;
            }

            var minValue = double.MaxValue;
            var maxValue = double.MinValue;

            foreach (var item in this.ValidItems)
            {
                minValue = Math.Min(minValue, item.Minimum);
                maxValue = Math.Max(maxValue, item.Maximum);
            }

            this.MinX = minValue;
            this.MaxX = maxValue;
        }

        protected override bool IsValid(TornadoBarItem item)
        {
            return this.XAxis.IsValidValue(item.Minimum) && this.XAxis.IsValidValue(item.Maximum);
        }

        protected override bool UpdateFromDataFields()
        {
            if (this.MinimumField == null || this.MaximumField == null)
            {
                return false;
            }

            var filler = new ListBuilder<BarItem>();
            filler.Add(this.MinimumField, double.NaN);
            filler.Add(this.MaximumField, double.NaN);
            filler.Add(this.BaseField, double.NaN);
            filler.Add(this.MinimumColorField, OxyColors.Automatic);
            filler.Add(this.MaximumColorField, OxyColors.Automatic);
            filler.Fill(
                this.ItemsSourceItems,
                this.ItemsSource,
                args => new TornadoBarItem()
                {
                    Minimum = Convert.ToDouble(args[0]),
                    Maximum = Convert.ToDouble(args[1]),
                    BaseValue = Convert.ToDouble(args[2]),
                    MinimumColor = (OxyColor)args[3],
                    MaximumColor = (OxyColor)args[4],
                });

            return true;
        }
    }
}
