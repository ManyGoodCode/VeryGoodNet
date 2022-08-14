namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class ContourSeries : XYAxisSeries
    {
        public new const string DefaultTrackerFormatString = "{0}\n{1}: {2}\n{3}: {4}\n{5}: {6}";
        private List<Contour> contours;
        private List<ContourSegment> segments;

        private OxyColor defaultColor;

        public ContourSeries()
        {
            this.ContourLevelStep = double.NaN;
            this.LabelStep = 1;
            this.MultiLabel = false;
            this.LabelSpacing = 150;
            this.LabelBackground = OxyColor.FromAColor(220, OxyColors.White);

            this.Color = OxyColors.Automatic;
            this.StrokeThickness = 1.0;
            this.LineStyle = LineStyle.Solid;
            this.MinimumSegmentLength = 2;

            this.TrackerFormatString = DefaultTrackerFormatString;
        }

        public OxyColor Color { get; set; }

        public OxyColor ActualColor
        {
            get { return this.Color.GetActualColor(this.defaultColor); }
        }

        public double[] ColumnCoordinates { get; set; }
        public double ContourLevelStep { get; set; }
        public double[] ContourLevels { get; set; }
        public OxyColor[] ContourColors { get; set; }
        public double[,] Data { get; set; }
        public OxyColor LabelBackground { get; set; }
        public string LabelFormatString { get; set; }
        public double LabelSpacing { get; set; }
        public bool MultiLabel { get; set; }
        public int LabelStep { get; set; }
        public LineStyle LineStyle { get; set; }
        public double[] RowCoordinates { get; set; }
        public double StrokeThickness { get; set; }

        public double MinimumSegmentLength { get; set; }

        public void CalculateContours()
        {
            if (this.Data == null)
            {
                return;
            }

            double[] actualContourLevels = this.ContourLevels;

            this.segments = new List<ContourSegment>();
            Conrec.RendererDelegate renderer = (startX, startY, endX, endY, contourLevel) =>
                this.segments.Add(new ContourSegment(new DataPoint(startX, startY), new DataPoint(endX, endY), contourLevel));

            if (actualContourLevels == null)
            {
                double max = this.Data[0, 0];
                double min = this.Data[0, 0];
                for (int i = 0; i < this.Data.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < this.Data.GetUpperBound(1); j++)
                    {
                        max = Math.Max(max, this.Data[i, j]);
                        min = Math.Min(min, this.Data[i, j]);
                    }
                }

                double actualStep = this.ContourLevelStep;
                if (double.IsNaN(actualStep))
                {
                    double range = max - min;
                    double step = range / 20;
                    double stepExp = Math.Round(Math.Log(Math.Abs(step), 10));
                    actualStep = Math.Pow(10, Math.Floor(stepExp));
                    this.ContourLevelStep = actualStep;
                }

                max = Math.Round(actualStep * (int)Math.Ceiling(max / actualStep), 14);
                min = Math.Round(actualStep * (int)Math.Floor(min / actualStep), 14);

                actualContourLevels = ArrayBuilder.CreateVector(min, max, actualStep);
            }

            Conrec.Contour(this.Data, this.ColumnCoordinates, this.RowCoordinates, actualContourLevels, renderer);

            this.JoinContourSegments();

            if (this.ContourColors != null && this.ContourColors.Length > 0)
            {
                foreach (var c in this.contours)
                {
                    // get the index of the contour's level
                    var index = IndexOf(actualContourLevels, c.ContourLevel);
                    if (index >= 0)
                    {
                        // clamp the index to the range of the ContourColors array
                        index = index % this.ContourColors.Length;
                        c.Color = this.ContourColors[index];
                    }
                }
            }
        }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            TrackerHitResult result = null;

            var xaxisTitle = this.XAxis.Title ?? "X";
            var yaxisTitle = this.YAxis.Title ?? "Y";
            var zaxisTitle = "Z";

            foreach (var c in this.contours)
            {
                var r = interpolate ? this.GetNearestInterpolatedPointInternal(c.Points, point) : this.GetNearestPointInternal(c.Points, point);
                if (r != null)
                {
                    if (result == null || result.Position.DistanceToSquared(point) > r.Position.DistanceToSquared(point))
                    {
                        result = r;
                        result.Text = StringHelper.Format(
                            this.ActualCulture,
                            this.TrackerFormatString,
                            null,
                            this.Title,
                            xaxisTitle,
                            this.XAxis.GetValue(r.DataPoint.X),
                            yaxisTitle,
                            this.YAxis.GetValue(r.DataPoint.Y),
                            zaxisTitle,
                            c.ContourLevel);
                    }
                }
            }

            return result;
        }

        public override void Render(IRenderContext rc)
        {
            if (this.contours == null)
            {
                this.CalculateContours();
            }

            if (this.contours.Count == 0)
            {
                return;
            }

            this.VerifyAxes();

            var contourLabels = new List<ContourLabel>();
            var dashArray = this.LineStyle.GetDashArray();

            foreach (var contour in this.contours)
            {
                if (this.StrokeThickness <= 0 || this.LineStyle == LineStyle.None)
                {
                    continue;
                }

                var transformedPoints = contour.Points.Select(this.Transform).ToArray();

                var strokeColor = contour.Color.GetActualColor(this.ActualColor);

                rc.DrawReducedLine(
                    transformedPoints,
                    this.MinimumSegmentLength * this.MinimumSegmentLength,
                    this.GetSelectableColor(strokeColor),
                    this.StrokeThickness,
                    this.EdgeRenderingMode,
                    dashArray,
                    LineJoin.Miter);

                // measure total contour length
                var contourLength = 0.0;
                for (int i = 1; i < transformedPoints.Length; i++)
                {
                    contourLength += (transformedPoints[i] - transformedPoints[i - 1]).Length;
                }

                // don't add label to contours, if ContourLevel is not close to LabelStep
                if (transformedPoints.Length <= 10 || (Math.Round(contour.ContourLevel / this.ContourLevelStep) % this.LabelStep != 0))
                {
                    continue;
                }

                if (!this.MultiLabel)
                {
                    this.AddContourLabels(contour, transformedPoints, contourLabels, (transformedPoints.Length - 1) * 0.5);
                    continue;
                }

                // calculate how many labels fit per contour
                var labelsCount = (int)(contourLength / this.LabelSpacing);
                if (labelsCount == 0)
                {
                    this.AddContourLabels(contour, transformedPoints, contourLabels, (transformedPoints.Length - 1) * 0.5);
                    continue;
                }

                var contourPartLength = 0.0;
                var contourPartLengthOld = 0.0;
                var intervalIndex = 1;
                var contourPartLengthTarget = 0.0;
                var contourFirstPartLengthTarget = (contourLength - ((labelsCount - 1) * this.LabelSpacing)) / 2;
                for (var j = 0; j < labelsCount; j++)
                {
                    var labelIndex = 0.0;

                    if (intervalIndex == 1)
                    {
                        contourPartLengthTarget = contourFirstPartLengthTarget;
                    }
                    else
                    {
                        contourPartLengthTarget = contourFirstPartLengthTarget + (j * this.LabelSpacing);
                    }

                    // find index of contour points where next label should be positioned
                    for (var k = intervalIndex; k < transformedPoints.Length; k++)
                    {
                        contourPartLength += (transformedPoints[k] - transformedPoints[k - 1]).Length;

                        if (contourPartLength > contourPartLengthTarget)
                        {
                            labelIndex = (k - 1) + ((contourPartLengthTarget - contourPartLengthOld) / (contourPartLength - contourPartLengthOld));
                            intervalIndex = k + 1;
                            break;
                        }

                        contourPartLengthOld = contourPartLength;
                    }

                    this.AddContourLabels(contour, transformedPoints, contourLabels, labelIndex);
                }
            }

            foreach (var cl in contourLabels)
            {
                this.RenderLabelBackground(rc, cl);
            }

            foreach (var cl in contourLabels)
            {
                this.RenderLabel(rc, cl);
            }
        }

        protected internal override void SetDefaultValues()
        {
            if (this.Color.IsAutomatic())
            {
                this.LineStyle = this.PlotModel.GetDefaultLineStyle();
                this.defaultColor = this.PlotModel.GetDefaultColor();
            }
        }

        protected internal override void UpdateMaxMin()
        {
            this.MinX = this.ColumnCoordinates.Min();
            this.MaxX = this.ColumnCoordinates.Max();
            this.MinY = this.RowCoordinates.Min();
            this.MaxY = this.RowCoordinates.Max();
        }

        private static int IndexOf(IList<double> values, double value)
        {
            double min = double.MaxValue;
            int index = -1;
            for (int i = 0; i < values.Count; i++)
            {
                var d = Math.Abs(values[i] - value);
                if (d < min)
                {
                    min = d;
                    index = i;
                }
            }

            return index;
        }

        private void AddContourLabels(Contour contour, ScreenPoint[] pts, ICollection<ContourLabel> contourLabels, double labelIndex)
        {
            if (pts.Length < 2)
            {
                return;
            }

            // Calculate position and angle of the label
            var i0 = (int)labelIndex;
            var i1 = i0 + 1;
            var dx = pts[i1].X - pts[i0].X;
            var dy = pts[i1].Y - pts[i0].Y;
            var x = pts[i0].X + (dx * (labelIndex - i0));
            var y = pts[i0].Y + (dy * (labelIndex - i0));

            var pos = new ScreenPoint(x, y);
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            if (angle > 90)
            {
                angle -= 180;
            }

            if (angle < -90)
            {
                angle += 180;
            }

            var formatString = string.Concat("{0:", this.LabelFormatString, "}");
            var text = string.Format(this.ActualCulture, formatString, contour.ContourLevel);
            contourLabels.Add(new ContourLabel { Position = pos, Angle = angle, Text = text });
        }

        private void JoinContourSegments(double epsFactor = 1e-10)
        {
            this.contours = new List<Contour>();

            static IEnumerable<SegmentPoint> GetPoints(ContourSegment segment)
            {
                var p1 = new SegmentPoint(segment.StartPoint);
                var p2 = new SegmentPoint(segment.EndPoint);
                p1.Partner = p2;
                p2.Partner = p1;
                yield return p1;
                yield return p2;
            }

            foreach (var group in this.segments.GroupBy(p => p.ContourLevel))
            {
                var level = group.Key;
                var points = group.SelectMany(GetPoints).OrderBy(p => p.Point.X).ToList();

                // first, go through the sorted points, find identical points and join them together 
                for (var i = 0; i < points.Count - 1; i++)
                {
                    var currentPoint = points[i];
                    if (currentPoint.Join != null)
                    {
                        continue;
                    }

                    var positionVectorLength = Math.Sqrt(Math.Pow(currentPoint.Point.X, 2) + Math.Pow(currentPoint.Point.Y, 2));
                    var eps = positionVectorLength * epsFactor;

                    var maxX = currentPoint.Point.X + eps;
                    var i2 = i + 1;
                    SegmentPoint joinPoint;

                    // search for a point with the same coordinates (within eps) as the current point
                    // as points are sorted by X, we typically only need to check the point immediately following the current point
                    while (true)
                    {
                        if (i2 >= points.Count)
                        {
                            joinPoint = null;
                            break;
                        }

                        joinPoint = points[i2];
                        i2++;
                        if (joinPoint.Join != null)
                        {
                            continue;
                        }

                        if (joinPoint.Point.X > maxX)
                        {
                            joinPoint = null;
                            break;
                        }

                        var distance = Math.Sqrt(Math.Pow(joinPoint.Point.X - currentPoint.Point.X, 2) + Math.Pow(joinPoint.Point.Y - currentPoint.Point.Y, 2));
                        if (distance < eps)
                        {
                            break;
                        }
                    }

                    // join the two points together
                    if (joinPoint != null)
                    {
                        currentPoint.Join = joinPoint;
                        joinPoint.Join = currentPoint;
                    }
                }

                // go through the points again, this time we follow the joined point chains to obtain the contours
                foreach (var segmentPoint in points)
                {
                    if (segmentPoint.Processed)
                    {
                        continue;
                    }

                    var currentPoint = segmentPoint;

                    // search for the beginning of the contour (or use the entry point if the contour is closed)
                    while (currentPoint.Join != null)
                    {
                        currentPoint = currentPoint.Join.Partner;
                        if (currentPoint == segmentPoint)
                        {
                            break;
                        }
                    }

                    var dataPoints = new List<DataPoint> { currentPoint.Point, currentPoint.Partner.Point };
                    currentPoint.Processed = true;
                    currentPoint = currentPoint.Partner;
                    currentPoint.Processed = true;

                    // follow the chain of joined points and add their coordinates until we find the last point of the contour (or complete a rotation)
                    while (currentPoint.Join != null)
                    {
                        currentPoint = currentPoint.Join;
                        if (currentPoint.Processed)
                        {
                            break;
                        }

                        currentPoint.Processed = true;
                        currentPoint = currentPoint.Partner;
                        currentPoint.Processed = true;
                        dataPoints.Add(currentPoint.Point);
                    }

                    var contour = new Contour(dataPoints, level);
                    this.contours.Add(contour);
                }
            }
        }

        private void RenderLabel(IRenderContext rc, ContourLabel cl)
        {
            if (this.ActualFontSize > 0)
            {
                rc.DrawText(
                    cl.Position,
                    cl.Text,
                    this.ActualTextColor,
                    this.ActualFont,
                    this.ActualFontSize,
                    this.ActualFontWeight,
                    cl.Angle,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Middle);
            }
        }

        private void RenderLabelBackground(IRenderContext rc, ContourLabel cl)
        {
            if (this.LabelBackground.IsInvisible())
            {
                return;
            }

            // Calculate background polygon
            var size = rc.MeasureText(cl.Text, this.ActualFont, this.ActualFontSize, this.ActualFontWeight);
            double a = cl.Angle / 180 * Math.PI;
            double dx = Math.Cos(a);
            double dy = Math.Sin(a);

            double ux = dx * 0.6;
            double uy = dy * 0.6;
            double vx = -dy * 0.5;
            double vy = dx * 0.5;
            double x = cl.Position.X;
            double y = cl.Position.Y;

            var bpts = new[]
                           {
                               new ScreenPoint(x - (size.Width * ux) - (size.Height * vx), y - (size.Width * uy) - (size.Height * vy)),
                               new ScreenPoint(x + (size.Width * ux) - (size.Height * vx), y + (size.Width * uy) - (size.Height * vy)),
                               new ScreenPoint(x + (size.Width * ux) + (size.Height * vx), y + (size.Width * uy) + (size.Height * vy)),
                               new ScreenPoint(x - (size.Width * ux) + (size.Height * vx), y - (size.Width * uy) + (size.Height * vy))
                           };
            rc.DrawPolygon(bpts, this.LabelBackground, OxyColors.Undefined, 0, this.EdgeRenderingMode);
        }


        private class SegmentPoint
        {
            public SegmentPoint(DataPoint point)
            {
                this.Point = point;
            }

            public bool Processed { get; set; }
            public SegmentPoint Partner { get; set; }
            public SegmentPoint Join { get; set; }
            public DataPoint Point { get; }
        }

        private class Contour
        {
            internal readonly double ContourLevel;
            internal readonly List<DataPoint> Points;
            public Contour(List<DataPoint> points, double contourLevel)
            {
                this.Points = points;
                this.ContourLevel = contourLevel;
                this.Color = OxyColors.Automatic;
            }

            public OxyColor Color { get; set; }
        }

        private class ContourLabel
        {
            public double Angle { get; set; }
            public ScreenPoint Position { get; set; }
            public string Text { get; set; }
        }

        private class ContourSegment
        {
            internal readonly double ContourLevel;
            internal readonly DataPoint EndPoint;
            internal readonly DataPoint StartPoint;

            public ContourSegment(DataPoint startPoint, DataPoint endPoint, double contourLevel)
            {
                this.ContourLevel = contourLevel;
                this.StartPoint = startPoint;
                this.EndPoint = endPoint;
            }
        }
    }
}
