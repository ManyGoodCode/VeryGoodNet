using System;
using System.Runtime.InteropServices;

namespace SharpDX.DXGI
{
    internal class VirtualSurfaceUpdatesCallbackNativeShadow : SharpDX.ComObjectShadow
    {
        private static readonly VirtualSurfaceUpdatesCallbackNativeVtbl Vtbl = new VirtualSurfaceUpdatesCallbackNativeVtbl();
        public static IntPtr ToIntPtr(IVirtualSurfaceUpdatesCallbackNative virtualSurfaceUpdatesCallbackNative)
        {
            return ToCallbackPtr<IVirtualSurfaceUpdatesCallbackNative>(virtualSurfaceUpdatesCallbackNative);
        }

        public class VirtualSurfaceUpdatesCallbackNativeVtbl : ComObjectVtbl
        {
            public VirtualSurfaceUpdatesCallbackNativeVtbl() : base(1)
            {
                AddMethod(new UpdatesNeededDelegate(UpdatesNeededImpl));
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int UpdatesNeededDelegate(IntPtr thisPtr);
            private static int UpdatesNeededImpl(IntPtr thisPtr)
            {
                try
                {
                    var shadow = ToShadow<VirtualSurfaceUpdatesCallbackNativeShadow>(thisPtr);
                    var callback = (IVirtualSurfaceUpdatesCallbackNative)shadow.Callback;
                    callback.UpdatesNeeded();
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