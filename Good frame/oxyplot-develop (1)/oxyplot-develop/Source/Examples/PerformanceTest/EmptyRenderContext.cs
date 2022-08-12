namespace PerformanceTest
{
    using System.Collections.Generic;

    using OxyPlot;

    public class EmptyRenderContext : IRenderContext
    {
        public bool RendersToScreen { get; set; } = true;

        public void CleanUp()
        {
        }

        public void DrawEllipse(OxyRect extents, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
        }

        public void DrawEllipses(IList<OxyRect> extents, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
        }

        public void DrawImage(OxyImage source, double srcX, double srcY, double srcWidth, double srcHeight, double destX, double destY, double destWidth, double destHeight, double opacity, bool interpolate)
        {
        }

        public void PushClip(OxyRect clippingRectangle)
        {
        }

        public void PopClip()
        {
        }

        public int ClipCount => 0;

        public void DrawLine(IList<ScreenPoint> points, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[] dashArray = null, LineJoin lineJoin = LineJoin.Miter)
        {
        }

        public void DrawLineSegments(IList<ScreenPoint> points, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[] dashArray = null, LineJoin lineJoin = LineJoin.Miter)
        {
        }

        public void DrawPolygon(IList<ScreenPoint> points, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[] dashArray = null, LineJoin lineJoin = LineJoin.Miter)
        {
        }

        public void DrawPolygons(IList<IList<ScreenPoint>> polygons, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[] dashArray = null, LineJoin lineJoin = LineJoin.Miter)
        {
        }

        public void DrawRectangle(OxyRect rectangle, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
        }

        public void DrawRectangles(IList<OxyRect> rectangles, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
        }

        public void DrawText(ScreenPoint p, string text, OxyColor fill, string fontFamily = null, double fontSize = 10, double fontWeight = 400, double rotation = 0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top, OxySize? maxSize = null)
        {
        }

        public OxySize MeasureText(string text, string fontFamily = null, double fontSize = 10, double fontWeight = 500)
        {
            return OxySize.Empty;
        }

        public void SetToolTip(string text)
        {
        }
    }
}
