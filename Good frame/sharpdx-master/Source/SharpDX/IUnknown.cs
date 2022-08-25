using System;
using System.Runtime.InteropServices;

namespace SharpDX
{
    public partial interface IUnknown
    {
        Result QueryInterface(ref Guid guid, out IntPtr comObject);
        int AddReference();
        int Release();
    }
}