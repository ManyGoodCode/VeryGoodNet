namespace OxyPlot
{
    public class ZoomStepManipulator : MouseManipulator
    {
        public ZoomStepManipulator(IPlotView plotView)
            : base(plotView)
        {
        }

        public bool FineControl { get; set; }

        public double Step { get; set; }

        public override void Started(OxyMouseEventArgs e)
        {
            base.Started(e);

            bool isZoomEnabled = (this.XAxis != null && this.XAxis.IsZoomEnabled)
                                || (this.YAxis != null && this.YAxis.IsZoomEnabled);

            if (!isZoomEnabled)
            {
                return;
            }

            DataPoint current = this.InverseTransform(e.Position.X, e.Position.Y);

            double scale = this.Step;
            if (this.FineControl)
            {
                scale *= 3;
            }

            if (scale > 0)
            {
                scale = 1 + scale;
            }
            else
            {
                scale = 1.0 / (1 - scale);
            }

            if (this.XAxis != null)
            {
                this.XAxis.ZoomAt(scale, current.X);
            }

            if (this.YAxis != null)
            {
                this.YAxis.ZoomAt(scale, current.Y);
            }

            this.PlotView.InvalidatePlot(false);
            e.Handled = true;
        }
    }
}
