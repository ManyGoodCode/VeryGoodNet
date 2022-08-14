namespace OxyPlot
{
    using OxyPlot.Axes;

    public interface IXyAxisPlotElement : IPlotElement
    {
        Axis XAxis { get; }

        Axis YAxis { get; }

        ScreenPoint Transform(DataPoint p);

        DataPoint InverseTransform(ScreenPoint p);
    }
}
