using System.Runtime.InteropServices;

namespace SharpDX.RawInput
{
    // Contains information about the state of the mouse.
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawMouse
    {
        // The mouse state.
        public short Flags;

        [StructLayout(LayoutKind.Explicit)]
        public struct RawMouseButtonsData
        {
            [FieldOffset(0)]
            public int Buttons;

            // Flags for the event.
            [FieldOffset(0)]
            public short ButtonFlags;

            // If the mouse wheel is moved, this will contain the delta amount.
            [FieldOffset(2)]
            public short ButtonData;
        }

        public RawMouseButtonsData ButtonsData;

        // Raw button data.
        public int RawButtons;

        // The motion in the X direction. This is signed relative motion or
        // absolute motion, depending on the value of usFlags.
        public int LastX;

        // The motion in the Y direction. This is signed relative motion or absolute motion,
        // depending on the value of usFlags.
        public int LastY;

        // The device-specific additional information for the event.
        public int ExtraInformation;
    }
}

