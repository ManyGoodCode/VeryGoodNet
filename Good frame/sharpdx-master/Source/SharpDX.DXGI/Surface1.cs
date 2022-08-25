using System;
using SharpDX.Mathematics.Interop;

namespace SharpDX.DXGI
{
    public partial class Surface1
    {
        public void ReleaseDC()
        {
            ReleaseDC_(null);
        }

        public void ReleaseDC(RawRectangle dirtyRect)
        {
            ReleaseDC_(dirtyRect);
        }
    }
}