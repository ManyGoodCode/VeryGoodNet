namespace OxyPlot
{
    using System.Collections.Generic;

    public interface IInterpolationAlgorithm {
        List<DataPoint> CreateSpline(List<DataPoint> points, bool isClosed, double tolerance);

        List<ScreenPoint> CreateSpline(IList<ScreenPoint> points, bool isClosed, double tolerance);
    }
}