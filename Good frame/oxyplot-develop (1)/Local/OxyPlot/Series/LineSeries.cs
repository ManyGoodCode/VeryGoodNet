namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LineSeries : DataPointSeries
    {
        private const double ToleranceDivisor = 200;
        private List<ScreenPoint> outputBuffer;
        private List<ScreenPoint> contiguousScreenPointsBuffer;
        private List<ScreenPoint> decimatorBuffer;
        private OxyColor defaultColor;
        private OxyColor defaultMarkerFill;
        private LineStyle defaultLineStyle;
        private List<DataPoint> smoothedPoints;
        public LineSeries()
        {
            this.StrokeThickness = 2;
            this.LineJoin = LineJoin.Bevel;
            this.LineStyle = LineStyle.Automatic;

            this.Color = OxyColors.Automatic;
            this.BrokenLineColor = OxyColors.Undefined;

            this.MarkerFill = OxyColors.Automatic;
            this.MarkerStroke = OxyColors.Automatic;
            this.MarkerResolution = 0;
            this.MarkerSize = 3;
            this.MarkerStrokeThickness = 1;
            this.MarkerType = MarkerType.None;

            this.MinimumSegmentLength = 2;

            this.CanTrackerInterpolatePoints = true;
            this.LabelMargin = 6;
            this.smoothedPoints = new List<DataPoint>();
        }


        public OxyColor Color { get; set; }
        public OxyColor BrokenLineColor { get; set; }
        public LineStyle BrokenLineStyle { get; set; }
        public double BrokenLineThickness { get; set; }
        public double[] Dashes { get; set; }
        public Action<List<ScreenPoint>, List<ScreenPoint>> Decimator { get; set; }
        public string LabelFormatString { get; set; }
        public double LabelMargin { get; set; }
        public LineJoin LineJoin { get; set; }
        public LineStyle LineStyle { get; set; }
        public LineLegendPosition LineLegendPosition { get; set; }
        public OxyColor MarkerFill { get; set; }
        public ScreenPoint[] MarkerOutline { get; set; }
        public int MarkerResolution { get; set; }
        public double MarkerSize { get; set; }
        public OxyColor MarkerStroke { get; set; }
        public double MarkerStrokeThickness { get; set; }
        public MarkerType MarkerType { get; set; }
        public double MinimumSegmentLength { get; set; }
        public IInterpolationAlgorithm InterpolationAlgorithm { get; set; }
        public double StrokeThickness { get; set; }
        public OxyColor ActualColor
        {
            get
            {
                return this.Color.GetActualColor(this.defaultColor);
            }
        }

        public OxyColor ActualMarkerFill
        {
            get
            {
                return this.MarkerFill.GetActualColor(this.defaultMarkerFill);
            }
        }

        protected LineStyle ActualLineStyle
        {
            get
            {
                return this.LineStyle != LineStyle.Automatic ? this.LineStyle : this.defaultLineStyle;
            }
        }

        protected double[] ActualDashArray
        {
            get
            {
                return this.Dashes ?? this.ActualLineStyle.GetDashArray();
            }
        }

        protected List<DataPoint> SmoothedPoints
        {
            get
            {
                return this.smoothedPoints;
            }
        }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if (interpolate)
            {
                // Cannot interpolate if there is no line
                if (this.ActualColor.IsInvisible() || this.StrokeThickness.Equals(0))
                {
                    return null;
                }

                if (!this.CanTrackerInterpolatePoints)
                {
                    return null;
                }
            }

            if (interpolate && this.InterpolationAlgorithm != null)
            {
                var result = this.GetNearestInterpolatedPointInternal(this.SmoothedPoints, point);
                if (result != null)
                {
                    result.Text = StringHelper.Format(
                        this.ActualCulture,
                        this.TrackerFormatString,
                        result.Item,
                        this.Title,
                        this.XAxis.Title ?? XYAxisSeries.DefaultXAxisTitle,
                        this.XAxis.GetValue(result.DataPoint.X),
                        this.YAxis.Title ?? XYAxisSeries.DefaultYAxisTitle,
                        this.YAxis.GetValue(result.DataPoint.Y));
                }

                return result;
            }

            return base.GetNearestPoint(point, interpolate);
        }

        public override void Render(IRenderContext rc)
        {
            var actualPoints = this.ActualPoints;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            this.VerifyAxes();

            this.RenderPoints(rc, actualPoints);

            if (this.LabelFormatString != null)
            {
                // render point labels (not optimized for performance)
                this.RenderPointLabels(rc);
            }

            if (this.LineLegendPosition != LineLegendPosition.None && !string.IsNullOrEmpty(this.Title))
            {
                // renders a legend on the line
                this.RenderLegendOnLine(rc);
            }
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double ymid = (legendBox.Top + legendBox.Bottom) / 2;
            var pts = new[] { new ScreenPoint(legendBox.Left, ymid), new ScreenPoint(legendBox.Right, ymid) };
            rc.DrawLine(
                pts,
                this.GetSelectableColor(this.ActualColor),
                this.StrokeThickness,
                this.EdgeRenderingMode,
                this.ActualDashArray);
            var midpt = new ScreenPoint(xmid, ymid);
            rc.DrawMarker(
                midpt,
                this.MarkerType,
                this.MarkerOutline,
                this.MarkerSize,
                this.ActualMarkerFill,
                this.MarkerStroke,
                this.MarkerStrokeThickness,
                this.EdgeRenderingMode);
        }


        protected internal override void SetDefaultValues()
        {
            if (this.LineStyle == LineStyle.Automatic)
            {
                this.defaultLineStyle = this.PlotModel.GetDefaultLineStyle();
            }

            if (this.Color.IsAutomatic())
            {
                this.defaultColor = this.PlotModel.GetDefaultColor();
            }

            if (this.MarkerFill.IsAutomatic())
            {
                // No color was explicitly provided. Use the line color if it was set, else use default.
                this.defaultMarkerFill = this.Color.IsAutomatic() ? this.defaultColor : this.Color;
            }
        }


        protected internal override void UpdateMaxMin()
        {
            if (this.InterpolationAlgorithm != null)
            {
                // Update the max/min from the control points
                base.UpdateMaxMin();

                // Make sure the smooth points are re-evaluated.
                this.ResetSmoothedPoints();

                if (this.SmoothedPoints.Count == 0)
                {
                    return;
                }

                // Update the max/min from the smoothed points
                this.MinX = this.SmoothedPoints.Where(x => !double.IsNaN(x.X)).Min(x => x.X);
                this.MinY = this.SmoothedPoints.Where(x => !double.IsNaN(x.Y)).Min(x => x.Y);
                this.MaxX = this.SmoothedPoints.Where(x => !double.IsNaN(x.X)).Max(x => x.X);
                this.MaxY = this.SmoothedPoints.Where(x => !double.IsNaN(x.Y)).Max(x => x.Y);
            }
            else
            {
                base.UpdateMaxMin();
            }
        }

        protected void RenderPoints(IRenderContext rc, IList<DataPoint> points)
        {
            var lastValidPoint = new ScreenPoint?();
            var areBrokenLinesRendered = this.BrokenLineThickness > 0 && this.BrokenLineStyle != LineStyle.None;
            var dashArray = areBrokenLinesRendered ? this.BrokenLineStyle.GetDashArray() : null;
            var broken = areBrokenLinesRendered ? new List<ScreenPoint>(2) : null;

            if (this.contiguousScreenPointsBuffer == null)
            {
                this.contiguousScreenPointsBuffer = new List<ScreenPoint>(points.Count);
            }

			int startIdx = 0;
			double xmax = double.MaxValue;

			if (this.IsXMonotonic)
			{
				// determine render range
				var xmin = this.XAxis.ClipMinimum;
				xmax = this.XAxis.ClipMaximum;
				this.WindowStartIndex = this.UpdateWindowStartIndex(points, point => point.X, xmin, this.WindowStartIndex);

				startIdx = this.WindowStartIndex;
			}

			for (int i = startIdx; i < points.Count; i++)
	        {
				if (!this.ExtractNextContiguousLineSegment(points, ref i, ref lastValidPoint, xmax, broken, this.contiguousScreenPointsBuffer))
		        {
			        break;
		        }

				if (areBrokenLinesRendered)
				{
					if (broken.Count > 0)
					{
						var actualBrokenLineColor = this.BrokenLineColor.IsAutomatic()
														? this.ActualColor
														: this.BrokenLineColor;

						rc.DrawLineSegments(
							broken,
							actualBrokenLineColor,
							this.BrokenLineThickness,
                            this.EdgeRenderingMode,
							dashArray,
							this.LineJoin);
						broken.Clear();
					}
				}
				else
				{
					lastValidPoint = null;
				}

				if (this.Decimator != null)
				{
					if (this.decimatorBuffer == null)
					{
						this.decimatorBuffer = new List<ScreenPoint>(this.contiguousScreenPointsBuffer.Count);
					}
					else
					{
						this.decimatorBuffer.Clear();
					}

					this.Decimator(this.contiguousScreenPointsBuffer, this.decimatorBuffer);
					this.RenderLineAndMarkers(rc, this.decimatorBuffer);
				}
				else
				{
					this.RenderLineAndMarkers(rc, this.contiguousScreenPointsBuffer);
				}

				this.contiguousScreenPointsBuffer.Clear();
			}
        }

	    protected bool ExtractNextContiguousLineSegment(
			IList<DataPoint> points,
			ref int pointIdx,
			ref ScreenPoint? previousContiguousLineSegmentEndPoint,
			double xmax,
            List<ScreenPoint> broken,
            List<ScreenPoint> contiguous)
        {
            DataPoint currentPoint = default(DataPoint);
		    bool hasValidPoint = false;
		    
            // Skip all undefined points
		    for (; pointIdx < points.Count; pointIdx++)
		    {
				currentPoint = points[pointIdx];
			    if (currentPoint.X > xmax)
			    {
				    return false;
			    }
			    
			    if (hasValidPoint = this.IsValidPoint(currentPoint))
			    {
				    break;
			    }
		    }

		    if (!hasValidPoint)
		    {
			    return false;
		    }

            var screenPoint = this.Transform(currentPoint);

            if (previousContiguousLineSegmentEndPoint.HasValue)
            {
                broken.Add(previousContiguousLineSegmentEndPoint.Value);
                broken.Add(screenPoint);
            }

            contiguous.Add(screenPoint);

			int clipCount = 0;
			for (pointIdx++; pointIdx < points.Count; pointIdx++)
		    {
				currentPoint = points[pointIdx];
				clipCount += currentPoint.X > xmax ? 1 : 0;
				if (clipCount > 1)
				{
					break;
				}
				if (!this.IsValidPoint(currentPoint))
			    {
				    break;
			    }

				screenPoint = this.Transform(currentPoint);
				contiguous.Add(screenPoint);
			}

			previousContiguousLineSegmentEndPoint = screenPoint;

            return true;
        }

        protected void RenderPointLabels(IRenderContext rc)
        {
            int index = -1;
            foreach (var point in this.ActualPoints)
            {
                index++;

                if (!this.IsValidPoint(point))
                {
                    continue;
                }

                var pt = this.Transform(point) + new ScreenVector(0, -this.LabelMargin);

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

        protected void RenderLegendOnLine(IRenderContext rc)
        {
            // Find the position
            DataPoint point;
            HorizontalAlignment ha;
            var va = VerticalAlignment.Middle;
            double dx = 4;

            switch (this.LineLegendPosition)
            {
                case LineLegendPosition.Start:
                    point = this.ActualPoints[0];
                    ha = HorizontalAlignment.Right;
                    dx = -dx;
                    break;
                case LineLegendPosition.End:
                    point = this.ActualPoints[this.ActualPoints.Count - 1];
                    ha = HorizontalAlignment.Left;
                    break;
                case LineLegendPosition.None:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.Orientate(ref ha, ref va);
            var pt = this.Transform(point) + this.Orientate(new ScreenVector(dx, 0));

            // Render the legend
            rc.DrawText(
                pt,
                this.Title,
                this.ActualTextColor,
                this.ActualFont,
                this.ActualFontSize,
                this.ActualFontWeight,
                0,
                ha,
                va);
        }

        protected virtual void RenderLineAndMarkers(IRenderContext rc, IList<ScreenPoint> pointsToRender)
        {
            var screenPoints = pointsToRender;
            if (this.InterpolationAlgorithm != null)
            {
                // spline smoothing (should only be used on small datasets)
                var resampledPoints = ScreenPointHelper.ResamplePoints(pointsToRender, this.MinimumSegmentLength);
                screenPoints = this.InterpolationAlgorithm.CreateSpline(resampledPoints, false, 0.25);
            }

            // clip the line segments with the clipping rectangle
            if (this.StrokeThickness > 0 && this.ActualLineStyle != LineStyle.None)
            {
                this.RenderLine(rc, screenPoints);
            }

            if (this.MarkerType != MarkerType.None)
            {
                var markerBinOffset = this.MarkerResolution > 0 ? this.Transform(this.MinX, this.MinY) : default(ScreenPoint);

                rc.DrawMarkers(
                    pointsToRender, 
                    this.MarkerType, 
                    this.MarkerOutline, 
                    new[] { this.MarkerSize }, 
                    this.ActualMarkerFill, 
                    this.MarkerStroke, 
                    this.MarkerStrokeThickness, 
                    this.EdgeRenderingMode,
                    this.MarkerResolution, 
                    markerBinOffset);
            }
        }

        protected virtual void RenderLine(IRenderContext rc, IList<ScreenPoint> pointsToRender)
        {
            var dashArray = this.ActualDashArray;

            if (this.outputBuffer == null)
            {
                this.outputBuffer = new List<ScreenPoint>(pointsToRender.Count);
            }

            rc.DrawReducedLine(
                pointsToRender, 
                this.MinimumSegmentLength * this.MinimumSegmentLength, 
                this.GetSelectableColor(this.ActualColor), 
                this.StrokeThickness, 
                this.EdgeRenderingMode,
                dashArray, 
                this.LineJoin, 
                this.outputBuffer);
        }

        protected virtual void ResetSmoothedPoints()
        {
            double tolerance = Math.Abs(Math.Max(this.MaxX - this.MinX, this.MaxY - this.MinY) / ToleranceDivisor);
            this.smoothedPoints = this.InterpolationAlgorithm.CreateSpline(this.ActualPoints, false, tolerance);
        }

        protected class Segment
        {
            public Segment(DataPoint point1, DataPoint point2)
            {
                this.Point1 = point1;
                this.Point2 = point2;
            }

            public DataPoint Point1 { get; private set; }
            public DataPoint Point2 { get; private set; }
        }
    }
}
