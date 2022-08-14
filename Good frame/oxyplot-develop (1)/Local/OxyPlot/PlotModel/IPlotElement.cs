namespace OxyPlot
{
    public interface IPlotElement
    {
        int GetElementHashCode();

        OxyRect GetClippingRect();
    }
}
