namespace OxyPlot.Series
{
    public interface IStackableSeries : IBarSeries
    {
        bool IsStacked { get; }
        bool OverlapsStack { get; }
        string StackGroup { get; }
    }
}
