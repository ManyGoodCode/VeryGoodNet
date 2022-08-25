using System;
using System.Windows.Forms;

namespace SharpDX.RawInput
{
    /// <summary>
    /// RawInput Keyboard event.
    /// </summary>
    public class KeyboardInputEventArgs : RawInputEventArgs
    {
        public KeyboardInputEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardInputEventArgs"/> class.
        /// </summary>
        /// <param name="rawInput">The raw input.</param>
        /// <param name="hwnd">The handle of the window that received the RawInput mesage.</param>
        internal KeyboardInputEventArgs(ref RawInput rawInput, IntPtr hwnd)
            : base(ref rawInput, hwnd)
        {
            Key = (Keys) rawInput.Data.Keyboard.VKey;
            MakeCode = rawInput.Data.Keyboard.MakeCode;
            ScanCodeFlags = rawInput.Data.Keyboard.Flags;
            State = rawInput.Data.Keyboard.Message;
            ExtraInformation = rawInput.Data.Keyboard.ExtraInformation;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Keys Key { get; set; }

        /// <summary>
        /// Gets or sets the make code.
        /// </summary>
        /// <value>
        /// The make code.
        /// </value>
        public int MakeCode { get; set; }

        /// <summary>
        /// Gets or sets the scan code flags.
        /// </summary>
        /// <value>
        /// The scan code flags.
        /// </value>
        public ScanCodeFlags ScanCodeFlags { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public KeyState State { get; set; }

        /// <summary>
        /// Gets or sets the extra information.
        /// </summary>
        /// <value>
        /// The extra information.
        /// </value>
        public int ExtraInformation { get; set; }
    }
}