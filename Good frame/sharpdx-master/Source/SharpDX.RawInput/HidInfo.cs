using System;

namespace SharpDX.RawInput
{
    /// <summary>
    /// Defines the raw input data coming from the specified Human Interface Device (HID). 
    /// </summary>
    public class HidInfo : DeviceInfo
    {
        public HidInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HidInfo"/> class.
        /// </summary>
        /// <param name="rawDeviceInfo">The raw device info.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceHandle">The device handle.</param>
        internal HidInfo(ref RawDeviceInformation rawDeviceInfo, string deviceName, IntPtr deviceHandle) : base(ref rawDeviceInfo, deviceName, deviceHandle)
        {
            VendorId = rawDeviceInfo.Hid.VendorId;
            ProductId = rawDeviceInfo.Hid.ProductId;
            VersionNumber = rawDeviceInfo.Hid.VersionNumber;
            UsagePage = rawDeviceInfo.Hid.UsagePage;
            Usage = rawDeviceInfo.Hid.Usage;
        }

        /// <summary>
        /// Gets or sets the vendor id.
        /// </summary>
        /// <value>
        /// The vendor id.
        /// </value>
        /// <unmanaged>unsigned int dwVendorId</unmanaged>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        /// <unmanaged>unsigned int dwProductId</unmanaged>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        /// <value>
        /// The version number.
        /// </value>
        /// <unmanaged>unsigned int dwVersionNumber</unmanaged>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the usage page.
        /// </summary>
        /// <value>
        /// The usage page.
        /// </value>
        /// <unmanaged>HID_USAGE_PAGE usUsagePage</unmanaged>
        public SharpDX.Multimedia.UsagePage UsagePage { get; set; }

        /// <summary>
        /// Gets or sets the usage.
        /// </summary>
        /// <value>
        /// The usage.
        /// </value>
        /// <unmanaged>HID_USAGE_ID usUsage</unmanaged>
        public SharpDX.Multimedia.UsageId Usage { get; set; }
    }
}