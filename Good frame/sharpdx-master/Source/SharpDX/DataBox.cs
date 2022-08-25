using System;
using System.Runtime.InteropServices;

namespace SharpDX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DataBox
    {
        public DataBox(IntPtr datapointer, int rowPitch, int slicePitch)
        {
            DataPointer = datapointer;
            RowPitch = rowPitch;
            SlicePitch = slicePitch;
        }

        public DataBox(IntPtr dataPointer)
        {
            DataPointer = dataPointer;
            RowPitch = 0;
            SlicePitch = 0;
        }

        public IntPtr DataPointer;
        public int RowPitch;
        public int SlicePitch;
        public bool IsEmpty
        {
            get
            {
                return DataPointer == IntPtr.Zero && RowPitch == 0 && SlicePitch == 0;
            }
        }
    }
}