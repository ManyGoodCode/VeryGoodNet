namespace OxyPlot
{
    using System;

    [Flags]
    public enum OxyModifierKeys
    {
        // No modifiers are pressed.
        None = 0,

        // The Control key.
        Control = 1,

        // The Alt/Menu key.
        Alt = 2,

        // The Shift key.
        Shift = 4,

        // The Windows key.
        Windows = 8
    }
}