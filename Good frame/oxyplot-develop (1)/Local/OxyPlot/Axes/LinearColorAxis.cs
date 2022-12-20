
namespace OxyPlot.Axes
{
    using System;
    using System.Collections.Generic;

    public class LinearColorAxis : LinearAxis, IColorAxis
    {
        public LinearColorAxis()
        {
            this.Position = AxisPosition.None;
            this.AxisDistance = 20;

            this.IsPanEnabled = false;
            this.IsZoomEnabled = false;
            this.Palette = OxyPalettes.Viridis();

            this.LowColor = OxyColors.Undefined;
            this.HighColor = OxyColors.Undefined;
            this.InvalidNumberColor = OxyColors.Gray;
        }

        public OxyColor InvalidNumberColor { get; set; }
        public OxyColor HighColor { get; set; }
        public OxyColor LowColor { get; set; }
        public OxyPalette Palette { get; set; }
        public bool RenderAsImage { get; set; }
        public override bool IsXyAxis()
        {
            return false;
        }

        public OxyColor GetColor(int paletteIndex)
        {
            if (paletteIndex == int.MinValue)
            {
                return this.InvalidNumberColor;
            }

            if (paletteIndex == 0)
            {
                return this.LowColor;
            }

            if (paletteIndex == this.Palette.Colors.Count + 1)
            {
                return this.HighColor;
            }

            return this.Palette.Colors[paletteIndex - 1];
        }

        public IEnumerable<OxyColor> GetColors()
        {
            yield return this.LowColor;
            foreach (var color in this.Palette.Colors)
            {
                yield return color;
            }

            yield return this.HighColor;
        }

        public int GetPaletteIndex(double value)
        {
            if (double.IsNaN(value))
            {
                return int.MinValue;
            }

            if (!this.LowColor.IsUndefined() && value < this.ClipMinimum)
            {
                return 0;
            }

            if (!this.HighColor.IsUndefined() && value > this.ClipMaximum)
            {
                return this.Palette.Colors.Count + 1;
            }

            int index = 1 + (int)((value - this.ClipMinimum) / (this.ClipMaximum - this.ClipMinimum) * this.Palette.Colors.Count);

            if (index < 1)
            {
                index = 1;
            }

            if (index > this.Palette.Colors.Count)
            {
                index = this.Palette.Colors.Count;
            }

            return index;
        }

        public override void Render(IRenderContext rc, int pass)
        {
            if (this.Position == AxisPosition.None)
            {
                return;
            }

            if (this.Palette == null)
            {
                throw new InvalidOperationException("No Palette defined for color axis.");
            }

            if (pass == 0)
            {
                double distance = this.AxisDistance;
                double left = this.PlotModel.PlotArea.Left;
                double top = this.PlotModel.PlotArea.Top;
                double width = this.MajorTickSize - 2;
                double height = this.MajorTickSize - 2;

                switch (this.Position)
                {
                    case AxisPosition.Left:
                        left = this.PlotModel.PlotArea.Left - this.PositionTierMinShift - width - distance;
                        top = this.PlotModel.PlotArea.Top;
                        break;
                    case AxisPosition.Right:
                        left = this.PlotModel.PlotArea.Right + this.PositionTierMinShift + distance;
                        top = this.PlotModel.PlotArea.Top;
                        break;
                    case AxisPosition.Top:
                        left = this.PlotModel.PlotArea.Left;
                        top = this.PlotModel.PlotArea.Top - this.PositionTierMinShift - height - distance;
                        break;
                    case AxisPosition.Bottom:
                        left = this.PlotModel.PlotArea.Left;
                        top = this.PlotModel.PlotArea.Bottom + this.PositionTierMinShift + distance;
                        break;
                }

                if (this.RenderAsImage)
                {
                    var axisLength = this.Transform(this.ClipMaximum) - this.Transform(this.ClipMinimum);
                    bool reverse = axisLength > 0;
                    axisLength = Math.Abs(axisLength);

                    if (this.IsHorizontal())
                    {
                        var colorAxisImage = this.GenerateColorAxisImage(reverse);
                        rc.DrawImage(colorAxisImage, left, top, axisLength, height, 1, true);
                    }
                    else
                    {
                        var colorAxisImage = this.GenerateColorAxisImage(reverse);
                        rc.DrawImage(colorAxisImage, left, top, width, axisLength, 1, true);
                    }
                }
                else
                {
                    Action<double, double, OxyColor> drawColorRect = (ylow, yhigh, color) =>
                                       {
                                           double ymin = Math.Min(ylow, yhigh);
                                           double ymax = Math.Max(ylow, yhigh) + 0.5;
                                           rc.DrawRectangle(
                                               this.IsHorizontal()
                                                   ? new OxyRect(ymin, top, ymax - ymin, height)
                                                   : new OxyRect(left, ymin, width, ymax - ymin),
                                               color,
                                               OxyColors.Undefined,
                                               0,
                                               this.EdgeRenderingMode);
                                       };

                    int n = this.Palette.Colors.Count;
                    for (int i = 0; i < n; i++)
                    {
                        double ylow = this.Transform(this.GetLowValue(i));
                        double yhigh = this.Transform(this.GetHighValue(i));
                        drawColorRect(ylow, yhigh, this.Palette.Colors[i]);
                    }

                    double highLowLength = 10;
                    if (this.IsHorizontal())
                    {
                        highLowLength *= -1;
                    }

                    if (!this.LowColor.IsUndefined())
                    {
                        double ylow = this.Transform(this.ClipMinimum);
                        drawColorRect(ylow, ylow + highLowLength, this.LowColor);
                    }

                    if (!this.HighColor.IsUndefined())
                    {
                        double yhigh = this.Transform(this.ClipMaximum);
                        drawColorRect(yhigh, yhigh - highLowLength, this.HighColor);
                    }
                }
            }

            base.Render(rc, pass);
        }

        protected double GetHighValue(int paletteIndex)
        {
            return this.GetLowValue(paletteIndex + 1);
        }

        protected double GetLowValue(int paletteIndex)
        {
            return ((double)paletteIndex / this.Palette.Colors.Count * (this.ClipMaximum - this.ClipMinimum))
                   + this.ClipMinimum;
        }

        private OxyImage GenerateColorAxisImage(bool reverse)
        {
            int n = this.Palette.Colors.Count;
            var buffer = this.IsHorizontal() ? new OxyColor[n, 1] : new OxyColor[1, n];
            for (var i = 0; i < n; i++)
            {
                var color = this.Palette.Colors[i];
                var i2 = reverse ? n - 1 - i : i;
                if (this.IsHorizontal())
                {
                    buffer[i2, 0] = color;
                }
                else
                {
                    buffer[0, i2] = color;
                }
            }

            return OxyImage.Create(buffer, ImageFormat.Png);
        }
    }
}
