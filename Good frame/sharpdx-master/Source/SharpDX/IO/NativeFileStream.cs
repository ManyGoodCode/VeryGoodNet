using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace SharpDX.IO
{
    /// <summary>
    /// Windows File Helper.
    /// </summary>
    public class NativeFileStream : Stream
    {
        private bool canRead;
        private bool canWrite;
        private bool canSeek;
        private IntPtr handle;
        private long position;

        public unsafe NativeFileStream(string fileName, NativeFileMode fileMode, NativeFileAccess access, NativeFileShare share = NativeFileShare.Read)
        {
            handle = NativeFile.Create(fileName, access, share, IntPtr.Zero, fileMode, NativeFileOptions.None, IntPtr.Zero);
            if (handle == new IntPtr(-1))
            {
                var lastWin32Error = MarshalGetLastWin32Error();
                if (lastWin32Error == 2)
                {
                    throw new FileNotFoundException("Unable to find file", fileName);
                }

                var lastError = Result.GetResultFromWin32Error(lastWin32Error);
                throw new IOException(string.Format(CultureInfo.InvariantCulture, "Unable to open file {0}", fileName), lastError.Code);
            }

            canRead = 0 != (access & NativeFileAccess.Read);
            canWrite = 0 != (access & NativeFileAccess.Write);
            canSeek = true;

        }

        public IntPtr Handle
        {
            get { return handle; }
        }

        private static int MarshalGetLastWin32Error()
        {
            return Marshal.GetLastWin32Error();
        }

        public override void Flush()
        {
            if (!NativeFile.FlushFileBuffers(handle))
                throw new IOException("Unable to flush stream", MarshalGetLastWin32Error());
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition;
            if (!NativeFile.SetFilePointerEx(handle, offset, out newPosition, origin))
                throw new IOException("Unable to seek to this position", MarshalGetLastWin32Error());
            position = newPosition;
            return position;
        }

        public override void SetLength(long value)
        {
            long newPosition;
            if (!NativeFile.SetFilePointerEx(handle, value, out newPosition, SeekOrigin.Begin))
                throw new IOException("Unable to seek to this position", MarshalGetLastWin32Error());
            if (!NativeFile.SetEndOfFile(handle))
                throw new IOException("Unable to set the new length", MarshalGetLastWin32Error());

            if (position < value)
            {
                Seek(position, SeekOrigin.Begin);
            }
            else
            {
                Seek(0, SeekOrigin.End);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            unsafe
            {
                fixed (void* pbuffer = buffer)
                    return Read((IntPtr)pbuffer, offset, count);
            }
        }

        // Reads a block of bytes from the stream and writes the data in a given buffer.
        public int Read(IntPtr buffer, int offset, int count)
        {
            if (buffer == IntPtr.Zero)
                throw new ArgumentNullException("buffer");

            int numberOfBytesRead;
            unsafe
            {
                void* pbuffer = (byte*)buffer + offset;
                {
                    if (!NativeFile.ReadFile(handle, (IntPtr)pbuffer, count, out numberOfBytesRead, IntPtr.Zero))
                        throw new IOException("Unable to read from file", MarshalGetLastWin32Error());
                }

                position += numberOfBytesRead;
            }
            return numberOfBytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            unsafe
            {
                fixed (void* pbuffer = buffer)
                    Write((IntPtr)pbuffer, offset, count);
            }
        }

        public void Write(IntPtr buffer, int offset, int count)
        {
            if (buffer == IntPtr.Zero)
                throw new ArgumentNullException("buffer");

            int numberOfBytesWritten;
            unsafe
            {
                void* pbuffer = (byte*)buffer + offset;
                {
                    if (!NativeFile.WriteFile(handle, (IntPtr)pbuffer, count, out numberOfBytesWritten, IntPtr.Zero))
                        throw new IOException("Unable to write to file", MarshalGetLastWin32Error());
                }
                position += numberOfBytesWritten;
            }
        }

        public override bool CanRead
        {
            get { return canRead; }
        }

        public override bool CanSeek
        {
            get { return canSeek; }
        }

        public override bool CanWrite
        {
            get { return canWrite; }
        }

        public override long Length
        {
            get
            {
                long length;
                if (!NativeFile.GetFileSizeEx(handle, out length))
                    throw new IOException("Unable to get file length", MarshalGetLastWin32Error());
                return length;
            }
        }

        public override long Position
        {
            get { return position; }
            set
            {
                Seek(value, SeekOrigin.Begin);
                position = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Utilities.CloseHandle(handle);
            handle = IntPtr.Zero;
            base.Dispose(disposing);
        }
    }
}
