
namespace OxyPlot.Axes
{
    using System;
    using System.Collections.Generic;

    public class CategoryColorAxis : CategoryAxis, IColorAxis
    {
        public CategoryColorAxis()
        {
            this.Palette = new OxyPalette();
        }
        
        public OxyColor InvalidCategoryColor { get; set; }
        public OxyPalette Palette { get; set; }
        public OxyColor GetColor(int paletteIndex)
        {
            if (paletteIndex == -1)
            {
                return this.InvalidCategoryColor;
            }

            if (paletteIndex >= this.Palette.Colors.Count)
            {
                return this.InvalidCategoryColor;
            }

            return this.Palette.Colors[paletteIndex];
        }

        public int GetPaletteIndex(double value)
        {
            return (int)value;
        }


        public override void Render(IRenderContext rc, int pass)
        {
            if (this.Position == AxisPosition.None)
            {
                return;
            }

            if (pass == 0)
            {
                double left = this.PlotModel.PlotArea.Left;
                double top = this.PlotModel.PlotArea.Top;
                double width = this.MajorTickSize - 2;
                double height = this.MajorTickSize - 2;

                switch (this.Position)
                {
                    case AxisPosition.Left:
                        left = this.PlotModel.PlotArea.Left - this.PositionTierMinShift - width;
                        top = this.PlotModel.PlotArea.Top;
                        break;
                    case AxisPosition.Right:
                        left = this.PlotModel.PlotArea.Right + this.PositionTierMinShift;
                        top = this.PlotModel.PlotArea.Top;
                        break;
                    case AxisPosition.Top:
                        left = this.PlotModel.PlotArea.Left;
                        top = this.PlotModel.PlotArea.Top - this.PositionTierMinShift - height;
                        break;
                    case AxisPosition.Bottom:
                        left = this.PlotModel.PlotArea.Left;
                        top = this.PlotModel.PlotArea.Bottom + this.PositionTierMinShift;
                        break;
                }

                Action<double, double, OxyColor> drawColorRect = (ylow, yhigh, color) =>
                {
                    double ymin = Math.Min(ylow, yhigh);
                    double ymax = Math.Max(ylow, yhigh);
                    rc.DrawRectangle(
                        this.IsHorizontal()
                            ? new OxyRect(ymin, top, ymax - ymin, height)
                            : new OxyRect(left, ymin, width, ymax - ymin),
                        color,
                        OxyColors.Undefined,
                        0,
                        this.EdgeRenderingMode);
                };

                IList<double> majorLabelValues;
                IList<double> majorTickValues;
                IList<double> minorTickValues;
                this.GetTickValues(out majorLabelValues, out majorTickValues, out minorTickValues);

                int n = this.Palette.Colors.Count;
                for (int i = 0; i < n; i++)
                {
                    double low = this.Transform(this.GetLowValue(i, majorLabelValues));
                    double high = this.Transform(this.GetHighValue(i, majorLabelValues));
                    drawColorRect(low, high, this.Palette.Colors[i]);
                }
            }

            base.Render(rc, pass);
        }


        protected double GetHighValue(int paletteIndex)
        {
            IList<double> majorLabelValues;
            IList<double> majorTickValues;
            IList<double> minorTickValues;
            this.GetTickValues(out majorLabelValues, out majorTickValues, out minorTickValues);
            var highValue = this.GetHighValue(paletteIndex, majorLabelValues);
            return highValue;
        }

        private double GetHighValue(int paletteIndex, IList<double> majorLabelValues)
        {
            double highValue = paletteIndex >= this.Palette.Colors.Count - 1
                                   ? this.ClipMaximum
                                   : (majorLabelValues[paletteIndex] + majorLabelValues[paletteIndex + 1]) / 2;
            return highValue;
        }

        private double GetLowValue(int paletteIndex, IList<double> majorLabelValues)
        {
            double lowValue = paletteIndex == 0
                                  ? this.ClipMinimum
                                  : (majorLabelValues[paletteIndex - 1] + majorLabelValues[paletteIndex]) / 2;
            return lowValue;
        }
    }
}
