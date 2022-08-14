namespace OxyPlot
{
    using System;

    public abstract class OxyInputEventArgs : System.EventArgs
    {
        public bool Handled { get; set; }

        public OxyModifierKeys ModifierKeys { get; set; }

        public bool IsAltDown
        {
            get
            {
                return (this.ModifierKeys & OxyModifierKeys.Alt) == OxyModifierKeys.Alt;
            }
        }

        public bool IsControlDown
        {
            get
            {
                return (this.ModifierKeys & OxyModifierKeys.Control) == OxyModifierKeys.Control;
            }
        }

        public bool IsShiftDown
        {
            get
            {
                return (this.ModifierKeys & OxyModifierKeys.Shift) == OxyModifierKeys.Shift;
            }
        }
    }
}