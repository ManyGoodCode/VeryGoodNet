using System;

namespace SharpDX.RawInput
{
    /// <summary>
    /// Describes the format of the raw input from a Human Interface Device (HID). 
    /// </summary>
    public class HidInputEventArgs : RawInputEventArgs
    {
        public HidInputEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HidInputEventArgs"/> class.
        /// </summary>
        /// <param name="rawInput">The raw input.</param>
        /// <param name="hwnd">The handle of the window that received the RawInput mesage.</param>
        internal HidInputEventArgs(ref RawInput rawInput, IntPtr hwnd) : base(ref rawInput, hwnd)
        {
            Count = rawInput.Data.Hid.Count;
            DataSize = rawInput.Data.Hid.SizeHid;
            RawData = new byte[Count * DataSize];
            unsafe
            {
                if (RawData.Length > 0) fixed (void* __to = RawData) fixed (void* __from = &rawInput.Data.Hid.RawData) SharpDX.Utilities.CopyMemory((IntPtr)__to, (IntPtr)__from, RawData.Length *sizeof(byte));
            }
        }

        /// <summary>
        /// Gets or sets the number of Hid structure in the <see cref="RawData"/>.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the size of the Hid structure in the <see cref="RawData"/>.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public int DataSize { get; set; }

        /// <summary>
        /// Gets or sets the raw data.
        /// </summary>
        /// <value>
        /// The raw data.
        /// </value>
        public byte[] RawData { get; set; }
    }
}