namespace OxyPlot
{
    public abstract class MouseManipulator : PlotManipulator<OxyMouseEventArgs>
    {
        protected MouseManipulator(IPlotView plotView)
            : base(plotView)
        {
        }

        public ScreenPoint StartPosition { get; protected set; }

        public override void Started(OxyMouseEventArgs e)
        {
            this.AssignAxes(e.Position);
            base.Started(e);
            this.StartPosition = e.Position;
        }
    }
}