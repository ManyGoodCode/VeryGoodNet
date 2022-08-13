namespace OxyPlot
{
    public class OxyMouseEnterGesture : OxyInputGesture
    {
        public OxyMouseEnterGesture(OxyModifierKeys modifiers = OxyModifierKeys.None)
        {
            this.Modifiers = modifiers;
        }

        public OxyModifierKeys Modifiers { get; private set; }

        public override bool Equals(OxyInputGesture other)
        {
            OxyMouseEnterGesture mg = other as OxyMouseEnterGesture;
            return mg != null && mg.Modifiers == this.Modifiers;
        }
    }
}