namespace OxyPlot.Series
{
    using System.Collections.Generic;
    public class LinearBarSeries : DataPointSeries
    {
        private readonly List<OxyRect> rectangles = new List<OxyRect>();
        private readonly List<int> rectanglesPointIndexes = new List<int>();
        private OxyColor defaultColor;
        public LinearBarSeries()
        {
            this.FillColor = OxyColors.Automatic;
            this.BarWidth = 5;
            this.StrokeColor = OxyColors.Black;
            this.StrokeThickness = 0;
            this.TrackerFormatString = XYAxisSeries.DefaultTrackerFormatString;
            this.NegativeFillColor = OxyColors.Undefined;
            this.NegativeStrokeColor = OxyColors.Undefined;
        }

        public OxyColor FillColor { get; set; }
        public double BarWidth { get; set; }
        public double StrokeThickness { get; set; }
        public OxyColor StrokeColor { get; set; }
        public OxyColor NegativeFillColor { get; set; }
        public OxyColor NegativeStrokeColor { get; set; }
        public OxyColor ActualColor
        {
            get
            {
                return this.FillColor.GetActualColor(this.defaultColor);
            }
        }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            var rectangleIndex = this.FindRectangleIndex(point);
            if (rectangleIndex < 0)
            {
                return null;
            }

            var rectangle = this.rectangles[rectangleIndex];
            if (!rectangle.Contains(point))
            {
                return null;
            }

            var pointIndex = this.rectanglesPointIndexes[rectangleIndex];
            var dataPoint = this.ActualPoints[pointIndex];
            var item = this.GetItem(pointIndex);

            // Format: {0}\n{1}: {2}\n{3}: {4}
            var trackerParameters = new[]
            {
                this.Title,
                this.XAxis.Title ?? "X",
                this.XAxis.GetValue(dataPoint.X), 
                this.YAxis.Title ?? "Y", 
                this.YAxis.GetValue(dataPoint.Y),
            };

            var text = StringHelper.Format(this.ActualCulture, this.TrackerFormatString, item, trackerParameters);

            return new TrackerHitResult
            {
                Series = this,
                DataPoint = dataPoint,
                Position = point,
                Item = item,
                Index = pointIndex,
                Text = text,
            };
        }

        public override void Render(IRenderContext rc)
        {
            this.rectangles.Clear();
            this.rectanglesPointIndexes.Clear();

            var actualPoints = this.ActualPoints;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            this.VerifyAxes();

            this.RenderBars(rc, actualPoints);
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            var xmid = (legendBox.Left + legendBox.Right) / 2;
            var ymid = (legendBox.Top + legendBox.Bottom) / 2;
            var height = (legendBox.Bottom - legendBox.Top) * 0.8;
            var width = height;
            rc.DrawRectangle(
                new OxyRect(xmid - (0.5 * width), ymid - (0.5 * height), width, height),
                this.GetSelectableColor(this.ActualColor),
                this.StrokeColor,
                this.StrokeThickness,
                this.EdgeRenderingMode);
        }

        protected internal override void SetDefaultValues()
        {
            if (this.FillColor.IsAutomatic())
            {
                this.defaultColor = this.PlotModel.GetDefaultColor();
            }
        }

        protected internal override void UpdateAxisMaxMin()
        {
            base.UpdateAxisMaxMin();

            this.YAxis.Include(0.0);
        }

        private int FindRectangleIndex(ScreenPoint point)
        {
            IComparer<OxyRect> comparer;
            if (this.IsTransposed())
            {
                comparer = ComparerHelper.CreateComparer<OxyRect>(
                    (x, y) =>
                        {
                            if (x.Bottom < point.Y)
                            {
                                return 1;
                            }

                            if (x.Top > point.Y)
                            {
                                return -1;
                            }

                            return 0;
                        });
            }
            else
            {
                comparer = ComparerHelper.CreateComparer<OxyRect>(
                    (x, y) =>
                        {
                            if (x.Right < point.X)
                            {
                                return -1;
                            }

                            if (x.Left > point.X)
                            {
                                return 1;
                            }

                            return 0;
                        });
            }

            return this.rectangles.BinarySearch(0, this.rectangles.Count, new OxyRect(), comparer);
        }


        private void RenderBars(IRenderContext rc, List<DataPoint> actualPoints)
        {
            var widthOffset = this.GetBarWidth(actualPoints) / 2;
            var widthVector = this.Orientate(new ScreenVector(widthOffset, 0));

            for (var pointIndex = 0; pointIndex < actualPoints.Count; pointIndex++)
            {
                var actualPoint = actualPoints[pointIndex];
                if (!this.IsValidPoint(actualPoint))
                {
                    continue;
                }

                var screenPoint = this.Transform(actualPoint) - widthVector;
                var basePoint = this.Transform(new DataPoint(actualPoint.X, 0)) + widthVector;
                var rectangle = new OxyRect(basePoint, screenPoint);
                this.rectangles.Add(rectangle);
                this.rectanglesPointIndexes.Add(pointIndex);

                var barColors = this.GetBarColors(actualPoint.Y);

                rc.DrawRectangle(
                    rectangle, 
                    barColors.FillColor, 
                    barColors.StrokeColor, 
                    this.StrokeThickness, 
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));
            }
        }

        private double GetBarWidth(List<DataPoint> actualPoints)
        {
            var minDistance = this.BarWidth / this.XAxis.Scale;
            for (var pointIndex = 1; pointIndex < actualPoints.Count; pointIndex++)
            {
                var distance = actualPoints[pointIndex].X - actualPoints[pointIndex - 1].X;
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            return minDistance * this.XAxis.Scale;
        }

        private BarColors GetBarColors(double y)
        {
            var positive = y >= 0.0;
            var fillColor = (positive || this.NegativeFillColor.IsUndefined()) ? this.GetSelectableFillColor(this.ActualColor) : this.NegativeFillColor;
            var strokeColor = (positive || this.NegativeStrokeColor.IsUndefined()) ? this.StrokeColor : this.NegativeStrokeColor;

            return new BarColors(fillColor, strokeColor);
        }

        private struct BarColors
        {
            public BarColors(OxyColor fillColor, OxyColor strokeColor) : this()
            {
                this.FillColor = fillColor;
                this.StrokeColor = strokeColor;
            }

            public OxyColor FillColor { get; private set; }
            public OxyColor StrokeColor { get; private set; }
        }
    }
}
