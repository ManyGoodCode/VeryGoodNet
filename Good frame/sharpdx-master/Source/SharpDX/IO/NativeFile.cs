using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpDX.IO
{
    // Windows 文件 帮助
    public static class NativeFile
    {
        private const string KERNEL_FILE = "kernel32.dll";

        // 检查文件路径是否存在
        public static bool Exists(string filePath)
        {
            try
            {
                WIN32_FILE_ATTRIBUTE_DATA data;
                if (GetFileAttributesEx(filePath, 0, out data))
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        // Opens a binary file, reads the contents of the file into a byte array, and then closes the file.
        public static byte[] ReadAllBytes(string path)
        {
            byte[] buffer;
            using (var stream = new NativeFileStream(path, NativeFileMode.Open, NativeFileAccess.Read))
            {
                int offset = 0;
                long length = stream.Length;
                if (length > 0x7fffffffL)
                {
                    throw new IOException("File too long");
                }

                int count = (int)length;
                buffer = new byte[count];

                while (count > 0)
                {
                    int num4 = stream.Read(buffer, offset, count);
                    if (num4 == 0)
                    {
                        throw new EndOfStreamException();
                    }
                    offset += num4;
                    count -= num4;
                }
            }

            return buffer;
        }

        // Opens a text file, reads all lines of the file, and then closes the file.
        public static string ReadAllText(string path)
        {
            return ReadAllText(path, Encoding.UTF8);
        }

        // Opens a text file, reads all lines of the file, and then closes the file.
        public static string ReadAllText(string path, Encoding encoding, NativeFileShare sharing = NativeFileShare.Read)
        {
            using (var stream = new NativeFileStream(path, NativeFileMode.Open, NativeFileAccess.Read, sharing))
            {
                using (StreamReader reader = new StreamReader(stream, encoding, true, 0x400))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FILETIME
        {
            public uint DateTimeLow;
            public uint DateTimeHigh;

            public DateTime ToDateTime()
            {
                return DateTime.FromFileTimeUtc((((long)DateTimeHigh) << 32) | ((uint)DateTimeLow));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WIN32_FILE_ATTRIBUTE_DATA
        {
            public uint FileAttributes;
            public FILETIME CreationTime;
            public FILETIME LastAccessTime;
            public FILETIME LastWriteTime;
            public uint FileSizeHigh;
            public uint FileSizeLow;
        }

        // Gets the last write time access for the specified path.
        public static DateTime GetLastWriteTime(string path)
        {
            WIN32_FILE_ATTRIBUTE_DATA data;
            if (GetFileAttributesEx(path, 0, out data))
            {
                return data.LastWriteTime.ToDateTime().ToLocalTime();
            }
            return new DateTime(0);
        }

        // Reads to a file.
        [DllImport(KERNEL_FILE, EntryPoint = "ReadFile", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool ReadFile(IntPtr fileHandle, IntPtr buffer, int numberOfBytesToRead, out int numberOfBytesRead, IntPtr overlapped);


        [DllImport(KERNEL_FILE, EntryPoint = "FlushFileBuffers", SetLastError = true)]
        internal static extern bool FlushFileBuffers(IntPtr hFile);

        // Writes to a file.
        [DllImport(KERNEL_FILE, EntryPoint = "WriteFile", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool WriteFile(IntPtr fileHandle, IntPtr buffer, int numberOfBytesToRead, out int numberOfBytesRead, IntPtr overlapped);

        // Sets the file pointer.
        [DllImport(KERNEL_FILE, EntryPoint = "SetFilePointerEx", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetFilePointerEx(IntPtr handle, long distanceToMove, out long distanceToMoveHigh, SeekOrigin seekOrigin);

        // Sets the end of file.
        [DllImport(KERNEL_FILE, EntryPoint = "SetEndOfFile", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetEndOfFile(IntPtr handle);

        [DllImport(KERNEL_FILE, EntryPoint = "GetFileAttributesExW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetFileAttributesEx(string name, int fileInfoLevel, out WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

        // Creates the file.
        [DllImport("kernel32.dll", EntryPoint = "CreateFile", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr Create(
            string fileName,
            NativeFileAccess desiredAccess,
            NativeFileShare shareMode,
            IntPtr securityAttributes,
            NativeFileMode mode,
            NativeFileOptions flagsAndOptions,
            IntPtr templateFile);

        // Gets the size of the file.
        [DllImport("kernel32.dll", EntryPoint = "GetFileSizeEx", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetFileSizeEx(IntPtr handle, out long fileSize);
    }
}
