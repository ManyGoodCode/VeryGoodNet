namespace OxyPlot
{
    public class TrackerManipulator : MouseManipulator
    {
        private Series.Series currentSeries;
        public TrackerManipulator(IPlotView plotView)
            : base(plotView)
        {
            this.Snap = true;
            this.PointsOnly = false;
            this.LockToInitialSeries = true;
            this.FiresDistance = 20.0;
            this.CheckDistanceBetweenPoints = false;
        }

        public bool PointsOnly { get; set; }

        public bool Snap { get; set; }

        public bool LockToInitialSeries { get; set; }

        public double FiresDistance { get; set; }

        public bool CheckDistanceBetweenPoints { get; set; }

        public override void Completed(OxyMouseEventArgs e)
        {
            base.Completed(e);
            e.Handled = true;

            this.currentSeries = null;
            this.PlotView.HideTracker();
            if (this.PlotView.ActualModel != null)
            {
                this.PlotView.ActualModel.RaiseTrackerChanged(null);
            }
        }

        public override void Delta(OxyMouseEventArgs e)
        {
            base.Delta(e);
            e.Handled = true;

            if (this.currentSeries == null || !this.LockToInitialSeries)
            {
                this.currentSeries = this.PlotView.ActualModel?.GetSeriesFromPoint(e.Position, this.FiresDistance);
            }

            if (this.currentSeries == null)
            {
                if (!this.LockToInitialSeries)
                {
                    this.PlotView.HideTracker();
                }

                return;
            }

            PlotModel actualModel = this.PlotView.ActualModel;
            if (actualModel == null)
            {
                return;
            }

            if (!actualModel.PlotArea.Contains(e.Position.X, e.Position.Y))
            {
                return;
            }

            TrackerHitResult result = Utilities.TrackerHelper.GetNearestHit(
                this.currentSeries, e.Position, this.Snap, this.PointsOnly, this.FiresDistance, this.CheckDistanceBetweenPoints);
            if (result != null)
            {
                result.PlotModel = this.PlotView.ActualModel;
                this.PlotView.ShowTracker(result);
                this.PlotView.ActualModel.RaiseTrackerChanged(result);
            }
        }


        public override void Started(OxyMouseEventArgs e)
        {
            base.Started(e);
            this.currentSeries = this.PlotView.ActualModel?.GetSeriesFromPoint(e.Position, FiresDistance);
            this.Delta(e);
        }
    }
}
