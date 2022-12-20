
namespace OxyPlot.Axes
{
    public interface IColorAxis : IPlotElement
    {
        OxyColor GetColor(int paletteIndex);
        int GetPaletteIndex(double value);
    }
}
