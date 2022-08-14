namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ErrorBarSeries : BarSeries
    {
        public new const string DefaultTrackerFormatString = "{0}\n{1}: {2}, Error: {Error:0.###}";
        public ErrorBarSeries()
        {
            this.ErrorWidth = 0.4;
            this.ErrorStrokeThickness = 1;
            this.TrackerFormatString = DefaultTrackerFormatString;
        }

        public double ErrorStrokeThickness { get; set; }
        public double ErrorWidth { get; set; }
        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();

            if (this.ValidItems.Count == 0)
            {
                return;
            }

            var categoryAxis = this.GetCategoryAxis();

            double minValue = double.MaxValue, maxValue = double.MinValue;
            if (this.IsStacked)
            {
                var labels = this.GetCategoryAxis().ActualLabels;
                for (var i = 0; i < labels.Count; i++)
                {
                    int j = 0;
                    var items = this.ValidItems.Where(item => item.GetCategoryIndex(j++) == i).ToList();
                    var values = items.Select(item => item.Value).Concat(new[] { 0d }).ToList();
                    var minTemp = values.Where(v => v <= 0).Sum();
                    var maxTemp = values.Where(v => v >= 0).Sum() + ((ErrorBarItem)items.Last()).Error;

                    int stackIndex = this.Manager.GetStackIndex(this.StackGroup);
                    var stackedMinValue = this.Manager.GetCurrentMinValue(stackIndex, i);
                    if (!double.IsNaN(stackedMinValue))
                    {
                        minTemp += stackedMinValue;
                    }

                    this.Manager.SetCurrentMinValue(stackIndex, i, minTemp);

                    var stackedMaxValue = this.Manager.GetCurrentMaxValue(stackIndex, i);
                    if (!this.OverlapsStack && !double.IsNaN(stackedMaxValue))
                    {
                        maxTemp += stackedMaxValue;
                    }

                    this.Manager.SetCurrentMaxValue(stackIndex, i, maxTemp);

                    minValue = Math.Min(minValue, minTemp + this.BaseValue);
                    maxValue = Math.Max(maxValue, maxTemp + this.BaseValue);
                }
            }
            else
            {
                var valuesMin = this.ValidItems.Select(item => item.Value - ((ErrorBarItem)item).Error).Concat(new[] { 0d }).ToList();
                var valuesMax = this.ValidItems.Select(item => item.Value + ((ErrorBarItem)item).Error).Concat(new[] { 0d }).ToList();
                minValue = valuesMin.Min();
                maxValue = valuesMax.Max();
                if (this.BaseValue < minValue)
                {
                    minValue = this.BaseValue;
                }

                if (this.BaseValue > maxValue)
                {
                    maxValue = this.BaseValue;
                }
            }

            this.MinX = minValue;
            this.MaxX = maxValue;
        }

        protected override void RenderItem(
            IRenderContext rc,
            double barValue,
            double categoryValue,
            double actualBarWidth,
            BarItem item,
            OxyRect rect)
        {
            base.RenderItem(rc, barValue, categoryValue, actualBarWidth, item, rect);

            if (!(item is ErrorBarItem errorItem))
            {
                return;
            }

            // Render the error
            var errorStart = barValue - errorItem.Error;
            var errorEnd = barValue + errorItem.Error;
            var start = 0.5 - (this.ErrorWidth / 2);
            var end = 0.5 + (this.ErrorWidth / 2);
            var categoryStart = categoryValue + (start * actualBarWidth);
            var categoryMiddle = categoryValue + (0.5 * actualBarWidth);
            var categoryEnd = categoryValue + (end * actualBarWidth);

            var lowerErrorPoint = this.Transform(errorStart, categoryMiddle);
            var upperErrorPoint = this.Transform(errorEnd, categoryMiddle);

            rc.DrawLine(
                new List<ScreenPoint> { lowerErrorPoint, upperErrorPoint },
                this.StrokeColor,
                this.ErrorStrokeThickness,
                this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                null,
                LineJoin.Miter);

            if (this.ErrorWidth > 0)
            {
                var lowerLeftErrorPoint = this.Transform(errorStart, categoryStart);
                var lowerRightErrorPoint = this.Transform(errorStart, categoryEnd);
                rc.DrawLine(
                    new List<ScreenPoint> { lowerLeftErrorPoint, lowerRightErrorPoint },
                    this.StrokeColor,
                    this.ErrorStrokeThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                    null,
                    LineJoin.Miter);

                var upperLeftErrorPoint = this.Transform(errorEnd, categoryStart);
                var upperRightErrorPoint = this.Transform(errorEnd, categoryEnd);
                rc.DrawLine(
                    new List<ScreenPoint> { upperLeftErrorPoint, upperRightErrorPoint },
                    this.StrokeColor,
                    this.ErrorStrokeThickness,
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness),
                    null,
                    LineJoin.Miter);
            }
        }
    }
}
