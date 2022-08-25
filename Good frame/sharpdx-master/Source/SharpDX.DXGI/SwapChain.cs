using System;
using SharpDX.Mathematics.Interop;

namespace SharpDX.DXGI
{
    public partial class SwapChain
    {
        public SwapChain(Factory factory, ComObject device, SwapChainDescription description)
            : base(IntPtr.Zero)
        {
            factory.CreateSwapChain(device, ref description, this);
        }
	
        public T GetBackBuffer<T>(int index) where T : ComObject
        {
            IntPtr temp;
            GetBuffer(index, Utilities.GetGuidFromType(typeof (T)), out temp);
            return FromPointer<T>(temp);
        }
        public SharpDX.DXGI.FrameStatistics FrameStatistics
        {
            get
            {
                SharpDX.DXGI.FrameStatistics output;
                SharpDX.Result result = TryGetFrameStatistics(out output);
                result.CheckError();
                return output;
            }
        }
	
        public bool IsFullScreen
        {
            get
            {
                RawBool isFullScreen;
                Output output;
                GetFullscreenState(out isFullScreen, out output);
                if (output != null)
                    output.Dispose();
                return isFullScreen;
            }

            set
            {
                SetFullscreenState(value, null);
            }
        }

        public SharpDX.Result Present(int syncInterval, SharpDX.DXGI.PresentFlags flags)
        {
            unsafe
            {
                SharpDX.Result result;
                result = TryPresent(syncInterval, flags);
                result.CheckError();
                return result;
            }
        }
    }
}