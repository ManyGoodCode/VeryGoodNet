using System;

namespace SharpDX.IO
{
    // Native File access flags.
    [Flags]
    public enum NativeFileAccess : uint
    {
        // Read access.
        Read = 0x80000000,

        // Write access.
        Write = 0x40000000,

        // Read/Write Access,
        ReadWrite = Read | Write,

        // Execute access.
        Execute = 0x20000000,

        // All access
        All = 0x10000000
    }
}