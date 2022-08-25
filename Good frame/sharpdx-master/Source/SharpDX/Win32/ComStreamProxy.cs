using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SharpDX.Win32
{
    [Guid("0000000c-0000-0000-C000-000000000046")]
    internal class ComStreamProxy : CallbackBase, IStream
    {
        private Stream sourceStream;
        byte[] tempBuffer = new byte[0x1000];

        public ComStreamProxy(Stream sourceStream)
        {
            this.sourceStream = sourceStream;
        }

        public unsafe int Read(IntPtr buffer, int numberOfBytesToRead)
        {
            int totalRead = 0;

            while (numberOfBytesToRead > 0)
            {
                int countRead = Math.Min(numberOfBytesToRead, tempBuffer.Length);
                int count = sourceStream.Read(tempBuffer, 0, countRead);
                if (count == 0)
                    return totalRead;
                Utilities.Write(new IntPtr(totalRead + (byte*)buffer), tempBuffer, 0, count);
                numberOfBytesToRead -= count;
                totalRead += count;
            }
            return totalRead;
        }

        public unsafe int Write(IntPtr buffer, int numberOfBytesToWrite)
        {
            int totalWrite = 0;

            while (numberOfBytesToWrite > 0)
            {
                int countWrite = Math.Min(numberOfBytesToWrite, tempBuffer.Length);
                Utilities.Read(new IntPtr(totalWrite + (byte*)buffer), tempBuffer, 0, countWrite);
                sourceStream.Write(tempBuffer, 0, countWrite);
                numberOfBytesToWrite -= countWrite;
                totalWrite += countWrite;
            }
            return totalWrite;
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return sourceStream.Seek(offset, origin);
        }

        public void SetSize(long newSize)
        {
        }

        public unsafe long CopyTo(IStream streamDest, long numberOfBytesToCopy, out long bytesWritten)
        {
            bytesWritten = 0;

            fixed (void* pBuffer = tempBuffer)
            {
                while (numberOfBytesToCopy > 0)
                {
                    int countCopy = (int)Math.Min(numberOfBytesToCopy, tempBuffer.Length);
                    int count = sourceStream.Read(tempBuffer, 0, countCopy);
                    if (count == 0)
                        break;
                    streamDest.Write((IntPtr)pBuffer, count);
                    numberOfBytesToCopy -= count;
                    bytesWritten += count;
                }
            }
            return bytesWritten;
        }

        public void Commit(CommitFlags commitFlags)
        {
            sourceStream.Flush();
        }

        public void Revert()
        {
            throw new NotImplementedException();
        }

        public void LockRegion(long offset, long numberOfBytesToLock, LockType dwLockType)
        {
            throw new NotImplementedException();
        }

        public void UnlockRegion(long offset, long numberOfBytesToLock, LockType dwLockType)
        {
            throw new NotImplementedException();
        }

        public StorageStatistics GetStatistics(StorageStatisticsFlags storageStatisticsFlags)
        {
            long length = sourceStream.Length;
            if (length == 0)
                length = 0x7fffffff;

            return new StorageStatistics
                {
                    Type = 2, // IStream
                    CbSize = length,
                    GrfLocksSupported = 2, // exclusive
                    GrfMode = 0x00000002, // read-write
                };
        }

        public IStream Clone()
        {
            return new ComStreamProxy(sourceStream);
        }

        protected override void Dispose(bool disposing)
        {
            sourceStream = null;
            base.Dispose(disposing);
        }
    }
}