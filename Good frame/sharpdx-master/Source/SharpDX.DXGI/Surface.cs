using System;

namespace SharpDX.DXGI
{
    public partial class Surface
    {
        public DataRectangle Map(MapFlags flags)
        {
            MappedRectangle mappedRect;
            Map(out mappedRect, (int) flags);
            return new DataRectangle(mappedRect.PBits, mappedRect.Pitch);
        }

        public DataRectangle Map(MapFlags flags, out DataStream dataStream)
        {
            var dataRectangle = Map(flags);
            dataStream = new DataStream(dataRectangle.DataPointer, Description.Height * dataRectangle.Pitch, true, true);
            return dataRectangle;
        }

        public static Surface FromSwapChain(SwapChain swapChain, int index)
        {
            IntPtr surfacePointer;
            swapChain.GetBuffer(index, Utilities.GetGuidFromType(typeof (Surface)), out surfacePointer);
            return new Surface(surfacePointer);
        }
    }
}