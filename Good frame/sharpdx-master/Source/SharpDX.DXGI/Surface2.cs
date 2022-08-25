using System;

namespace SharpDX.DXGI
{
    public partial class Surface2
    {
        public Surface2(Resource1 resource, int index) : base(IntPtr.Zero)
        {
            resource.CreateSubresourceSurface(index, this);
        }
    }
}