namespace SharpDX.XInput
{
    // Common interface for XInput to allow using XInput 1.4, 1.3 or 9.1.0.
    internal interface IXInput
    {
        int XInputSetState(int dwUserIndex, SharpDX.XInput.Vibration vibrationRef);
        int XInputGetState(int dwUserIndex, out SharpDX.XInput.State stateRef);
        int XInputGetAudioDeviceIds(int dwUserIndex,
            System.IntPtr renderDeviceIdRef,
            System.IntPtr renderCountRef,
            System.IntPtr captureDeviceIdRef,
            System.IntPtr captureCountRef);
        void XInputEnable(SharpDX.Mathematics.Interop.RawBool enable);
        int XInputGetBatteryInformation(int dwUserIndex, SharpDX.XInput.BatteryDeviceType devType, out SharpDX.XInput.BatteryInformation batteryInformationRef);
        int XInputGetKeystroke(int dwUserIndex, int dwReserved, out SharpDX.XInput.Keystroke keystrokeRef);
        int XInputGetCapabilities(int dwUserIndex, SharpDX.XInput.DeviceQueryType dwFlags, out SharpDX.XInput.Capabilities capabilitiesRef);
    }
}