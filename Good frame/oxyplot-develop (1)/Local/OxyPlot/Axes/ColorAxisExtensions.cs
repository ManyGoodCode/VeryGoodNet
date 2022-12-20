
namespace OxyPlot.Axes
{
    public static class ColorAxisExtensions
    {
        public static OxyColor GetColor(this IColorAxis axis, double value)
        {
            return axis.GetColor(axis.GetPaletteIndex(value));
        }
    }
}
