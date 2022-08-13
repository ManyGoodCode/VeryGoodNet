namespace OxyPlot
{
    public class OxyKeyGesture : OxyInputGesture
    {
        public OxyKeyGesture(OxyKey key, OxyModifierKeys modifiers = OxyModifierKeys.None)
        {
            this.Key = key;
            this.Modifiers = modifiers;
        }

        public OxyModifierKeys Modifiers { get; set; }
        public OxyKey Key { get; set; }
        public override bool Equals(OxyInputGesture other)
        {
            OxyKeyGesture kg = other as OxyKeyGesture;
            return kg != null && kg.Modifiers == this.Modifiers && kg.Key == this.Key;
        }
    }
}