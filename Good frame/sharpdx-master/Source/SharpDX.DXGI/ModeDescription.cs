namespace SharpDX.DXGI
{
    public partial struct ModeDescription
    {
        public ModeDescription(int width, int height, Rational refreshRate, Format format)
        {
            this.Width = width;
            this.Height = height;
            this.RefreshRate = refreshRate;
            this.Format = format;
            this.ScanlineOrdering = DisplayModeScanlineOrder.Unspecified;
            this.Scaling = DisplayModeScaling.Unspecified;
        }

        public ModeDescription(Format format) : this()
        {
            Format = format;
        }
    }
}