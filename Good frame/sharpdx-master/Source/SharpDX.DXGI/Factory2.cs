
using System;
namespace SharpDX.DXGI
{
    public partial class Factory2
    {
        public Factory2(bool debug = false)
            : this(IntPtr.Zero)
        {
            IntPtr factoryPtr;
            DXGI.CreateDXGIFactory2(debug ? DXGI.CreateFactoryDebug : 0x00, Utilities.GetGuidFromType(GetType()), out factoryPtr);
            NativePointer = factoryPtr;
        }
    }
}