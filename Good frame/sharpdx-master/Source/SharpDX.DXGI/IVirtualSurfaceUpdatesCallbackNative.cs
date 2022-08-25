using System;
using System.Collections.Generic;
using System.Text;

namespace SharpDX.DXGI
{
    [Shadow(typeof(VirtualSurfaceUpdatesCallbackNativeShadow))]
    internal partial interface IVirtualSurfaceUpdatesCallbackNative
    {	
        void UpdatesNeeded();
    }
}