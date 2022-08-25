using System;

namespace SharpDX.DXGI
{
    public partial class Factory4
    {
        public Factory4() : this(IntPtr.Zero)
        {
            IntPtr factoryPtr;
            DXGI.CreateDXGIFactory1(Utilities.GetGuidFromType(GetType()), out factoryPtr);
            NativePointer = factoryPtr;
        }
        public Factory4(bool debug = false) : this(IntPtr.Zero)
        {
            IntPtr factoryPtr;
            DXGI.CreateDXGIFactory2(debug ? DXGI.CreateFactoryDebug : 0x00, Utilities.GetGuidFromType(typeof(Factory4)), out factoryPtr);
            NativePointer = factoryPtr;
        }

        public Adapter GetWarpAdapter()
        {
            IntPtr adapterPtr;
            EnumWarpAdapter(Utilities.GetGuidFromType(typeof(Adapter)), out adapterPtr);
            return new Adapter(adapterPtr);
        }

        public Adapter GetAdapterByLuid(long adapterLuid)
        {
            IntPtr adapterPtr;
            EnumAdapterByLuid(adapterLuid, Utilities.GetGuidFromType(typeof(Adapter)), out adapterPtr);
            return new Adapter(adapterPtr);
        }
    }
}