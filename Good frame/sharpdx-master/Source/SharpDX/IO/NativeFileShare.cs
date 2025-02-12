using System;

namespace SharpDX.IO
{
    /// <summary>
    /// Native file share.
    /// </summary>
    [Flags]
    public enum NativeFileShare : uint
    {
        /// <summary>
        /// None flag.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Enables subsequent open operations on an object to request read access.
        /// Otherwise, other processes cannot open the object if they request read access.
        /// If this flag is not specified, but the object has been opened for read access, the function fails.
        /// </summary>
        Read = 0x00000001,

        /// <summary>
        /// Enables subsequent open operations on an object to request write access.
        /// Otherwise, other processes cannot open the object if they request write access.
        /// If this flag is not specified, but the object has been opened for write access, the function fails.
        /// </summary>
        Write = 0x00000002,

        /// <summary>
        /// Read and Write flags.
        /// </summary>
        ReadWrite = Read  | Write,

        /// <summary>
        /// Enables subsequent open operations on an object to request delete access.
        /// Otherwise, other processes cannot open the object if they request delete access.
        /// If this flag is not specified, but the object has been opened for delete access, the function fails.
        /// </summary>
        Delete = 0x00000004
    }
}