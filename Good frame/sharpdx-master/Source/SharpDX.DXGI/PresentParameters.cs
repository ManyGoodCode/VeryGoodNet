using System.Runtime.InteropServices;
using SharpDX.Mathematics.Interop;

namespace SharpDX.DXGI
{
    public partial struct PresentParameters 
    {
        public RawRectangle[] DirtyRectangles;
        public RawRectangle? ScrollRectangle;
        public RawPoint? ScrollOffset;

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        internal partial struct __Native
        {
            public int DirtyRectsCount;
            public System.IntPtr PDirtyRects;
            public System.IntPtr PScrollRect;
            public System.IntPtr PScrollOffset;
        }
    }
}