using System;
using SharpDX.Mathematics.Interop;

namespace SharpDX.DXGI
{
    public partial class SwapChain1
    {
        public SwapChain1(Factory2 factory, ComObject device, IntPtr hwnd, ref SwapChainDescription1 description, SwapChainFullScreenDescription? fullScreenDescription = null, Output restrictToOutput = null)
            : base(IntPtr.Zero)
        {
            factory.CreateSwapChainForHwnd(device, hwnd, ref description, fullScreenDescription, restrictToOutput, this);
        }

        public SwapChain1(Factory2 factory, ComObject device, ComObject coreWindow, ref SwapChainDescription1 description, Output restrictToOutput = null)
            : base(IntPtr.Zero)
        {
            factory.CreateSwapChainForCoreWindow(device, coreWindow, ref description, restrictToOutput, this);
        }

        /// <summary>
        /// Creates a swapchain for DirectComposition API or WinRT XAML framework. This is applicable only for WinRT platform.
        /// </summary>
        /// <param name="factory">The DXGI Factory used to create the swapchain.</param>
        /// <param name="device">The associated device instance.</param>
        /// <param name="description">The swap chain description.</param>
        /// <param name="restrictToOutput">The output to which this swap chain should be restricted. Default is null, meaning that there is no restriction.</param>
        public SwapChain1(Factory2 factory, ComObject device, ref SwapChainDescription1 description, Output restrictToOutput = null)
            : base(IntPtr.Zero)
        {
            factory.CreateSwapChainForComposition(device, ref description, restrictToOutput, this);
        }

        public unsafe Result Present(int syncInterval, PresentFlags presentFlags, PresentParameters presentParameters)
        {
            bool hasScrollRectangle = presentParameters.ScrollRectangle.HasValue;
            bool hasScrollOffset = presentParameters.ScrollOffset.HasValue;

            var scrollRectangle = hasScrollRectangle ? presentParameters.ScrollRectangle.Value : new RawRectangle();
            var scrollOffset = hasScrollOffset ? presentParameters.ScrollOffset.Value : default(RawPoint);

            fixed (void* pDirtyRects = presentParameters.DirtyRectangles)
            {
                var native = default(PresentParameters.__Native);
                native.DirtyRectsCount = presentParameters.DirtyRectangles != null ? presentParameters.DirtyRectangles.Length : 0;
                native.PDirtyRects = (IntPtr)pDirtyRects;
                native.PScrollRect = hasScrollRectangle ? new IntPtr(&scrollRectangle) : IntPtr.Zero;
                native.PScrollOffset = hasScrollOffset ? new IntPtr(&scrollOffset) : IntPtr.Zero;

                return Present1(syncInterval, presentFlags, new IntPtr(&native));
            }
        }
    }
}
