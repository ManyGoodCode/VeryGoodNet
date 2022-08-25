using System;

namespace SharpDX.Win32
{
    [Shadow(typeof(ComStreamBaseShadow))]
    public partial interface IStreamBase
    {
        int Read(IntPtr buffer, int numberOfBytesToRead);
        int Write(IntPtr buffer, int numberOfBytesToRead);
    }
}

