namespace OxyPlot.Legends
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Legend : LegendBase
    {
        private OxyRect legendBox;

        public Legend()
        {
            this.IsLegendVisible = true;
            this.legendBox = new OxyRect();
            this.Key = null;
            this.GroupNameFont = null;
            this.GroupNameFontWeight = FontWeights.Normal;
            this.GroupNameFontSize = double.NaN;

            this.LegendTitleFont = null;
            this.LegendTitleFontSize = double.NaN;
            this.LegendTitleFontWeight = FontWeights.Bold;
            this.LegendFont = null;
            this.LegendFontSize = double.NaN;
            this.LegendFontWeight = FontWeights.Normal;
            this.LegendSymbolLength = 16;
            this.LegendSymbolMargin = 4;
            this.LegendPadding = 8;
            this.LegendColumnSpacing = 8;
            this.LegendItemSpacing = 24;
            this.LegendLineSpacing = 0;
            this.LegendMargin = 8;

            this.LegendBackground = OxyColors.Undefined;
            this.LegendBorder = OxyColors.Undefined;
            this.LegendBorderThickness = 1;

            this.LegendTextColor = OxyColors.Automatic;
            this.LegendTitleColor = OxyColors.Automatic;

            this.LegendMaxWidth = double.NaN;
            this.LegendMaxHeight = double.NaN;
            this.LegendPlacement = LegendPlacement.Inside;
            this.LegendPosition = LegendPosition.RightTop;
            this.LegendOrientation = LegendOrientation.Vertical;
            this.LegendItemOrder = LegendItemOrder.Normal;
            this.LegendItemAlignment = HorizontalAlignment.Left;
            this.LegendSymbolPlacement = LegendSymbolPlacement.Left;

            this.ShowInvisibleSeries = true;

            this.SeriesInvisibleTextColor = OxyColor.FromAColor(64, this.LegendTextColor);

            this.SeriesPosMap = new Dictionary<Series.Series, OxyRect>();

            this.Selectable = true;
            this.SelectionMode = SelectionMode.Single;
        }

        /// <summary>
        /// 可见性反转
        /// </summary>
        protected override HitTestResult LegendHitTest(HitTestArguments args)
        {
            ScreenPoint point = args.Point;
            bool isContains = this.IsPointInLegend(point);
            if (!isContains)
                return null;
            
            if (this.SeriesPosMap != null && this.SeriesPosMap.Count > 0)
            {
                foreach (KeyValuePair<Series.Series, OxyRect> kvp in this.SeriesPosMap)
                {
                    if (kvp.Value.Contains(point))
                    {
                        if (this.ShowInvisibleSeries)
                        {
                            // 可见性反转
                            kvp.Key.IsVisible = !kvp.Key.IsVisible;
                            this.PlotModel.InvalidatePlot(false);
                            break;
                        }
                    }
                }
            }

            return null;
        }

        public string GroupNameFont { get; set; }

        public double GroupNameFontSize { get; set; }

        public double GroupNameFontWeight { get; set; }

        private Dictionary<Series.Series, OxyRect> SeriesPosMap { get; set; }
        public OxyColor SeriesInvisibleTextColor { get; set; }
        public bool IsPointInLegend(ScreenPoint point)
        {
            return this.legendBox.Contains(point);
        }
    }
}
