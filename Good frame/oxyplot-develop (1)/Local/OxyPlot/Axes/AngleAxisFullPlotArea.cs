namespace OxyPlot.Axes
{
    public class AngleAxisFullPlotArea : AngleAxis
    {
        public override void Render(IRenderContext rc, int pass)
        {
            var r = new AngleAxisFullPlotAreaRenderer(rc, this.PlotModel);
            r.Render(this, pass);
        }
    }
}
