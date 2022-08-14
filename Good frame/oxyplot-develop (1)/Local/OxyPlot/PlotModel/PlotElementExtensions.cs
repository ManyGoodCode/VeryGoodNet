namespace OxyPlot
{
    public static class PlotElementExtensions
    {
        public static DataPoint InverseTransform(this IXyAxisPlotElement element, double x, double y)
        {
            return element.InverseTransform(new ScreenPoint(x, y));
        }

        public static bool IsTransposed(this ITransposablePlotElement element)
        {
            return element.XAxis.IsVertical();
        }

        public static ScreenPoint Orientate(this ITransposablePlotElement element, ScreenPoint point)
        {
            return element.IsTransposed() ? new ScreenPoint(point.Y, point.X) : point;
        }

        public static ScreenVector Orientate(this ITransposablePlotElement element, ScreenVector vector)
        {
            vector = new ScreenVector(
                element.XAxis.IsReversed ? -vector.X : vector.X,
                element.YAxis.IsReversed ? -vector.Y : vector.Y);
            return element.IsTransposed() ? new ScreenVector(-vector.Y, -vector.X) : vector;
        }

        public static void Orientate(this ITransposablePlotElement element, ref HorizontalAlignment ha, ref VerticalAlignment va)
        {
            if (element.XAxis.IsReversed)
            {
                ha = (HorizontalAlignment)(-(int)ha);
            }

            if (element.YAxis.IsReversed)
            {
                va = (VerticalAlignment)(-(int)va);
            }

            if (element.IsTransposed())
            {
                HorizontalAlignment orientatedHa = (HorizontalAlignment)(-(int)va);
                va = (VerticalAlignment)(-(int)ha);
                ha = orientatedHa;
            }
        }

        public static ScreenPoint Transform(this IXyAxisPlotElement element, double x, double y)
        {
            return element.Transform(new DataPoint(x, y));
        }
    }
}
