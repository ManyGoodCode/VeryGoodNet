namespace OxyPlot
{
    public class OxyTouchEventArgs : OxyInputEventArgs
    {
        public OxyTouchEventArgs()
        {
        }

        public OxyTouchEventArgs(ScreenPoint[] currentTouches, ScreenPoint[] previousTouches)
        {
            this.Position = currentTouches[0];

            if (currentTouches.Length == previousTouches.Length)
            {
                this.DeltaTranslation = currentTouches[0] - previousTouches[0];
            }

            double scale = 1;
            if (currentTouches.Length > 1 && currentTouches.Length == previousTouches.Length)
            {
                double currentDistance = (currentTouches[1] - currentTouches[0]).Length;
                double previousDistance = (previousTouches[1] - previousTouches[0]).Length;
                scale = currentDistance / previousDistance;

                if (scale < 0.5)
                {
                    scale = 0.5;
                }

                if (scale > 2)
                {
                    scale = 2;
                }
            }

            this.DeltaScale = new ScreenVector(scale, scale);
        }

        public ScreenPoint Position { get; set; }
        public ScreenVector DeltaScale { get; set; }

        public ScreenVector DeltaTranslation { get; set; }
    }
}