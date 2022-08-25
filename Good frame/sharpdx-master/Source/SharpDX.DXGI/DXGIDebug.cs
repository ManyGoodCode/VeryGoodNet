using System;

namespace SharpDX.DXGI
{
    public partial class DXGIDebug
    {
        public static DXGIDebug TryCreate()
        {
            return DebugInterface.TryCreateComPtr<DXGIDebug>(out IntPtr comPtr) ? new DXGIDebug(comPtr) : null;
        }
    }
}