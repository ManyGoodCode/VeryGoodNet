namespace OxyPlot
{
    public class HitTestArguments
    {
        public HitTestArguments(ScreenPoint point, double tolerance)
        {
            this.Point = point;
            this.Tolerance = tolerance;
        }

        public ScreenPoint Point { get; private set; }

        public double Tolerance { get; private set; }
    }
}