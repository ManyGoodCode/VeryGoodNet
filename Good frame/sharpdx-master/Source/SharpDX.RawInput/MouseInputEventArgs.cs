using System;

namespace SharpDX.RawInput
{
    // RawInput Mouse event.
    public class MouseInputEventArgs : RawInputEventArgs
    {
        public MouseInputEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseInputEventArgs"/> class.
        /// </summary>
        /// <param name="rawInput">The raw input.</param>
        /// <param name="hwnd">The handle of the window that received the RawInput mesage.</param>
        internal MouseInputEventArgs(ref RawInput rawInput, IntPtr hwnd)
            : base(ref rawInput, hwnd)
        {
            Mode = (MouseMode) rawInput.Data.Mouse.Flags;
            ButtonFlags = (MouseButtonFlags)rawInput.Data.Mouse.ButtonsData.ButtonFlags;
            WheelDelta = rawInput.Data.Mouse.ButtonsData.ButtonData;
            Buttons = rawInput.Data.Mouse.RawButtons;
            X = rawInput.Data.Mouse.LastX;
            Y = rawInput.Data.Mouse.LastY;
            ExtraInformation = rawInput.Data.Mouse.ExtraInformation;
        }

        /// Gets or sets the mode.
        /// The mode.
        public MouseMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the button flags.
        /// </summary>
        /// <value>
        /// The button flags.
        /// </value>
        public MouseButtonFlags ButtonFlags { get; set; }

        /// <summary>
        /// Gets or sets the extra information.
        /// </summary>
        /// <value>
        /// The extra information.
        /// </value>
        public int ExtraInformation { get; set; }

        /// <summary>
        /// Gets or sets the raw buttons.
        /// </summary>
        /// <value>
        /// The raw buttons.
        /// </value>
        public int Buttons { get; set; }

        /// <summary>
        /// Gets or sets the wheel delta.
        /// </summary>
        /// <value>
        /// The wheel delta.
        /// </value>
        public int WheelDelta { get; set; }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>
        /// The X.
        /// </value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>
        /// The Y.
        /// </value>
        public int Y { get; set; }
    }
}