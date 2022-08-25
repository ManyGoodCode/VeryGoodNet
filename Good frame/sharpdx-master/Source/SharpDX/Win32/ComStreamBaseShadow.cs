using System;
using System.Runtime.InteropServices;

namespace SharpDX.Win32
{
    internal class ComStreamBaseShadow : SharpDX.ComObjectShadow
    {
        private static readonly ComStreamBaseVtbl Vtbl = new ComStreamBaseVtbl(0);

        internal class ComStreamBaseVtbl : ComObjectVtbl
        {
            public ComStreamBaseVtbl(int numberOfMethods)
                : base(numberOfMethods + 2)
            {
                AddMethod(new ReadDelegate(ReadImpl));
                AddMethod(new WriteDelegate(WriteImpl));
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int ReadDelegate(IntPtr thisPtr, IntPtr buffer, int sizeOfBytes, out int bytesRead);
            private static int ReadImpl(IntPtr thisPtr, IntPtr buffer, int sizeOfBytes, out int bytesRead)
            {
                bytesRead = 0;
                try
                {
                    ComStreamBaseShadow shadow = ToShadow<ComStreamBaseShadow>(thisPtr);
                    IStream callback = ((IStream) shadow.Callback);
                    bytesRead = callback.Read(buffer, sizeOfBytes);
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int WriteDelegate(IntPtr thisPtr, IntPtr buffer, int sizeOfBytes, out int bytesWrite);
            private static int WriteImpl(IntPtr thisPtr, IntPtr buffer, int sizeOfBytes, out int bytesWrite)
            {
                bytesWrite = 0;
                try
                {
                    ComStreamBaseShadow shadow = ToShadow<ComStreamBaseShadow>(thisPtr);
                    IStream callback = ((IStream)shadow.Callback);
                    bytesWrite = callback.Write(buffer, sizeOfBytes);
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }
        }

        protected override CppObjectVtbl GetVtbl
        {
            get { return Vtbl; }
        }
    }
}

