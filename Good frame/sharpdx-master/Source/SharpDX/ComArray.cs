using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpDX
{
    public class ComArray : DisposeBase, IEnumerable
    {
        protected ComObject[] values;
        private IntPtr nativeBuffer;

        public ComArray(params ComObject[] array)
        {
            values = array;
            nativeBuffer = IntPtr.Zero;
            if (values != null)
            {
                int length = array.Length;
                values = new ComObject[length];
                nativeBuffer = Utilities.AllocateMemory(length * Utilities.SizeOf<IntPtr>());
                for (int i = 0; i < length; i++)
                    Set(i, array[i]);
            }
        }

        public ComArray(int size)
        {
            values = new ComObject[size];
            nativeBuffer = Utilities.AllocateMemory(size * Utilities.SizeOf<IntPtr>());
        }

        public IntPtr NativePointer
        {
            get { return nativeBuffer; }
        }

        public int Length
        {
            get { return values == null ? 0 : values.Length; }
        }

        public ComObject Get(int index)
        {
            return values[index];
        }

        internal void SetFromNative(int index, ComObject value)
        {
            values[index] = value;
            unsafe
            {
                value.NativePointer = ((IntPtr*)nativeBuffer)[index];
            }
        }

        public void Set(int index, ComObject value)
        {
            values[index] = value;
            unsafe
            {
                ((IntPtr*)nativeBuffer)[index] = value.NativePointer;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                values = null;
            }
            Utilities.FreeMemory(nativeBuffer);
            nativeBuffer = IntPtr.Zero;
        }

        public IEnumerator GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }

    public class ComArray<T> : ComArray, IEnumerable<T> where T : ComObject
    {
        public ComArray(params T[] array) : base(array)
        {
        }

        public ComArray(int size) : base(size)
        {
        }

        public T this[int i]
        {
            get
            {
                return (T)Get(i);
            }
            set
            {
                Set(i, value);
            }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return new ArrayEnumerator<T>(values.GetEnumerator());
        }

        private struct ArrayEnumerator<T1> : IEnumerator<T1> where T1 : ComObject
        {
            private readonly IEnumerator enumerator;

            public ArrayEnumerator(IEnumerator enumerator)
            {
                this.enumerator = enumerator;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }

            public T1 Current
            {
                get
                {
                    return (T1)enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
        }
    }
}