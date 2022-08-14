namespace OxyPlot
{
    public class TouchTrackerManipulator : TouchManipulator
    {
        private Series.Series currentSeries;

        public TouchTrackerManipulator(IPlotView plotView)
            : base(plotView)
        {
            this.Snap = true;
            this.PointsOnly = false;
            this.LockToInitialSeries = true;
            this.FiresDistance = 20.0;
            this.CheckDistanceBetweenPoints = false;

            this.SetHandledForPanOrZoom = false;
        }

        public bool PointsOnly { get; set; }
        public bool Snap { get; set; }
        public bool LockToInitialSeries { get; set; }

        public double FiresDistance { get; set; }
        public bool CheckDistanceBetweenPoints { get; set; }
        public override void Completed(OxyTouchEventArgs e)
        {
            base.Completed(e);

            this.currentSeries = null;
            this.PlotView.HideTracker();
            if (this.PlotView.ActualModel != null)
            {
                this.PlotView.ActualModel.RaiseTrackerChanged(null);
            }
        }

        public override void Delta(OxyTouchEventArgs e)
        {
            base.Delta(e);
            this.PlotView.HideTracker();
        }

        public override void Started(OxyTouchEventArgs e)
        {
            base.Started(e);
            this.currentSeries = this.PlotView.ActualModel?.GetSeriesFromPoint(e.Position, this.FiresDistance);
            UpdateTracker(e.Position);
        }

        private void UpdateTracker(ScreenPoint position)
        {
            if (this.currentSeries == null || !this.LockToInitialSeries)
            {
                this.currentSeries = this.PlotView.ActualModel?.GetSeriesFromPoint(position, this.FiresDistance);
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

            if (!actualModel.PlotArea.Contains(position.X, position.Y))
            {
                return;
            }

            TrackerHitResult result = Utilities.TrackerHelper.GetNearestHit(
                this.currentSeries, position, this.Snap, this.PointsOnly, this.FiresDistance, this.CheckDistanceBetweenPoints);
            if (result != null)
            {
                result.PlotModel = this.PlotView.ActualModel;
                this.PlotView.ShowTracker(result);
                this.PlotView.ActualModel.RaiseTrackerChanged(result);
            }
        }
    }
}
