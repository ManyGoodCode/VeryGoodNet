using System;
using System.Runtime.InteropServices;

using SharpDX.Direct3D;

namespace SharpDX.DXGI
{
    public partial class DeviceChild
    {
        public T GetDevice<T>() where T : ComObject
        {
            IntPtr temp;
            GetDevice(Utilities.GetGuidFromType(typeof(T)), out temp);
            return FromPointer<T>(temp);
        }

        public string DebugName
        {
            get
            {
                unsafe
                {
                    byte* pname = stackalloc byte[1024];
                    int size = 1024 - 1;
                    if (GetPrivateData(CommonGuid.DebugObjectName, ref size, new IntPtr(pname)).Failure)
                        return string.Empty;
                    pname[size] = 0;
                    return Marshal.PtrToStringAnsi(new IntPtr(pname));
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    SetPrivateData(CommonGuid.DebugObjectName, 0, IntPtr.Zero);
                }
                else
                {
                    var namePtr = Utilities.StringToHGlobalAnsi(value);
                    SetPrivateData(CommonGuid.DebugObjectName, value.Length, namePtr);
                }
            }
        }
    }
}