using System;
using SharpDX.Mathematics.Interop;

namespace SharpDX.DXGI
{
    public partial class Factory5
    {
        public unsafe bool PresentAllowTearing
        {
            get 
            {
                RawBool allowTearing;
                CheckFeatureSupport(Feature.PresentAllowTearing, new IntPtr(&allowTearing), sizeof(RawBool));
                return allowTearing;
            }
        }
    }
}