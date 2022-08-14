namespace OxyPlot.Utilities
{
    internal static class TrackerHelper
    {
        public static TrackerHitResult GetNearestHit(
            Series.Series series,
            ScreenPoint point,
            bool snap,
            bool pointsOnly,
            double firesDistance,
            bool checkDistanceBetweenPoints)
        {
            if (series == null)
            {
                return null;
            }

            if (snap || pointsOnly)
            {
                var result = series.GetNearestPoint(point, false);
                if (ShouldTrackerOpen(result, point, firesDistance))
                {
                    return result;
                }
            }

            if (!pointsOnly)
            {
                var result = series.GetNearestPoint(point, true);
                if (!checkDistanceBetweenPoints || ShouldTrackerOpen(result, point, firesDistance))
                {
                    return result;
                }
            }

            return null;
        }

        private static bool ShouldTrackerOpen(TrackerHitResult result, ScreenPoint point, double firesDistance) =>
            result?.Position.DistanceTo(point) < firesDistance;
    }
}
