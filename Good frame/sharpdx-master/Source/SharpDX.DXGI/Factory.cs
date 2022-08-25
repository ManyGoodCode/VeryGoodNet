using System;
using System.Collections.Generic;

namespace SharpDX.DXGI
{
    public partial class Factory
    {
        public Adapter GetAdapter(int index)
        {
            Adapter adapter;
            GetAdapter(index, out adapter).CheckError();
            return adapter;
        }
        public Adapter[] Adapters
        {
            get
            {
                List<Adapter> adapters = new List<Adapter>();
                do
                {
                    Adapter adapter;
                    var result = GetAdapter(adapters.Count, out adapter);
                    if (result == ResultCode.NotFound)
                        break;
                    adapters.Add(adapter);
                } while (true);
                return adapters.ToArray();
            }
        }

        /// <summary>
        ///   Return the number of available adapters from this factory.
        /// </summary>
        /// <returns>The number of adapters</returns>
        /// <unmanaged>HRESULT IDXGIFactory::EnumAdapters([In] unsigned int Adapter,[Out] IDXGIAdapter** ppAdapter)</unmanaged>	
        public int GetAdapterCount()
        {
            int nbAdapters = 0;
            do
            {
                Adapter adapter;
                var result = GetAdapter(nbAdapters, out adapter);
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