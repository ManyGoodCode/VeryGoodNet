using System;
using System.Runtime.InteropServices;
using SharpDX.Mathematics.Interop;

namespace SharpDX.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMessage
    {
        public IntPtr handle;
        public uint msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public RawPoint p;
    }
}