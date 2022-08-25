using System;
using System.Runtime.InteropServices;

namespace SharpDX.Animation
{
    internal class ManagerEventHandlerShadow : SharpDX.ComObjectShadow
    {
        private static readonly ManagerEventHandlerVtbl Vtbl = new ManagerEventHandlerVtbl();
        public static IntPtr ToIntPtr(ManagerEventHandler callback)
        {
            return ToCallbackPtr<ManagerEventHandler>(callback);
        }

        public class ManagerEventHandlerVtbl : ComObjectVtbl
        {
            public ManagerEventHandlerVtbl() : base(1)
            {
                AddMethod(new OnManagerStatusChangedDelegate(OnManagerStatusChangedImpl));
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate SharpDX.Result OnManagerStatusChangedDelegate(IntPtr thisPtr, SharpDX.Animation.ManagerStatus newStatus, SharpDX.Animation.ManagerStatus previousStatus);
            private static SharpDX.Result OnManagerStatusChangedImpl(IntPtr thisPtr, SharpDX.Animation.ManagerStatus newStatus, SharpDX.Animation.ManagerStatus previousStatus)
            {
                try
                {
                    var shadow = ToShadow<ManagerEventHandlerShadow>(thisPtr);
                    var callback = (ManagerEventHandler)shadow.Callback;
                    callback.OnManagerStatusChanged(previousStatus, newStatus);
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