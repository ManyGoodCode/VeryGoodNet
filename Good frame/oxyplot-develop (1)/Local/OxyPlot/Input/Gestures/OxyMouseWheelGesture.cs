namespace OxyPlot
{
    public class OxyMouseWheelGesture : OxyInputGesture
    {
        public OxyMouseWheelGesture(OxyModifierKeys modifiers = OxyModifierKeys.None)
        {
            this.Modifiers = modifiers;
        }

        public OxyModifierKeys Modifiers { get; private set; }
        public override bool Equals(OxyInputGesture other)
        {
            OxyMouseWheelGesture mwg = other as OxyMouseWheelGesture;
            return mwg != null && mwg.Modifiers == this.Modifiers;
        }
    }
}