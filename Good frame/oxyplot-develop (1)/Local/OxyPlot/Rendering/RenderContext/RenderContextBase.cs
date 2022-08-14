
namespace OxyPlot
{
    using System;
    using System.Collections.Generic;

    public abstract class RenderContextBase : IRenderContext
    {
        protected RenderContextBase()
        {
            this.RendersToScreen = true;
        }

        public static bool IsStraightLine(ScreenPoint p1, ScreenPoint p2)
        {
            const double epsilon = 1e-5;
            return Math.Abs(p1.X - p2.X) < epsilon || Math.Abs(p1.Y - p2.Y) < epsilon;
        }

        public static bool IsStraightLine(IList<ScreenPoint> points)
        {
            for (int i = 1; i < points.Count; i++)
            {
                if (!IsStraightLine(points[i - 1], points[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool RendersToScreen { get; set; }

        public virtual void DrawEllipse(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            ScreenPoint[] polygon = CreateEllipse(rect);
            this.DrawPolygon(polygon, fill, stroke, thickness, edgeRenderingMode, null, LineJoin.Miter);
        }

        public virtual void DrawEllipses(IList<OxyRect> rectangles, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            foreach (OxyRect r in rectangles)
            {
                this.DrawEllipse(r, fill, stroke, thickness, edgeRenderingMode);
            }
        }

        public abstract void DrawLine(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin);

        public virtual void DrawLineSegments(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            for (int i = 0; i + 1 < points.Count; i += 2)
            {
                this.DrawLine(new[] { points[i], points[i + 1] }, stroke, thickness, edgeRenderingMode, dashArray, lineJoin);
            }
        }

        public abstract void DrawPolygon(
            IList<ScreenPoint> points,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin);

        public virtual void DrawPolygons(
            IList<IList<ScreenPoint>> polygons,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            foreach (var polygon in polygons)
            {
                this.DrawPolygon(polygon, fill, stroke, thickness, edgeRenderingMode, dashArray, lineJoin);
            }
        }

        public virtual void DrawRectangle(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            ScreenPoint[] polygon = CreateRectangle(rect);
            this.DrawPolygon(polygon, fill, stroke, thickness, edgeRenderingMode, null, LineJoin.Miter);
        }

        public virtual void DrawRectangles(IList<OxyRect> rectangles, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            foreach (OxyRect r in rectangles)
            {
                this.DrawRectangle(r, fill, stroke, thickness, edgeRenderingMode);
            }
        }

        public abstract void DrawText(
            ScreenPoint p,
            string text,
            OxyColor fill,
            string fontFamily,
            double fontSize,
            double fontWeight,
            double rotate,
            HorizontalAlignment halign,
            VerticalAlignment valign,
            OxySize? maxSize);

        public abstract OxySize MeasureText(string text, string fontFamily, double fontSize, double fontWeight);

        public virtual void SetToolTip(string text)
        {
        }


        public virtual void CleanUp()
        {
        }

        public virtual void DrawImage(
            OxyImage source,
            double srcX,
            double srcY,
            double srcWidth,
            double srcHeight,
            double destX,
            double destY,
            double destWidth,
            double destHeight,
            double opacity,
            bool interpolate)
        {
        }

        public abstract void PopClip();

        public abstract void PushClip(OxyRect clippingRectangle);

        public abstract int ClipCount { get; }


        protected static ScreenPoint[] CreateEllipse(OxyRect rect, int n = 40)
        {
            double cx = rect.Center.X;
            double cy = rect.Center.Y;
            double dx = rect.Width / 2;
            double dy = rect.Height / 2;
            ScreenPoint[] points = new ScreenPoint[n];
            for (int i = 0; i < n; i++)
            {
                double t = Math.PI * 2 * i / n;
                points[i] = new ScreenPoint(cx + (Math.Cos(t) * dx), cy + (Math.Sin(t) * dy));
            }

            return points;
        }

        protected static ScreenPoint[] CreateRectangle(OxyRect rect)
        {
            return new[]
                       {
                           new ScreenPoint(rect.Left, rect.Top), new ScreenPoint(rect.Left, rect.Bottom),
                           new ScreenPoint(rect.Right, rect.Bottom), new ScreenPoint(rect.Right, rect.Top)
                       };
        }

        protected virtual bool ShouldUseAntiAliasingForRect(EdgeRenderingMode edgeRenderingMode)
        {
            switch (edgeRenderingMode)
            {
                case EdgeRenderingMode.PreferGeometricAccuracy:
                    return true;
                default:
                    return false;
            }
        }

        protected virtual bool ShouldUseAntiAliasingForEllipse(EdgeRenderingMode edgeRenderingMode)
        {
            switch (edgeRenderingMode)
            {
                case EdgeRenderingMode.PreferSpeed:
                    return false;
                default:
                    return true;
            }
        }

        protected virtual bool ShouldUseAntiAliasingForLine(EdgeRenderingMode edgeRenderingMode, IList<ScreenPoint> points)
        {
            switch (edgeRenderingMode)
            {
                case EdgeRenderingMode.PreferSpeed:
                case EdgeRenderingMode.PreferSharpness:
                case EdgeRenderingMode.Automatic when IsStraightLine(points):
                case EdgeRenderingMode.Adaptive when IsStraightLine(points):
                    return false;
                default:
                    return true;
            }
        }
    }
}
