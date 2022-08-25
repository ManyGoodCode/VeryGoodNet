using System.IO;

namespace SharpDX.Win32
{
    [Shadow(typeof(ComStreamShadow))]
    public partial interface IStream
    {
        long Seek(long offset, SeekOrigin origin);
        void SetSize(long newSize);
        long CopyTo(IStream streamDest, long numberOfBytesToCopy, out long bytesWritten);
        void Commit(CommitFlags commitFlags);
        void Revert();
        void LockRegion(long offset, long numberOfBytesToLock, LockType dwLockType);
        void UnlockRegion(long offset, long numberOfBytesToLock, LockType dwLockType);
        StorageStatistics GetStatistics(StorageStatisticsFlags storageStatisticsFlags);
        IStream Clone();
    }
}