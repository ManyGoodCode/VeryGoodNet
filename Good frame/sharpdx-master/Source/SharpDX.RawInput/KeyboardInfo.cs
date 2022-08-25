using System;

namespace SharpDX.RawInput
{
    /// <summary>
    /// Defines the raw input data coming from the specified keyboard. 
    /// </summary>
    /// <unmanaged>RID_DEVICE_INFO_KEYBOARD</unmanaged>	
    public class KeyboardInfo : DeviceInfo
    {
        public KeyboardInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardInfo"/> class.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        internal KeyboardInfo(ref RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle) : base(ref rawDeviceInfo, deviceName, deviceHandle)
        {
            KeyboardType = rawDeviceInfo.Keyboard.Type;
            Subtype = rawDeviceInfo.Keyboard.SubType;
            KeyboardMode = rawDeviceInfo.Keyboard.KeyboardMode;
            FunctionKeyCount = rawDeviceInfo.Keyboard.NumberOfFunctionKeys;
            IndicatorCount = rawDeviceInfo.Keyboard.NumberOfIndicators;
            TotalKeyCount = rawDeviceInfo.Keyboard.NumberOfKeysTotal;
        }

        /// <summary>
        /// Gets or sets the type of the keyboard.
        /// </summary>
        /// <value>
        /// The type of the keyboard.
        /// </value>
        /// <unmanaged>unsigned int dwType</unmanaged>	
        public int KeyboardType { get; set; }

        /// <summary>
        /// Gets or sets the subtype.
        /// </summary>
        /// <value>
        /// The subtype.
        /// </value>
        /// <unmanaged>unsigned int dwSubType</unmanaged>	
        public int Subtype { get; set; }

        /// <summary>
        /// Gets or sets the keyboard mode.
        /// </summary>
        /// <value>
        /// The keyboard mode.
        /// </value>
        /// <unmanaged>unsigned int dwKeyboardMode</unmanaged>	
        public int KeyboardMode { get; set; }

        /// <summary>
        /// Gets or sets the function key count.
        /// </summary>
        /// <value>
        /// The function key count.
        /// </value>
        /// <unmanaged>unsigned int dwNumberOfFunctionKeys</unmanaged>	
        public int FunctionKeyCount { get; set; }

        /// <summary>
        /// Gets or sets the indicator count.
        /// </summary>
        /// <value>
        /// The indicator count.
        /// </value>
        /// <unmanaged>unsigned int dwNumberOfIndicators</unmanaged>	
        public int IndicatorCount { get; set; }


        /// <summary>
        /// Gets or sets the total key count.
        /// </summary>
        /// <value>
        /// The total key count.
        /// </value>
        /// <unmanaged>unsigned int dwNumberOfKeysTotal</unmanaged>	
        public int TotalKeyCount { get; set; }
    }
}