using System;
using System.Runtime.InteropServices;

namespace SharpDX
{
    internal abstract class CppObjectShadow : CppObject
    {
        public ICallbackable Callback { get; private set; }
        protected abstract CppObjectVtbl GetVtbl { get; }
        public unsafe virtual void Initialize(ICallbackable callbackInstance)
        {
            this.Callback = callbackInstance;
            NativePointer = Marshal.AllocHGlobal(IntPtr.Size * 2);

            var handle = GCHandle.Alloc(this);
            Marshal.WriteIntPtr(NativePointer, GetVtbl.Pointer);

            *((IntPtr*) NativePointer + 1) = GCHandle.ToIntPtr(handle);
        }

        protected unsafe override void Dispose(bool disposing)
        {
            if (NativePointer != IntPtr.Zero)
            {
                GCHandle.FromIntPtr(*(((IntPtr*)NativePointer) + 1)).Free();
                Marshal.FreeHGlobal(NativePointer);
                NativePointer = IntPtr.Zero;
            }
            Callback = null;
            base.Dispose(disposing);
        }

        internal static T ToShadow<T>(IntPtr thisPtr) where T : CppObjectShadow
        {
            unsafe
            {
                return (T)GCHandle.FromIntPtr(*(((IntPtr*)thisPtr) + 1)).Target;
            }
        }
    }
}
