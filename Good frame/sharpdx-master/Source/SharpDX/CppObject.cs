﻿using System;

namespace SharpDX
{
    public class CppObject : DisposeBase, ICallbackable
    {
        protected internal unsafe void* _nativePointer;
        public object Tag { get; set; }
        public CppObject(IntPtr pointer)
        {
            NativePointer = pointer;
        }

        protected CppObject()
        {
        }

        public IntPtr NativePointer
        {
            get
            {
                unsafe
                {
                    return (IntPtr)_nativePointer;
                }
            }
            set
            {
                unsafe
                {
                    var newNativePointer = (void*)value;
                    if (_nativePointer != newNativePointer)
                    {
                        NativePointerUpdating();
                        var oldNativePointer = _nativePointer;
                        _nativePointer = newNativePointer;
                        NativePointerUpdated((IntPtr)oldNativePointer);
                    }
                }
            }
        }

        public static explicit operator IntPtr(CppObject cppObject)
        {
            return cppObject == null ? IntPtr.Zero : cppObject.NativePointer;
        }

        protected void FromTemp(CppObject temp)
        {
            NativePointer = temp.NativePointer;
            temp.NativePointer = IntPtr.Zero;
        }

        protected void FromTemp(IntPtr temp)
        {
            NativePointer = temp;
        }

        protected virtual void NativePointerUpdating()
        {
        }

        protected virtual void NativePointerUpdated(IntPtr oldNativePointer)
        {
        }

        protected override void Dispose(bool disposing)
        {
        }

        public static T FromPointer<T>(IntPtr comObjectPtr) where T : CppObject
        {
            return (comObjectPtr == IntPtr.Zero) ? null : (T)Activator.CreateInstance(typeof(T), comObjectPtr);
        }

        internal static T FromPointerUnsafe<T>(IntPtr comObjectPtr)
        {
            return (comObjectPtr == IntPtr.Zero) ? (T)(object)null : (T)Activator.CreateInstance(typeof(T), comObjectPtr);
        }

        public static IntPtr ToCallbackPtr<TCallback>(ICallbackable callback)
            where TCallback : ICallbackable
        {
            if (callback == null)
                return IntPtr.Zero;

            if (callback is CppObject)
                return ((CppObject)callback).NativePointer;

            var shadowContainer = callback.Shadow as ShadowContainer;
            if (shadowContainer == null)
            {
                shadowContainer = new ShadowContainer();
                shadowContainer.Initialize(callback);
            }

            return shadowContainer.Find(typeof(TCallback));
        }

        IDisposable ICallbackable.Shadow
        {
            get { throw new InvalidOperationException("Invalid access to Callback. This is used internally."); }
            set { throw new InvalidOperationException("Invalid access to Callback. This is used internally."); }
        }
    }
}