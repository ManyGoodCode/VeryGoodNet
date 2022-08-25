using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace SharpDX.Win32
{
    internal class ComStringEnumerator : IEnumerator<string>, IEnumerable<string>
    {
        private readonly IEnumString enumString;
        private string current;

        public ComStringEnumerator(IntPtr ptrToIEnumString)
        {
            enumString = (IEnumString)Marshal.GetObjectForIUnknown(ptrToIEnumString);
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            unsafe
            {
                var output = new string[1];
                int count = 0;
                var hasNext = enumString.Next(1, output, new IntPtr(&count)) == Result.Ok.Code;
                current = hasNext ? output[0] : null;
                return hasNext;
            }
        }

        public void Reset()
        {
            enumString.Reset();
        }

        public string Current
        {
            get { return current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}