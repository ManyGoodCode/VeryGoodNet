using SharpDX.Win32;

namespace SharpDX.XInput
{
    public sealed class ResultCode
    {
        public static readonly Result NotConnected = ErrorCodeHelper.ToResult(ErrorCode.DeviceNotConnected);
    }
}

