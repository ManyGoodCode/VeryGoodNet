namespace OxyPlot
{
    public class OxyShakeGesture : OxyInputGesture
    {
        public override bool Equals(OxyInputGesture other)
        {
            OxyShakeGesture sg = other as OxyShakeGesture;
            return sg != null;
        }
    }
}