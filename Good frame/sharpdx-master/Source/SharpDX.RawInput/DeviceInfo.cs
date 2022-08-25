using System;

namespace SharpDX.RawInput
{
    /// <summary>
    /// Defines the raw input data coming from any device.
    /// </summary>
    /// <unmanaged>RID_DEVICE_INFO</unmanaged>	
    public partial class DeviceInfo
    {
        public DeviceInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfo"/> class.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        internal DeviceInfo(ref RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle)
        {
            DeviceName = deviceName;
            Handle = deviceHandle;
            DeviceType = rawDeviceInfo.Type;
        }

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        /// <value>
        /// The name of the device.
        /// </value>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        /// <value>
        /// The type of the device.
        /// </value>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// Gets or sets the handle.
        /// </summary>
        /// <value>
        /// The handle.
        /// </value>
        public System.IntPtr Handle { get; set; }

        /// <summary>
        /// Converts the specified raw device info to the <see cref="DeviceInfo"/>.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        /// <returns></returns>
        internal static DeviceInfo Convert(ref RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle)
        {
            DeviceInfo deviceInfo = null;
            switch (rawDeviceInfo.Type)
            {
                case DeviceType.HumanInputDevice:
                    deviceInfo = new HidInfo(ref rawDeviceInfo, deviceName, deviceHandle);
                    break;
                case DeviceType.Keyboard:
                    deviceInfo = new KeyboardInfo(ref rawDeviceInfo, deviceName, deviceHandle);
                    break;
                case DeviceType.Mouse:
                    deviceInfo = new MouseInfo(ref rawDeviceInfo, deviceName, deviceHandle);
                    break;
                default:
                    throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unsupported Device Type [{0}]", (int)rawDeviceInfo.Type));
            }
            return deviceInfo;
        }
    }
}

