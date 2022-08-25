namespace SharpDX.XInput
{
    public partial struct Gamepad
    {
        /// <summary>Constant TriggerThreshold.</summary>
        /// <unmanaged>XINPUT_GAMEPAD_TRIGGER_THRESHOLD</unmanaged>
        public const byte TriggerThreshold = unchecked((byte)30u);

        public override string ToString()
        {
            return string.Format("Buttons: {0}, LeftTrigger: {1}, RightTrigger: {2}, LeftThumbX: {3}, LeftThumbY: {4}, RightThumbX: {5}, RightThumbY: {6}", Buttons, LeftTrigger, RightTrigger, LeftThumbX, LeftThumbY, RightThumbX, RightThumbY);
        }
    }
}

