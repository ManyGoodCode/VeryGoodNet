using System;
using System.Collections.Generic;

namespace SharpDX.DXGI
{
    public partial class Factory1
    {
        public Factory1() : base(IntPtr.Zero)
        {
            IntPtr factoryPtr;
            DXGI.CreateDXGIFactory1(Utilities.GetGuidFromType(GetType()), out factoryPtr);
            NativePointer = factoryPtr;
        }
	
        public Adapter1 GetAdapter1(int index)
        {
            Adapter1 adapter;
            GetAdapter1(index, out adapter).CheckError();
            return adapter;
        }
	
        public Adapter1[] Adapters1
        {
            get
            {
                var adapters = new List<Adapter1>();
                do
                {
                    Adapter1 adapter;
                    var result = GetAdapter1(adapters.Count, out adapter);
                    if (result == ResultCode.NotFound)
                        break;
                    adapters.Add(adapter);
                } while (true);
                return adapters.ToArray();
            }
        }

        public int GetAdapterCount1()
        {
            int nbAdapters = 0;
            do
            {
                Adapter1 adapter;
                var result = GetAdapter1(nbAdapters, out adapter);
                if (adapter != null)
                    adapter.Dispose();
                if (result == ResultCode.NotFound)
                    break;
                nbAdapters++;
            } while (true);
            return nbAdapters;
        }
    }
}