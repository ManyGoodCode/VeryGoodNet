using System;

namespace SharpDX.RawInput
{
    // RawInput event arguments base.
    public abstract class RawInputEventArgs : EventArgs
    {
        protected RawInputEventArgs()
        {
        }

        internal RawInputEventArgs(ref RawInput rawInput, IntPtr hwnd)
        {
            Device = rawInput.Header.Device;
	        WindowHandle = hwnd;
        }

        // Gets or sets the RawInput device.
        // The device.
        public IntPtr Device { get; set; }

        // Gets or sets the handle of the window that received the RawInput mesage.
        // The window handle.
        public IntPtr WindowHandle { get; set; }
    }
}