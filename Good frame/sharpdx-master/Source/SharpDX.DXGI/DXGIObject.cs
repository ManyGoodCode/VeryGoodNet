using System;

namespace SharpDX.DXGI
{
    public partial class DXGIObject
    {
        public T GetParent<T>() where T : ComObject
        {
            IntPtr temp;
            this.GetParent(Utilities.GetGuidFromType(typeof (T)), out temp);
            return FromPointer<T>(temp);
        }
    }
}