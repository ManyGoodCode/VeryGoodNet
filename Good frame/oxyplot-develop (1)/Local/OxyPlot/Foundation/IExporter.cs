namespace OxyPlot
{
    using System.IO;

    public interface IExporter
    {
        void Export(IPlotModel model, Stream stream);
    }
}