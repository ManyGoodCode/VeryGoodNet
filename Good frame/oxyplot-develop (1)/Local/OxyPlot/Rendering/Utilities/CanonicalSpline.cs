namespace OxyPlot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CanonicalSpline : IInterpolationAlgorithm 
    {
        public double Tension { get; }

        public CanonicalSpline(double tension)
        {
            this.Tension = tension;
        }

        public List<DataPoint> CreateSpline(List<DataPoint> points, bool isClosed, double tolerance)
        {
            return CreateSpline(points, this.Tension, null, isClosed, tolerance);
        }

        public List<ScreenPoint> CreateSpline(IList<ScreenPoint> points, bool isClosed, double tolerance)
        {
            return CreateSpline(points, this.Tension, null, isClosed, tolerance);
        }

        internal static List<DataPoint> CreateSpline(List<DataPoint> points, double tension, IList<double> tensions, bool isClosed, double tolerance)
        {
            var screenPoints = points.Select(p => new ScreenPoint(p.X, p.Y)).ToList();
            var interpolatedScreenPoints = CreateSpline(screenPoints, tension, tensions, isClosed, tolerance);
            var interpolatedDataPoints = new List<DataPoint>(interpolatedScreenPoints.Count);

            foreach (var s in interpolatedScreenPoints)
            {
                interpolatedDataPoints.Add(new DataPoint(s.X, s.Y));
            }

            return interpolatedDataPoints;
        }

        internal static List<ScreenPoint> CreateSpline(
            IList<ScreenPoint> points, double tension, IList<double> tensions, bool isClosed, double tolerance)
        {
            var result = new List<ScreenPoint>();
            if (points == null)
            {
                return result;
            }

            int n = points.Count;
            if (n < 1)
            {
                return result;
            }

            if (n < 2)
            {
                result.AddRange(points);
                return result;
            }

            if (n == 2)
            {
                if (!isClosed)
                {
                    Segment(result, points[0], points[0], points[1], points[1], tension, tension, tolerance);
                }
                else
                {
                    Segment(result, points[1], points[0], points[1], points[0], tension, tension, tolerance);
                    Segment(result, points[0], points[1], points[0], points[1], tension, tension, tolerance);
                }
            }
            else
            {
                bool useTensionCollection = tensions != null && tensions.Count > 0;

                for (int i = 0; i < n; i++)
                {
                    double t1 = useTensionCollection ? tensions[i % tensions.Count] : tension;
                    double t2 = useTensionCollection ? tensions[(i + 1) % tensions.Count] : tension;

                    if (i == 0)
                    {
                        Segment(
                            result,
                            isClosed ? points[n - 1] : points[0],
                            points[0],
                            points[1],
                            points[2],
                            t1,
                            t2,
                            tolerance);
                    }
                    else if (i == n - 2)
                    {
                        Segment(
                            result,
                            points[i - 1],
                            points[i],
                            points[i + 1],
                            isClosed ? points[0] : points[i + 1],
                            t1,
                            t2,
                            tolerance);
                    }
                    else if (i == n - 1)
                    {
                        if (isClosed)
                        {
                            Segment(result, points[i - 1], points[i], points[0], points[1], t1, t2, tolerance);
                        }
                    }
                    else
                    {
                        Segment(result, points[i - 1], points[i], points[i + 1], points[i + 2], t1, t2, tolerance);
                    }
                }
            }

            return result;
        }

        private static void Segment(
            IList<ScreenPoint> points,
            ScreenPoint pt0,
            ScreenPoint pt1,
            ScreenPoint pt2,
            ScreenPoint pt3,
            double t1,
            double t2,
            double tolerance,
            int maxSegments = 1000)
        {
            double sx1 = t1 * (pt2.X - pt0.X);
            double sy1 = t1 * (pt2.Y - pt0.Y);
            double sx2 = t2 * (pt3.X - pt1.X);
            double sy2 = t2 * (pt3.Y - pt1.Y);

            double ax = sx1 + sx2 + (2 * pt1.X) - (2 * pt2.X);
            double ay = sy1 + sy2 + (2 * pt1.Y) - (2 * pt2.Y);
            double bx = (-2 * sx1) - sx2 - (3 * pt1.X) + (3 * pt2.X);
            double by = (-2 * sy1) - sy2 - (3 * pt1.Y) + (3 * pt2.Y);

            double cx = sx1;
            double cy = sy1;
            double dx = pt1.X;
            double dy = pt1.Y;

            var num = Math.Min(maxSegments, (int)((Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y)) / tolerance));

            for (int i = 1; i < num; i++)
            {
                double t = (double)i / (num - 1);
                var pt = new ScreenPoint(
                    (ax * t * t * t) + (bx * t * t) + (cx * t) + dx,
                    (ay * t * t * t) + (by * t * t) + (cy * t) + dy);
                points.Add(pt);
            }
        }
    }
}