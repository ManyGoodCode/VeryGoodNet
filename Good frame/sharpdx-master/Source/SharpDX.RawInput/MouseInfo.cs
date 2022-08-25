using System;

namespace SharpDX.RawInput
{
    /// <summary>
    /// Defines the raw input data coming from the specified mouse.
    /// </summary>
    /// <unmanaged>RID_DEVICE_INFO_MOUSE</unmanaged>	
    public class MouseInfo : DeviceInfo
    {
        public MouseInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseInfo"/> class.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        internal MouseInfo(ref RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle) : base(ref rawDeviceInfo, deviceName, deviceHandle)
        {
            Id = rawDeviceInfo.Mouse.Id;
            ButtonCount = rawDeviceInfo.Mouse.NumberOfButtons;
            SampleRate = rawDeviceInfo.Mouse.SampleRate;
            HasHorizontalWheel = rawDeviceInfo.Mouse.HasHorizontalWheel;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        /// <unmanaged>unsigned int dwId</unmanaged>	
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the button count.
        /// </summary>
        /// <value>
        /// The button count.
        /// </value>
        /// <unmanaged>unsigned int dwNumberOfButtons</unmanaged>	
        public int ButtonCount { get; set; }

        /// <summary>
        /// Gets or sets the sample rate.
        /// </summary>
        /// <value>
        /// The sample rate.
        /// </value>
        /// <unmanaged>unsigned int dwSampleRate</unmanaged>	
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has horizontal wheel.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has horizontal wheel; otherwise, <c>false</c>.
        /// </value>
        /// <unmanaged>BOOL fHasHorizontalWheel</unmanaged>	
        public bool HasHorizontalWheel { get; set; }
    }
}