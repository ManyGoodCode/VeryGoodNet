using System;
using System.Runtime.InteropServices;

namespace SharpDX.Win32
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct SecurityAttributes
    {
        public int Length;
        public IntPtr Descriptor;

        private int inheritHandle;
        public bool InheritHandle
        {
            get { return inheritHandle != 0; }
            set { inheritHandle = value ? 1 : 0; }
        }
    }
}
