namespace OxyPlot
{
    public class OxyMouseDownGesture : OxyInputGesture
    {
        public OxyMouseDownGesture(OxyMouseButton mouseButton, OxyModifierKeys modifiers = OxyModifierKeys.None, int clickCount = 1)
        {
            this.MouseButton = mouseButton;
            this.Modifiers = modifiers;
            this.ClickCount = clickCount;
        }

        public OxyModifierKeys Modifiers { get; private set; }
        public OxyMouseButton MouseButton { get; private set; }
        public int ClickCount { get; private set; }

        public override bool Equals(OxyInputGesture other)
        {
            OxyMouseDownGesture mg = other as OxyMouseDownGesture;
            return mg != null && mg.Modifiers == this.Modifiers && mg.MouseButton == this.MouseButton && mg.ClickCount == this.ClickCount;
        }
    }
}