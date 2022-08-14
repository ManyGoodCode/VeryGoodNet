
namespace OxyPlot
{
    using System;
    using System.Collections.Generic;


    public static class ScreenPointHelper
    {
        public static ScreenPoint FindNearestPointOnPolyline(ScreenPoint point, IList<ScreenPoint> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            double minimumDistance = double.MaxValue;
            var nearestPoint = default(ScreenPoint);

            for (int i = 0; i + 1 < points.Count; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                if (ScreenPoint.IsUndefined(p1) || ScreenPoint.IsUndefined(p2))
                {
                    continue;
                }

                var nearestPointOnSegment = FindPointOnLine(point, p1, p2);

                if (ScreenPoint.IsUndefined(nearestPointOnSegment))
                {
                    continue;
                }

                double l2 = (point - nearestPointOnSegment).LengthSquared;

                if (l2 < minimumDistance)
                {
                    nearestPoint = nearestPointOnSegment;
                    minimumDistance = l2;
                }
            }

            return nearestPoint;
        }

        public static ScreenPoint FindPointOnLine(ScreenPoint p, ScreenPoint p1, ScreenPoint p2)
        {
            double dx = p2.x - p1.x;
            double dy = p2.y - p1.y;
            double u = FindPositionOnLine(p, p1, p2);

            if (double.IsNaN(u))
            {
                u = 0;
            }

            if (u < 0)
            {
                u = 0;
            }

            if (u > 1)
            {
                u = 1;
            }

            return new ScreenPoint(p1.x + (u * dx), p1.y + (u * dy));
        }

        public static double FindPositionOnLine(ScreenPoint p, ScreenPoint p1, ScreenPoint p2)
        {
            double dx = p2.x - p1.x;
            double dy = p2.y - p1.y;
            double u1 = ((p.x - p1.x) * dx) + ((p.y - p1.y) * dy);
            double u2 = (dx * dx) + (dy * dy);

            if (u2 < 1e-6)
            {
                return double.NaN;
            }

            return u1 / u2;
        }

        public static bool IsPointInPolygon(ScreenPoint p, IList<ScreenPoint> pts)
        {
            if (pts == null)
            {
                return false;
            }

            int nvert = pts.Count;
            bool c = false;
            for (int i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((pts[i].Y > p.Y) != (pts[j].Y > p.Y))
                    && (p.X < ((pts[j].X - pts[i].X) * ((p.Y - pts[i].Y) / (pts[j].Y - pts[i].Y))) + pts[i].X))
                {
                    c = !c;
                }
            }

            return c;
        }

        public static IList<ScreenPoint> ResamplePoints(IList<ScreenPoint> allPoints, double minimumDistance)
        {
            double minimumSquaredDistance = minimumDistance * minimumDistance;
            int n = allPoints.Count;
            var result = new List<ScreenPoint>(n);
            if (n > 0)
            {
                result.Add(allPoints[0]);
                int i0 = 0;
                for (int i = 1; i < n; i++)
                {
                    double distSquared = allPoints[i0].DistanceToSquared(allPoints[i]);
                    if (distSquared < minimumSquaredDistance && i != n - 1)
                    {
                        continue;
                    }

                    i0 = i;
                    result.Add(allPoints[i]);
                }
            }

            return result;
        }

        public static ScreenPoint GetCentroid(IList<ScreenPoint> points)
        {
            double cx = 0;
            double cy = 0;
            double a = 0;

            for (int i = 0; i < points.Count; i++)
            {
                int i1 = (i + 1) % points.Count;
                double da = (points[i].x * points[i1].y) - (points[i1].x * points[i].y);
                cx += (points[i].x + points[i1].x) * da;
                cy += (points[i].y + points[i1].y) * da;
                a += da;
            }

            a *= 0.5;
            cx /= 6 * a;
            cy /= 6 * a;
            return new ScreenPoint(cx, cy);
        }
    }
}