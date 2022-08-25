using System.Windows.Forms;

namespace SharpDX.RawInput
{
    /// <summary>
    /// Options used when using <see cref="Device.RegisterDevice(SharpDX.Multimedia.UsagePage,SharpDX.Multimedia.UsageId,SharpDX.RawInput.DeviceFlags)"/>
    /// </summary>
    public enum RegisterDeviceOptions
    {
        // Default register using <see cref="Application.AddMessageFilter"/> for RawInput message filtering.
        Default = 0,

        // To disable message filtering
        NoFiltering = 1,

        // Use custom message filtering instead of <see cref="Application.AddMessageFilter"/>
        CustomFiltering = 2,
    }
}