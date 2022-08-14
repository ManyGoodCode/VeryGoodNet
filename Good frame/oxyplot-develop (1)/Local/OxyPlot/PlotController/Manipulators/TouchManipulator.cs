namespace OxyPlot
{
    public class TouchManipulator : PlotManipulator<OxyTouchEventArgs>
    {
        public TouchManipulator(IPlotView plotView)
            : base(plotView)
        {
            SetHandledForPanOrZoom = true;
        }

        protected bool SetHandledForPanOrZoom { get; set; }

        private bool IsPanEnabled { get; set; }

        private bool IsZoomEnabled { get; set; }

        public override void Completed(OxyTouchEventArgs e)
        {
            base.Completed(e);

            if (this.SetHandledForPanOrZoom)
            {
                e.Handled |= this.IsPanEnabled || this.IsZoomEnabled;
            }
        }

        public override void Delta(OxyTouchEventArgs e)
        {
            base.Delta(e);
            if (!this.IsPanEnabled && !this.IsZoomEnabled)
            {
                return;
            }

            ScreenPoint newPosition = e.Position;
            ScreenPoint previousPosition = newPosition - e.DeltaTranslation;

            if (this.XAxis != null)
            {
                this.XAxis.Pan(previousPosition, newPosition);
            }

            if (this.YAxis != null)
            {
                this.YAxis.Pan(previousPosition, newPosition);
            }

            DataPoint current = this.InverseTransform(newPosition.X, newPosition.Y);

            if (this.XAxis != null)
            {
                this.XAxis.ZoomAt(e.DeltaScale.X, current.X);
            }

            if (this.YAxis != null)
            {
                this.YAxis.ZoomAt(e.DeltaScale.Y, current.Y);
            }

            this.PlotView.InvalidatePlot(false);
            e.Handled = true;
        }

        public override void Started(OxyTouchEventArgs e)
        {
            this.AssignAxes(e.Position);
            base.Started(e);

            if (this.SetHandledForPanOrZoom)
            {
                this.IsPanEnabled = (this.XAxis != null && this.XAxis.IsPanEnabled)
                                    || (this.YAxis != null && this.YAxis.IsPanEnabled);

                this.IsZoomEnabled = (this.XAxis != null && this.XAxis.IsZoomEnabled)
                                     || (this.YAxis != null && this.YAxis.IsZoomEnabled);

                e.Handled |= this.IsPanEnabled || this.IsZoomEnabled;
            }
        }
    }
}