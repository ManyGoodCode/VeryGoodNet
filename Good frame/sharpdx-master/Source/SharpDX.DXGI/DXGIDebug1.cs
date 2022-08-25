using System;

namespace SharpDX.DXGI
{
    public partial class DXGIDebug1
    {
        public new static DXGIDebug1 TryCreate()
        {
            return DebugInterface.TryCreateComPtr<DXGIDebug1>(out IntPtr comPtr) ? new DXGIDebug1(comPtr) : null;
        }
    }
}