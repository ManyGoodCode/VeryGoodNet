
using System;

namespace SharpDX.DXGI
{
    public partial class Resource1
    {	
        public System.IntPtr CreateSharedHandle(string name, SharpDX.DXGI.SharedResourceFlags dwAccess, SharpDX.Win32.SecurityAttributes? attributesRef = null )
        {
            return CreateSharedHandle(attributesRef, dwAccess, name);
        }
    }
}