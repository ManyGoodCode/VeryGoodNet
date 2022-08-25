using System;
using System.Runtime.InteropServices;
using SharpDX.Direct3D;

namespace SharpDX
{
    public class DataBuffer : DisposeBase
    {
        private unsafe sbyte* _buffer;
        private GCHandle _gCHandle;
        private readonly bool _ownsBuffer;
        private readonly int _size;
#if !WINDOWS_UWP
        private Blob _blob;
#endif

        public static DataBuffer Create<T>(T[] userBuffer, int index = 0, bool pinBuffer = true) where T : struct
        {
            unsafe
            {
                if (userBuffer == null)
                    throw new ArgumentNullException("userBuffer");

                if (index < 0 || index > userBuffer.Length)
                    throw new ArgumentException("Index is out of range [0, userBuffer.Length-1]", "index");

                DataBuffer buffer;

                var sizeOfBuffer = Utilities.SizeOf(userBuffer);
                var indexOffset = index * Utilities.SizeOf<T>();

                if (pinBuffer)
                {
                    var handle = GCHandle.Alloc(userBuffer, GCHandleType.Pinned);
                    buffer = new DataBuffer(indexOffset + (byte*)handle.AddrOfPinnedObject(), sizeOfBuffer - indexOffset, handle);
                }
                else
                {
                    buffer = new DataBuffer(indexOffset + (byte*)(IntPtr)Interop.Fixed(userBuffer), sizeOfBuffer - indexOffset, true);
                }

                return buffer;
            }
        }

        public DataBuffer(int sizeInBytes)
        {
            unsafe
            {
                System.Diagnostics.Debug.Assert(sizeInBytes > 0);

                _buffer = (sbyte*)Utilities.AllocateMemory(sizeInBytes);
                _size = sizeInBytes;
                _ownsBuffer = true;
            }
        }

        public DataBuffer(DataPointer dataPointer)
            : this(dataPointer.Pointer, dataPointer.Size)
        {
        }


        public unsafe DataBuffer(IntPtr userBuffer, int sizeInBytes)
            : this((void*)userBuffer, sizeInBytes, false)
        {
        }


        internal unsafe DataBuffer(void* buffer, int sizeInBytes, GCHandle handle)
        {
            System.Diagnostics.Debug.Assert(sizeInBytes > 0);

            _buffer = (sbyte*)buffer;
            _size = sizeInBytes;
            _gCHandle = handle;
            _ownsBuffer = false;
        }

        internal unsafe DataBuffer(void* buffer, int sizeInBytes, bool makeCopy)
        {
            System.Diagnostics.Debug.Assert(sizeInBytes > 0);

            if (makeCopy)
            {
                _buffer = (sbyte*)Utilities.AllocateMemory(sizeInBytes);
                Utilities.CopyMemory((IntPtr)_buffer, (IntPtr)buffer, sizeInBytes);
            }
            else
            {
                _buffer = (sbyte*)buffer;
            }
            _size = sizeInBytes;
            _ownsBuffer = makeCopy;
        }
#if !WINDOWS_UWP
        internal unsafe DataBuffer(Blob buffer)
        {
            System.Diagnostics.Debug.Assert(buffer.GetBufferSize() > 0);

            _buffer = (sbyte*)buffer.GetBufferPointer();
            _size = buffer.GetBufferSize();
            _blob = buffer;
        }
#endif

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if !WINDOWS_UWP
                if (_blob != null)
                {
                    _blob.Dispose();
                    _blob = null;
                }
#endif
            }

            if (_gCHandle.IsAllocated)
                _gCHandle.Free();

            unsafe
            {
                if (_ownsBuffer && _buffer != (sbyte*)0)
                {
                    Utilities.FreeMemory((IntPtr)_buffer);
                    _buffer = (sbyte*)0;
                }
            }
        }

        public unsafe void Clear(byte value = 0)
        {
            Utilities.ClearMemory((IntPtr)_buffer, value, Size);
        }

        public T Get<T>(int positionInBytes) where T : struct
        {
            unsafe
            {
                T result = default(T);
                Utilities.Read((IntPtr)(_buffer + positionInBytes), ref result);
                return result;
            }
        }

        public void Get<T>(int positionInBytes, out T value) where T : struct
        {
            unsafe
            {
                Utilities.ReadOut((IntPtr)(_buffer + positionInBytes), out value);
            }
        }

        public T[] GetRange<T>(int positionInBytes, int count) where T : struct
        {
            unsafe
            {
                var result = new T[count];
                Utilities.Read((IntPtr)(_buffer + positionInBytes), result, 0, count);
                return result;
            }
        }

        public void GetRange<T>(int positionInBytes, T[] buffer, int offset, int count) where T : struct
        {
            unsafe
            {
                Utilities.Read((IntPtr)(_buffer + positionInBytes), buffer, offset, count);
            }
        }

        public void Set<T>(int positionInBytes, ref T value) where T : struct
        {
            unsafe
            {
                Interop.CopyInline(_buffer + positionInBytes, ref value);
            }
        }

        public void Set<T>(int positionInBytes, T value) where T : struct
        {
            unsafe
            {
                Interop.CopyInline(_buffer + positionInBytes, ref value);
            }
        }

        public void Set(int positionInBytes, bool value)
        {
            unsafe
            {
                *((int*)(_buffer + positionInBytes)) = value ? 1 : 0;
            }
        }

        public void Set<T>(int positionInBytes, T[] data) where T : struct
        {
            Set(positionInBytes, data, 0, data.Length);
        }

        public void Set(int positionInBytes, IntPtr source, long count)
        {
            unsafe
            {
                Utilities.CopyMemory((IntPtr)(_buffer + positionInBytes), source, (int)count);
            }
        }

        public void Set<T>(int positionInBytes, T[] data, int offset, int count) where T : struct
        {
            unsafe
            {
                Utilities.Write((IntPtr)(_buffer + positionInBytes), data, offset, count);
            }
        }

        public IntPtr DataPointer
        {
            get
            {
                unsafe
                {
                    return new IntPtr(_buffer);
                }
            }
        }

        public int Size
        {
            get { return _size; }
        }

        public static implicit operator DataPointer(DataBuffer from)
        {
            return new DataPointer(from.DataPointer, (int)from.Size);
        }
    }
}