using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpDX.Win32
{
    public class PropertyBag : ComObject
    {
        private IPropertyBag2 nativePropertyBag;
        public PropertyBag(IntPtr propertyBagPointer) : base(propertyBagPointer)
        {
        }

        protected override void NativePointerUpdated(IntPtr oldNativePointer)
        {
            base.NativePointerUpdated(oldNativePointer);
            if (NativePointer != IntPtr.Zero)
                nativePropertyBag = (IPropertyBag2)Marshal.GetObjectForIUnknown(NativePointer);
            else
                nativePropertyBag = null;
        }

        private void CheckIfInitialized()
        {
            if (nativePropertyBag == null)
                throw new InvalidOperationException("This instance is not bound to an unmanaged IPropertyBag2");
        }

        public int Count
        {
            get
            {
                CheckIfInitialized();
                int propertyCount;
                nativePropertyBag.CountProperties(out propertyCount);
                return propertyCount;
            }
        }

        public string[] Keys
        {
            get
            {
                CheckIfInitialized();
                var keys = new List<string>();
                for (int i = 0; i < Count; i++)
                {
                    PROPBAG2 propbag2;
                    int temp;
                    nativePropertyBag.GetPropertyInfo(i, 1, out propbag2, out temp);
                    keys.Add(propbag2.Name);
                }
                return keys.ToArray();
            }
        }

        public object Get(string name)
        {
            CheckIfInitialized();
            object value;
            var propbag2 = new PROPBAG2() {Name = name};
            Result error;
            var result = nativePropertyBag.Read(1, ref propbag2, IntPtr.Zero, out value, out error);
            if (result.Failure || error.Failure)
                throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Property with name [{0}] is not valid for this instance", name));
            propbag2.Dispose();
            return value;
        }

        public T1 Get<T1, T2>(PropertyBagKey<T1, T2> propertyKey)
        {
            var value = Get(propertyKey.Name);
            return (T1) Convert.ChangeType(value, typeof (T1));
        }

        public void Set(string name, object value)
        {
            CheckIfInitialized();
            var previousValue = Get(name);
            value = Convert.ChangeType(value, previousValue==null?value.GetType() : previousValue.GetType());

            var propbag2 = new PROPBAG2() { Name = name };
            var result = nativePropertyBag.Write(1, ref propbag2, value);
            result.CheckError();
            propbag2.Dispose();
        }

        public void Set<T1,T2>(PropertyBagKey<T1,T2> propertyKey, T1 value)
        {
            Set(propertyKey.Name, value);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PROPBAG2 : IDisposable
        {
            internal uint type;
            internal ushort vt;
            internal ushort cfType;
            internal IntPtr dwHint;
            internal IntPtr pstrName;
            internal Guid clsid;

            public string Name
            {
                get
                {
                    unsafe
                    {
                        return Marshal.PtrToStringUni(pstrName);
                    }
                }
                set
                {
                    pstrName = Marshal.StringToCoTaskMemUni(value);
                }
            }

            public void Dispose()
            {
                if (pstrName != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pstrName);
                    pstrName = IntPtr.Zero;
                }
            }
        }
        
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("22F55882-280B-11D0-A8A9-00A0C90C2004")]
        private interface IPropertyBag2
        {
            [PreserveSig()]
            Result Read([In] int cProperties, [In] ref PROPBAG2 pPropBag, IntPtr pErrLog, [Out] out object pvarValue, out Result phrError);
            [PreserveSig()]
            Result Write([In] int cProperties, [In] ref PROPBAG2 pPropBag, ref object value);
            [PreserveSig()]
            Result CountProperties(out int pcProperties);
            [PreserveSig()]
            Result GetPropertyInfo([In] int iProperty, [In] int cProperties, out PROPBAG2 pPropBag, out int pcProperties);
            [PreserveSig()]
            Result LoadObject([In, MarshalAs(UnmanagedType.LPWStr)] string pstrName, [In] uint dwHint, [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkObject, IntPtr pErrLog);
        }
    }
}