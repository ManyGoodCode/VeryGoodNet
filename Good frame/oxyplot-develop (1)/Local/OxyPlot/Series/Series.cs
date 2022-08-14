namespace OxyPlot.Series
{
    using OxyPlot.Axes;

    public abstract class Series : PlotElement
    {
        protected Series()
        {
            this.IsVisible = true;
            this.Background = OxyColors.Undefined;
            this.RenderInLegend = true;
        }

        public OxyColor Background { get; set; }
        public bool IsVisible { get; set; }
        public string Title { get; set; }
        public string LegendKey { get; set; }
        public string SeriesGroupName { get; set; }
        public bool RenderInLegend { get; set; }
        public string TrackerFormatString { get; set; }
        public string TrackerKey { get; set; }
        public virtual TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            return null;
        }

        public abstract void Render(IRenderContext rc);
        public abstract void RenderLegend(IRenderContext rc, OxyRect legendBox);
        protected internal abstract bool AreAxesRequired();
        protected internal abstract void EnsureAxes();
        protected internal abstract bool IsUsing(Axis axis);
        protected internal abstract void SetDefaultValues();
        protected internal abstract void UpdateAxisMaxMin();
        protected internal abstract void UpdateData();
        protected internal abstract void UpdateMaxMin();
        protected override HitTestResult HitTestOverride(HitTestArguments args)
        {
            var thr = this.GetNearestPoint(args.Point, true) ?? this.GetNearestPoint(args.Point, false);

            if (thr != null)
            {
                double distance = thr.Position.DistanceTo(args.Point);
                if (distance > args.Tolerance)
                {
                    return null;
                }

                return new HitTestResult(this, thr.Position, thr.Item, thr.Index);
            }

            return null;
        }
    }
}
