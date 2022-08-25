namespace SharpDX.DXGI
{
    public partial class OutputDuplication
    {
        public DataRectangle MapDesktopSurface()
        {
            MappedRectangle mappedRect;
            MapDesktopSurface(out mappedRect);
            return new DataRectangle(mappedRect.PBits, mappedRect.Pitch);
        }

        public void AcquireNextFrame(int timeoutInMilliseconds, out SharpDX.DXGI.OutputDuplicateFrameInformation frameInfoRef, out SharpDX.DXGI.Resource desktopResourceOut)
        {
            var result = this.TryAcquireNextFrame(timeoutInMilliseconds, out frameInfoRef, out desktopResourceOut);
            result.CheckError();
        }
    }
}