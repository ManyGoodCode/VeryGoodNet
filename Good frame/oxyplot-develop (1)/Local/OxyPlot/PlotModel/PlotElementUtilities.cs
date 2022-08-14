namespace OxyPlot
{
    public static class PlotElementUtilities
    {
        public static OxyRect GetClippingRect(IXyAxisPlotElement element)
        {
            OxyRect xrect = new OxyRect(element.XAxis.ScreenMin, element.XAxis.ScreenMax);
            OxyRect yrect = new OxyRect(element.YAxis.ScreenMin, element.YAxis.ScreenMax);
            return xrect.Intersect(yrect);
        }

        public static DataPoint InverseTransform(IXyAxisPlotElement element, ScreenPoint p)
        {
            return element.XAxis.InverseTransform(p.X, p.Y, element.YAxis);
        }

        public static DataPoint InverseTransformOrientated(ITransposablePlotElement element, ScreenPoint p)
        {
            return InverseTransform(element, element.Orientate(p));
        }

        public static ScreenPoint Transform(IXyAxisPlotElement element, DataPoint p)
        {
            return element.XAxis.Transform(p.X, p.Y, element.YAxis);
        }


        public static ScreenPoint TransformOrientated(ITransposablePlotElement element, DataPoint p)
        {
            return element.Orientate(Transform(element, p));
        }
    }
}
