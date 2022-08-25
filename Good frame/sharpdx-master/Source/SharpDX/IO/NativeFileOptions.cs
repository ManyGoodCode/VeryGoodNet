using System;

namespace SharpDX.IO
{
    /// <summary>
    /// Native file attributes.
    /// </summary>
    [Flags]
    public enum NativeFileOptions : uint
    {
        /// <summary>
        /// None attribute.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Read only attribute.
        /// </summary>
        Readonly = 0x00000001,

        /// <summary>
        /// Hidden attribute.
        /// </summary>
        Hidden = 0x00000002,

        /// <summary>
        /// System attribute.
        /// </summary>
        System = 0x00000004,

        /// <summary>
        /// Directory attribute.
        /// </summary>
        Directory = 0x00000010,

        /// <summary>
        /// Archive attribute.
        /// </summary>
        Archive = 0x00000020,

        /// <summary>
        /// Device attribute.
        /// </summary>
        Device = 0x00000040,

        /// <summary>
        /// Normal attribute.
        /// </summary>
        Normal = 0x00000080,

        /// <summary>
        /// Temporary attribute.
        /// </summary>
        Temporary = 0x00000100,

        /// <summary>
        /// Sparse file attribute.
        /// </summary>
        SparseFile = 0x00000200,

        /// <summary>
        /// ReparsePoint attribute.
        /// </summary>
        ReparsePoint = 0x00000400,

        /// <summary>
        /// Compressed attribute.
        /// </summary>
        Compressed = 0x00000800,

        /// <summary>
        /// Offline attribute.
        /// </summary>
        Offline = 0x00001000,

        /// <summary>
        /// Not content indexed attribute.
        /// </summary>
        NotContentIndexed = 0x00002000,

        /// <summary>
        /// Encrypted attribute.
        /// </summary>
        Encrypted = 0x00004000,

        /// <summary>
        /// Write through attribute.
        /// </summary>
        Write_Through = 0x80000000,

        /// <summary>
        /// Overlapped attribute.
        /// </summary>
        Overlapped = 0x40000000,

        /// <summary>
        /// No buffering attribute.
        /// </summary>
        NoBuffering = 0x20000000,

        /// <summary>
        /// Random access attribute.
        /// </summary>
        RandomAccess = 0x10000000,

        /// <summary>
        /// Sequential scan attribute.
        /// </summary>
        SequentialScan = 0x08000000,

        /// <summary>
        /// Delete on close attribute.
        /// </summary>
        DeleteOnClose = 0x04000000,

        /// <summary>
        /// Backup semantics attribute.
        /// </summary>
        BackupSemantics = 0x02000000,

        /// <summary>
        /// Post semantics attribute.
        /// </summary>
        PosixSemantics = 0x01000000,

        /// <summary>
        /// Open reparse point attribute.
        /// </summary>
        OpenReparsePoint = 0x00200000,

        /// <summary>
        /// Open no recall attribute.
        /// </summary>
        OpenNoRecall = 0x00100000,

        /// <summary>
        /// First pipe instance attribute.
        /// </summary>
        FirstPipeInstance = 0x00080000
    }
}