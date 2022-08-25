using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpDX
{
    internal class CppObjectVtbl
    {
        private readonly List<Delegate> methods;
        private readonly IntPtr vtbl;

        public CppObjectVtbl(int numberOfCallbackMethods)
        {
            vtbl = Marshal.AllocHGlobal(IntPtr.Size * numberOfCallbackMethods);
            methods = new List<Delegate>();
        }

        public IntPtr Pointer
        {
            get { return vtbl; }
        }

        public unsafe void AddMethod(Delegate method)
        {
            int index = methods.Count;
            methods.Add(method);
            *((IntPtr*) vtbl + index) = Marshal.GetFunctionPointerForDelegate(method);
        }
    }
}