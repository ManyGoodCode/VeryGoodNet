namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot.Axes;

    public abstract class XYAxisSeries : ItemsSeries, ITransposablePlotElement
    {
        public const string DefaultTrackerFormatString = "{0}\n{1}: {2}\n{3}: {4}";
        protected const string DefaultXAxisTitle = "X";
        protected const string DefaultYAxisTitle = "Y";
        protected XYAxisSeries()
        {
            this.TrackerFormatString = DefaultTrackerFormatString;
        }

        public double MaxX { get; protected set; }
        public double MaxY { get; protected set; }

        public double MinX { get; protected set; }
        public double MinY { get; protected set; }
        public Axis XAxis { get; private set; }
        public string XAxisKey { get; set; }
        public Axis YAxis { get; private set; }

        public string YAxisKey { get; set; }
        protected bool IsXMonotonic { get; set; }
        protected int WindowStartIndex { get; set; }

        public override OxyRect GetClippingRect()
        {
            return PlotElementUtilities.GetClippingRect(this);
        }

        public OxyRect GetScreenRectangle()
        {
            return this.GetClippingRect();
        }

        public DataPoint InverseTransform(ScreenPoint p)
        {
            return PlotElementUtilities.InverseTransformOrientated(this, p);
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
        }

        public ScreenPoint Transform(DataPoint p)
        {
            return PlotElementUtilities.TransformOrientated(this, p);
        }


        protected internal override bool AreAxesRequired()
        {
            return true;
        }

        protected internal override void EnsureAxes()
        {
            this.XAxis = this.XAxisKey != null ?
                         this.PlotModel.GetAxis(this.XAxisKey) :
                         this.PlotModel.DefaultXAxis;

            this.YAxis = this.YAxisKey != null ?
                         this.PlotModel.GetAxis(this.YAxisKey) :
                         this.PlotModel.DefaultYAxis;
        }

        protected internal override bool IsUsing(Axis axis)
        {
            return false;
        }

        protected internal override void SetDefaultValues()
        {
        }

        protected internal override void UpdateAxisMaxMin()
        {
            this.XAxis.Include(this.MinX);
            this.XAxis.Include(this.MaxX);
            this.YAxis.Include(this.MinY);
            this.YAxis.Include(this.MaxY);
        }

        protected internal override void UpdateData()
        {
            this.WindowStartIndex = 0;
        }


        protected internal override void UpdateMaxMin()
        {
            this.MinX = this.MinY = this.MaxX = this.MaxY = double.NaN;
        }

        protected TrackerHitResult GetNearestInterpolatedPointInternal(List<DataPoint> points, ScreenPoint point)
        {
            return this.GetNearestInterpolatedPointInternal(points, 0, point);
        }


        protected TrackerHitResult GetNearestInterpolatedPointInternal(List<DataPoint> points, int startIdx, ScreenPoint point)
        {
            if (this.XAxis == null || this.YAxis == null || points == null)
            {
                return null;
            }

            var spn = default(ScreenPoint);
            var dpn = default(DataPoint);
            double index = -1;

            double minimumDistance = double.MaxValue;

            for (int i = startIdx; i + 1 < points.Count; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                if (!this.IsValidPoint(p1) || !this.IsValidPoint(p2))
                {
                    continue;
                }

                var sp1 = this.Transform(p1);
                var sp2 = this.Transform(p2);

                var spl = ScreenPointHelper.FindPointOnLine(point, sp1, sp2);

                if (ScreenPoint.IsUndefined(spl))
                {
                    continue;
                }

                double l2 = (point - spl).LengthSquared;

                if (l2 < minimumDistance)
                {
                    double segmentLength = (sp2 - sp1).Length;
                    double u = segmentLength > 0 ? (spl - sp1).Length / segmentLength : 0;
                    dpn = this.InverseTransform(spl);
                    spn = spl;
                    minimumDistance = l2;
                    index = i + u;
                }
            }

            if (minimumDistance < double.MaxValue)
            {
                var item = this.GetItem((int)Math.Round(index));
                return new TrackerHitResult
                {
                    Series = this,
                    DataPoint = dpn,
                    Position = spn,
                    Item = item,
                    Index = index
                };
            }

            return null;
        }

        protected TrackerHitResult GetNearestPointInternal(IEnumerable<DataPoint> points, ScreenPoint point)
        {
            return this.GetNearestPointInternal(points, 0, point);
        }

        protected TrackerHitResult GetNearestPointInternal(IEnumerable<DataPoint> points, int startIdx, ScreenPoint point)
        {
            var spn = default(ScreenPoint);
            var dpn = default(DataPoint);
            double index = -1;

            double minimumDistance = double.MaxValue;
            int i = 0;
            foreach (var p in points.Skip(startIdx))
            {
                if (!this.IsValidPoint(p))
                {
                    i++;
                    continue;
                }

                var sp = this.Transform(p.x, p.y);
                double d2 = (sp - point).LengthSquared;

                if (d2 < minimumDistance)
                {
                    dpn = p;
                    spn = sp;
                    minimumDistance = d2;
                    index = i;
                }

                i++;
            }

            if (minimumDistance < double.MaxValue)
            {
                var item = this.GetItem((int)Math.Round(index));
                return new TrackerHitResult
                {
                    Series = this,
                    DataPoint = dpn,
                    Position = spn,
                    Item = item,
                    Index = index
                };
            }

            return null;
        }

        protected virtual bool IsValidPoint(DataPoint pt)
        {
            return
                this.XAxis != null && this.XAxis.IsValidValue(pt.X) &&
                this.YAxis != null && this.YAxis.IsValidValue(pt.Y);
        }

        protected bool IsValidPoint(double x, double y)
        {
            return
                this.XAxis != null && this.XAxis.IsValidValue(x) &&
                this.YAxis != null && this.YAxis.IsValidValue(y);
        }

        protected void InternalUpdateMaxMin(List<DataPoint> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            this.IsXMonotonic = true;

            if (points.Count == 0)
            {
                return;
            }

            double minx = this.MinX;
            double miny = this.MinY;
            double maxx = this.MaxX;
            double maxy = this.MaxY;

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

            double lastX = double.MinValue;
            foreach (var pt in points)
            {
                double x = pt.X;
                double y = pt.Y;

                // Check if the point is valid
                if (!this.IsValidPoint(pt))
                {
                    continue;
                }

                if (x < lastX)
                {
                    this.IsXMonotonic = false;
                }

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

                lastX = x;
            }

            if (minx < double.MaxValue)
            {
                if (minx < this.XAxis.FilterMinValue)
                {
                    minx = this.XAxis.FilterMinValue;
                }

                this.MinX = minx;
            }

            if (miny < double.MaxValue)
            {
                if (miny < this.YAxis.FilterMinValue)
                {
                    miny = this.YAxis.FilterMinValue;
                }

                this.MinY = miny;
            }

            if (maxx > double.MinValue)
            {
                if (maxx > this.XAxis.FilterMaxValue)
                {
                    maxx = this.XAxis.FilterMaxValue;
                }

                this.MaxX = maxx;
            }

            if (maxy > double.MinValue)
            {
                if (maxy > this.YAxis.FilterMaxValue)
                {
                    maxy = this.YAxis.FilterMaxValue;
                }

                this.MaxY = maxy;
            }
        }

        protected void InternalUpdateMaxMin<T>(List<T> items, Func<T, double> xf, Func<T, double> yf)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.IsXMonotonic = true;

            if (items.Count == 0)
            {
                return;
            }

            double minx = this.MinX;
            double miny = this.MinY;
            double maxx = this.MaxX;
            double maxy = this.MaxY;

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

            double lastX = double.MinValue;
            foreach (var item in items)
            {
                double x = xf(item);
                double y = yf(item);

                // Check if the point is valid
                if (!this.IsValidPoint(x, y))
                {
                    continue;
                }

                if (x < lastX)
                {
                    this.IsXMonotonic = false;
                }

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

                lastX = x;
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
        }

        protected void InternalUpdateMaxMin<T>(List<T> items, Func<T, double> xmin, Func<T, double> xmax, Func<T, double> ymin, Func<T, double> ymax)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.IsXMonotonic = true;

            if (items.Count == 0)
            {
                return;
            }

            double minx = this.MinX;
            double miny = this.MinY;
            double maxx = this.MaxX;
            double maxy = this.MaxY;

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

            double lastX0 = double.MinValue;
            double lastX1 = double.MinValue;
            foreach (var item in items)
            {
                double x0 = xmin(item);
                double x1 = xmax(item);
                double y0 = ymin(item);
                double y1 = ymax(item);

                if (!this.IsValidPoint(x0, y0) || !this.IsValidPoint(x1, y1))
                {
                    continue;
                }

                if (x0 < lastX0 || x1 < lastX1)
                {
                    this.IsXMonotonic = false;
                }

                if (x0 < minx)
                {
                    minx = x0;
                }

                if (x1 > maxx)
                {
                    maxx = x1;
                }

                if (y0 < miny)
                {
                    miny = y0;
                }

                if (y1 > maxy)
                {
                    maxy = y1;
                }

                lastX0 = x0;
                lastX1 = x1;
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
        }

        protected void VerifyAxes()
        {
            if (this.XAxis == null)
            {
                throw new InvalidOperationException("XAxis not defined.");
            }

            if (this.YAxis == null)
            {
                throw new InvalidOperationException("YAxis not defined.");
            }
        }

        protected int UpdateWindowStartIndex<T>(IList<T> items, Func<T, double> xgetter, double targetX, int lastIndex)
        {
            lastIndex = this.FindWindowStartIndex(items, xgetter, targetX, lastIndex);
            if (lastIndex > 0)
            {
                lastIndex--;
            }

            return lastIndex;
        }

        public int FindWindowStartIndex<T>(IList<T> items, Func<T, double> xgetter, double targetX, int initialGuess)
        {
            int start = 0;
            int nominalEnd = items.Count - 1;
            while (nominalEnd > 0 && double.IsNaN(xgetter(items[nominalEnd])))
                nominalEnd -= 1;
            int end = nominalEnd;
            int curGuess = Math.Max(0, Math.Min(end, initialGuess));

            double GetX(int index)
            {
                while (index <= nominalEnd)
                {
                    double guessX = xgetter(items[index]);
                    if (double.IsNaN(guessX))
                        index += 1;
                    else
                        return guessX;
                }
                return xgetter(items[nominalEnd]);
            }

            while (start < end)
            {
                double guessX = GetX(curGuess);
                if (guessX.Equals(targetX))
                {
                    start = curGuess;
                    break;
                }
                else if (guessX > targetX)
                {
                    end = curGuess - 1;
                }
                else
                { 
                    start = curGuess;
                }

                if (start >= end)
                {
                    break;
                }

                double endX = GetX(end);
                double startX = GetX(start);

                var m = (end - start + 1) / (endX - startX);
                
                curGuess = start + (int)((targetX - startX) * m);
                curGuess = Math.Max(start + 1, Math.Min(curGuess, end));
            }

            while (start > 0 && (xgetter(items[start]) > targetX))
                start -= 1;

            return start;
        }
    }
}
