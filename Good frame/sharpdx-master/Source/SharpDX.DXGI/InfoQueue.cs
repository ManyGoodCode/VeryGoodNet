using System;

namespace SharpDX.DXGI
{
    public partial class InfoQueue
    {
        public static InfoQueue TryCreate()
        {
            return DebugInterface.TryCreateComPtr<InfoQueue>(out IntPtr comPtr) ? new InfoQueue(comPtr) : null;
        }
    }
}