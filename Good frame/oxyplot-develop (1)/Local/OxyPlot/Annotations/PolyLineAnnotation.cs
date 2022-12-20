

namespace OxyPlot.Annotations
{
    using System.Collections.Generic;
    using System.Linq;

    public class PolylineAnnotation : PathAnnotation
    {
        private readonly List<DataPoint> points = new List<DataPoint>();
        public List<DataPoint> Points
        {
            get
            {
                return this.points;
            }
        }

        public IInterpolationAlgorithm InterpolationAlgorithm { get; set; }
        protected override IList<ScreenPoint> GetScreenPoints()
        {
            var screenPoints = this.Points.Select(this.Transform).ToList();

            if (this.InterpolationAlgorithm != null)
            {
                var resampledPoints = ScreenPointHelper.ResamplePoints(screenPoints, this.MinimumSegmentLength);
                return this.InterpolationAlgorithm.CreateSpline(resampledPoints, false, 0.25);
            }

            return this.Points.Select(this.Transform).ToList();
        }
    }
}
