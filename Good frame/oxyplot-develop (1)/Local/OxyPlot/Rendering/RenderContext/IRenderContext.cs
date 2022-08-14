namespace OxyPlot
{
    using System.Collections.Generic;

    public interface IRenderContext
    {
        bool RendersToScreen { get; }

        void DrawEllipse(
            OxyRect extents, 
            OxyColor fill, 
            OxyColor stroke, 
            double thickness,
            EdgeRenderingMode edgeRenderingMode);

        void DrawEllipses(
            IList<OxyRect> extents, 
            OxyColor fill, 
            OxyColor stroke, 
            double thickness, 
            EdgeRenderingMode edgeRenderingMode);

        void DrawLine(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Miter);

        void DrawLineSegments(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Miter);

        void DrawPolygon(
            IList<ScreenPoint> points,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Miter);

        void DrawPolygons(
            IList<IList<ScreenPoint>> polygons,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Miter);

        void DrawRectangle(
            OxyRect rectangle, 
            OxyColor fill, 
            OxyColor stroke, 
            double thickness, 
            EdgeRenderingMode edgeRenderingMode);

        void DrawRectangles(
            IList<OxyRect> rectangles, 
            OxyColor fill, 
            OxyColor stroke, 
            double thickness, 
            EdgeRenderingMode edgeRenderingMode);

        void DrawText(
            ScreenPoint p,
            string text,
            OxyColor fill,
            string fontFamily = null,
            double fontSize = 10,
            double fontWeight = FontWeights.Normal,
            double rotation = 0,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Top,
            OxySize? maxSize = null);


        OxySize MeasureText(string text, string fontFamily = null, double fontSize = 10, double fontWeight = 500);

        void SetToolTip(string text);

        void CleanUp();

        void DrawImage(
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
            bool interpolate);

        void PushClip(OxyRect clippingRectangle);

        void PopClip();

        int ClipCount { get; }
    }
}
