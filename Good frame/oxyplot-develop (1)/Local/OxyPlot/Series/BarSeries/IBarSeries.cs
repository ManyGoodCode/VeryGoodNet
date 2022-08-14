namespace OxyPlot.Series
{
    using OxyPlot.Axes;
    using System.Collections.Generic;

    public interface IBarSeries
    {
        double BarWidth { get; }
        CategoryAxis CategoryAxis { get; }
        bool IsVisible { get; }
        BarSeriesManager Manager { get; set; }
        PlotModel PlotModel { get; }
        Axis ValueAxis { get; }
        void UpdateValidData();
        IReadOnlyList<BarItemBase> ActualItems { get; }
    }
}
