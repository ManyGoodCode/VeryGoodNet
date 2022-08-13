namespace OxyPlot
{
    public class OxyTouchGesture : OxyInputGesture
    {
        public override bool Equals(OxyInputGesture other)
        {
            OxyTouchGesture tg = other as OxyTouchGesture;
            return tg != null;
        }
    }
}