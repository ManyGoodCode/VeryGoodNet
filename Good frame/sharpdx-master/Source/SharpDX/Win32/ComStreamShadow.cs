using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SharpDX.Win32
{
    internal class ComStreamShadow : ComStreamBaseShadow
    {
        private static readonly ComStreamVtbl Vtbl = new ComStreamVtbl();
        protected override CppObjectVtbl GetVtbl
        {
            get { return Vtbl; } 
        }

        public static IntPtr ToIntPtr(IStream stream)
        {
            return ToCallbackPtr<IStream>(stream);
        }

        private class ComStreamVtbl : ComStreamBaseVtbl
        {
            public ComStreamVtbl() : base(9)
            {
                AddMethod(new SeekDelegate(SeekImpl));
                AddMethod(new SetSizeDelegate(SetSizeImpl));
                AddMethod(new CopyToDelegate(CopyToImpl));
                AddMethod(new CommitDelegate(CommitImpl));
                AddMethod(new RevertDelegate(RevertImpl));
                AddMethod(new LockRegionDelegate(LockRegionImpl));
                AddMethod(new UnlockRegionDelegate(UnlockRegionImpl));
                AddMethod(new StatDelegate(StatImpl));
                AddMethod(new CloneDelegate(CloneImpl));
            }


            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private unsafe delegate int SeekDelegate(IntPtr thisPtr, long offset, SeekOrigin origin, IntPtr newPosition);
            private unsafe static int SeekImpl(IntPtr thisPtr, long offset, SeekOrigin origin, IntPtr newPosition)
            {
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    long position = callback.Seek(offset, origin);
                    if (newPosition != IntPtr.Zero)
                    {
                         *(long*)newPosition = position;
                    }
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result SetSizeDelegate(IntPtr thisPtr, long newSize);
            private static Result SetSizeImpl(IntPtr thisPtr, long newSize)
            {
                Result result = Result.Ok;
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    callback.SetSize(newSize);
                }
                catch (SharpDXException exception)
                {
                    result = exception.ResultCode;
                }
                catch (Exception)
                {
                    result = Result.Fail.Code;
                }
                return result;
            }


            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int CopyToDelegate(IntPtr thisPtr, IntPtr streamPointer, long numberOfBytes, out long numberOfBytesRead, out long numberOfBytesWritten);
            private static int CopyToImpl(IntPtr thisPtr, IntPtr streamPointer, long numberOfBytes, out long numberOfBytesRead, out long numberOfBytesWritten)
            {
                numberOfBytesRead = 0;
                numberOfBytesWritten = 0;
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    numberOfBytesRead = callback.CopyTo(new ComStream(streamPointer), numberOfBytes, out numberOfBytesWritten);
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }


            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result CommitDelegate(IntPtr thisPtr, CommitFlags flags);
            private static Result CommitImpl(IntPtr thisPtr, CommitFlags flags)
            {
                Result result = Result.Ok;
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    callback.Commit(flags);
                }
                catch (SharpDXException exception)
                {
                    result = exception.ResultCode;
                }
                catch (Exception)
                {
                    result = Result.Fail.Code;
                }

                return result;
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result RevertDelegate(IntPtr thisPtr);
            private static Result RevertImpl(IntPtr thisPtr)
            {
                Result result = Result.Ok;
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    callback.Revert();
                }
                catch (SharpDXException exception)
                {
                    result = exception.ResultCode;
                }
                catch (Exception)
                {
                    result = Result.Fail.Code;
                }
                return result;
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result LockRegionDelegate(IntPtr thisPtr, long offset, long numberOfBytes, LockType lockType);
            private static Result LockRegionImpl(IntPtr thisPtr, long offset, long numberOfBytes, LockType lockType)
            {
                Result result = Result.Ok;
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    callback.LockRegion(offset, numberOfBytes, lockType);
                }
                catch (SharpDXException exception)
                {
                    result = exception.ResultCode;
                }
                catch (Exception)
                {
                    result = Result.Fail.Code;
                }
                return result;
            }


            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result UnlockRegionDelegate(IntPtr thisPtr, long offset, long numberOfBytes, LockType lockType);
            private static Result UnlockRegionImpl(IntPtr thisPtr, long offset, long numberOfBytes, LockType lockType)
            {
                Result result = Result.Ok;
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    callback.UnlockRegion(offset, numberOfBytes, lockType);
                }
                catch (SharpDXException exception)
                {
                    result = exception.ResultCode;
                }
                catch (Exception)
                {
                    result = Result.Fail.Code;
                }
                return result;
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result StatDelegate(IntPtr thisPtr, ref StorageStatistics.__Native statisticsPtr, StorageStatisticsFlags flags);
            private static Result StatImpl(IntPtr thisPtr, ref StorageStatistics.__Native statisticsPtr, StorageStatisticsFlags flags)
            {
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    var statistics = callback.GetStatistics(flags);
                    statistics.__MarshalTo(ref statisticsPtr);
                }
                catch (SharpDXException exception)
                {
                    return exception.ResultCode;
                }
                catch (Exception)
                {
                    return Result.Fail.Code;
                }
                return Result.Ok;
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate Result CloneDelegate(IntPtr thisPtr, out IntPtr streamPointer);
            private static Result CloneImpl(IntPtr thisPtr, out IntPtr streamPointer)
            {
                streamPointer = IntPtr.Zero;
                Result result = Result.Ok;
                try
                {
                    ComStreamShadow shadow = ToShadow<ComStreamShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    IStream clone = callback.Clone();
                    streamPointer = ComStreamShadow.ToIntPtr(clone);
                }
                catch (SharpDXException exception)
                {
                    result = exception.ResultCode;
                }
                catch (Exception)
                {
                    result = Result.Fail.Code;
                }

                return result;
            }
        }
    }
}

