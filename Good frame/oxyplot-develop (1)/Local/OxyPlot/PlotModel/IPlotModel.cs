namespace OxyPlot
{
    public interface IPlotModel
    {
        OxyColor Background { get; }
        void Update(bool updateData);

        void Render(IRenderContext rc, OxyRect rect);

        void AttachPlotView(IPlotView plotView);
    }
}
