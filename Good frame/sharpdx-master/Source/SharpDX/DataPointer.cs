using System;
using System.Runtime.InteropServices;

namespace SharpDX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DataPointer : IEquatable<DataPointer>
    {
        public static readonly DataPointer Zero = new DataPointer(IntPtr.Zero, 0);

        public DataPointer(IntPtr pointer, int size)
        {
            Pointer = pointer;
            Size = size;
        }

        public unsafe DataPointer(void* pointer, int size)
        {
            Pointer = (IntPtr)pointer;
            Size = size;
        }

        public IntPtr Pointer;
        public int Size;
        public bool IsEmpty
        {
            get { return Equals(Zero); }
        }

        public bool Equals(DataPointer other)
        {
            return Pointer.Equals(other.Pointer) && Size == other.Size;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DataPointer && Equals((DataPointer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Pointer.GetHashCode() * 397) ^ Size;
            }
        }


        public DataStream ToDataStream()
        {
            if (Pointer == IntPtr.Zero) 
                throw new InvalidOperationException("DataPointer is Zero");
            return new DataStream(this);
        }

        public DataBuffer ToDataBuffer()
        {
            if (Pointer == IntPtr.Zero) 
                throw new InvalidOperationException("DataPointer is Zero");
            return new DataBuffer(this);
        }


        public byte[] ToArray()
        {
            if (Pointer == IntPtr.Zero)
                throw new InvalidOperationException("DataPointer is Zero");
            if (Size < 0) 
                throw new InvalidOperationException("Size cannot be < 0");
            var buffer = new byte[Size];
            Utilities.Read(Pointer, buffer, 0, Size);
            return buffer;
        }

        public T[] ToArray<T>() where T : struct
        {
            if (Pointer == IntPtr.Zero) throw new InvalidOperationException("DataPointer is Zero");
            var buffer = new T[Size / Utilities.SizeOf<T>()];
            CopyTo(buffer, 0, buffer.Length);
            return buffer;
        }

        public void CopyTo<T>(T[] buffer, int offset, int count) where T : struct
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (Pointer == IntPtr.Zero) 
                throw new InvalidOperationException("DataPointer is Zero");
            if (offset < 0) 
                throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
            if (count <= 0) 
                throw new ArgumentOutOfRangeException("count", "Must be > 0");
            if (count * Utilities.SizeOf<T>() > Size) 
                throw new ArgumentOutOfRangeException("buffer", "Total buffer size cannot be larger than size of this data pointer");
            Utilities.Read(Pointer, buffer, offset, count);
        }

        public void CopyFrom<T>(T[] buffer) where T : struct
        {
            if(buffer == null) 
                throw new ArgumentNullException("buffer");
            if (Pointer == IntPtr.Zero) 
                throw new InvalidOperationException("DataPointer is Zero");
            CopyFrom(buffer, 0, buffer.Length);
        }

        public void CopyFrom<T>(T[] buffer, int offset, int count) where T : struct
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (Pointer == IntPtr.Zero) 
                throw new InvalidOperationException("DataPointer is Zero");
            if (offset < 0) 
                throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
            if (count <= 0) 
                throw new ArgumentOutOfRangeException("count", "Must be > 0");
            if (count * Utilities.SizeOf<T>() > Size) 
                throw new ArgumentOutOfRangeException("buffer", "Total buffer size cannot be larger than size of this data pointer");
            Utilities.Write(Pointer, buffer, offset, count);
        }

        public static bool operator ==(DataPointer left, DataPointer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataPointer left, DataPointer right)
        {
            return !left.Equals(right);
        }
    }
}