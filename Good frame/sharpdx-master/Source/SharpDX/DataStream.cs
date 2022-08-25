using System;
using System.IO;
using System.Runtime.InteropServices;
using SharpDX.Direct3D;

namespace SharpDX
{
    public class DataStream : Stream
    {
        private unsafe byte* _buffer;
        private readonly bool _canRead;
        private readonly bool _canWrite;
        private GCHandle _gCHandle;
        private Blob _blob;
        private readonly bool _ownsBuffer;
        private long _position;
        private readonly long _size;

        public DataStream(Blob buffer)
        {
            unsafe
            {
                System.Diagnostics.Debug.Assert(buffer.GetBufferSize() > 0);

                _buffer = (byte*) buffer.GetBufferPointer();
                _size = buffer.GetBufferSize();
                _canRead = true;
                _canWrite = true;
                _blob = buffer;
            }
        }

        public static DataStream Create<T>(T[] userBuffer, bool canRead, bool canWrite, int index = 0, bool pinBuffer = true) where T : struct
        {
            unsafe
            {
                if (userBuffer == null)
                    throw new ArgumentNullException("userBuffer");

                if (index < 0 || index > userBuffer.Length)
                    throw new ArgumentException("Index is out of range [0, userBuffer.Length-1]", "index");

                DataStream stream;

                var sizeOfBuffer = Utilities.SizeOf(userBuffer);
                var indexOffset = index * Utilities.SizeOf<T>();

                if (pinBuffer)
                {
                    var handle = GCHandle.Alloc(userBuffer, GCHandleType.Pinned);
                    stream = new DataStream(indexOffset + (byte*)handle.AddrOfPinnedObject(), sizeOfBuffer - indexOffset, canRead, canWrite, handle);
                }
                else
                {
                    stream = new DataStream(indexOffset + (byte*)(IntPtr)Interop.Fixed(userBuffer), sizeOfBuffer - indexOffset, canRead, canWrite, true);
                }

                return stream;
            }
        }

        public DataStream(int sizeInBytes, bool canRead, bool canWrite)
        {
            unsafe
            {
                System.Diagnostics.Debug.Assert(sizeInBytes > 0);

                _buffer = (byte*) Utilities.AllocateMemory(sizeInBytes);
                _size = sizeInBytes;
                _ownsBuffer = true;
                _canRead = canRead;
                _canWrite = canWrite;
            }
        }

        public DataStream(DataPointer dataPointer) : this(dataPointer.Pointer, dataPointer.Size, true, true)
        {
        }

        public DataStream(IntPtr userBuffer, long sizeInBytes, bool canRead, bool canWrite)
        {
            unsafe
            {
                System.Diagnostics.Debug.Assert(userBuffer != IntPtr.Zero);
                System.Diagnostics.Debug.Assert(sizeInBytes > 0);
                _buffer = (byte*) userBuffer.ToPointer();
                _size = sizeInBytes;
                _canRead = canRead;
                _canWrite = canWrite;
            }
        }

        internal unsafe DataStream(void* dataPointer, int sizeInBytes, bool canRead, bool canWrite, GCHandle handle)
        {
            System.Diagnostics.Debug.Assert(sizeInBytes > 0);
            _gCHandle = handle;
            _buffer = (byte*)dataPointer;
            _size = sizeInBytes;
            _canRead = canRead;
            _canWrite = canWrite;
            _ownsBuffer = false;
        }

        internal unsafe DataStream(void* buffer, int sizeInBytes, bool canRead, bool canWrite, bool makeCopy)
        {
            System.Diagnostics.Debug.Assert(sizeInBytes > 0);
            if (makeCopy)
            {
                _buffer = (byte*) Utilities.AllocateMemory(sizeInBytes);
                Utilities.CopyMemory((IntPtr) _buffer, (IntPtr) buffer, sizeInBytes);
            }
            else
            {
                _buffer = (byte*) buffer;
            }
            _size = sizeInBytes;
            _canRead = canRead;
            _canWrite = canWrite;
            _ownsBuffer = makeCopy;
        }
        
        ~DataStream()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_blob != null)
                {
                    _blob.Dispose();
                    _blob = null;
                }
            }

            if (_gCHandle.IsAllocated)
                _gCHandle.Free();

            unsafe
            {
                if (_ownsBuffer && _buffer != (byte*)0)
                {
                    Utilities.FreeMemory((IntPtr)_buffer);
                    _buffer = (byte*)0;
                }
            }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// <exception cref = "T:System.NotSupportedException">Always thrown.</exception>
        public override void Flush()
        {
            throw new NotSupportedException("DataStream objects cannot be flushed.");
        }
        public T Read<T>() where T : struct
        {
            unsafe
            {
                if (!_canRead)
                    throw new NotSupportedException();

                byte* from = _buffer + _position;
                T result = default(T);
                _position = (byte*) Utilities.ReadAndPosition((IntPtr)from, ref result) - _buffer;
                return result;
            }
        }

        /// <inheritdoc/>
        public unsafe override int ReadByte()
        {
            if (_position >= Length)
                return -1;

            return _buffer[_position++];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int minCount = (int)Math.Min(RemainingLength, count);
            return ReadRange(buffer, offset, minCount);
        }

        public void Read(IntPtr buffer, int offset, int count)
        {
            unsafe
            {
                Utilities.CopyMemory(new IntPtr((byte*)buffer + offset), (IntPtr)(_buffer + _position), count);
                _position += count;
            }
        }

        public T[] ReadRange<T>(int count) where T : struct
        {
            unsafe
            {
                if (!_canRead)
                    throw new NotSupportedException();

                byte* from = _buffer + _position;
                var result = new T[count];
                _position = (byte*) Utilities.Read((IntPtr)from, result, 0, count) - _buffer;
                return result;
            }
        }

        public int ReadRange<T>(T[] buffer, int offset, int count) where T : struct
        {
            unsafe
            {
                if (!_canRead)
                    throw new NotSupportedException();

                var oldPosition = _position;
                _position = (byte*)Utilities.Read((IntPtr)(_buffer + _position), buffer, offset, count) - _buffer;
                return (int) (_position - oldPosition);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long targetPosition = 0;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    targetPosition = offset;
                    break;

                case SeekOrigin.Current:
                    targetPosition = _position + offset;
                    break;

                case SeekOrigin.End:
                    targetPosition = _size - offset;
                    break;
            }

            if (targetPosition < 0)
                throw new InvalidOperationException("Cannot seek beyond the beginning of the stream.");
            if (targetPosition > _size)
                throw new InvalidOperationException("Cannot seek beyond the end of the stream.");

            _position = targetPosition;
            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("DataStream objects cannot be resized.");
        }

        public void Write<T>(T value) where T : struct
        {
            if (!_canWrite)
                throw new NotSupportedException();
            unsafe
            {
                _position = (byte*) Utilities.WriteAndPosition((IntPtr)(_buffer + _position), ref value) - _buffer;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteRange(buffer, offset, count);
        }

        public void Write(IntPtr buffer, int offset, int count)
        {
            unsafe
            {
                Utilities.CopyMemory((IntPtr) (_buffer + _position), new IntPtr((byte*) buffer + offset), count);
                _position += count;
            }
        }
        
        public void WriteRange<T>(T[] data) where T : struct
        {
            WriteRange(data, 0, data.Length);
        }

        public void WriteRange(IntPtr source, long count)
        {
            unsafe
            {
                if (!_canWrite)
                    throw new NotSupportedException();

                System.Diagnostics.Debug.Assert(_canWrite);
                System.Diagnostics.Debug.Assert(source != IntPtr.Zero);
                System.Diagnostics.Debug.Assert(count > 0);
                System.Diagnostics.Debug.Assert((_position + count) <= _size);
                Utilities.CopyMemory((IntPtr) (_buffer + _position), source, (int) count);
                _position += count;
            }
        }

        public void WriteRange<T>(T[] data, int offset, int count) where T : struct
        {
            unsafe
            {
                if (!_canWrite)
                    throw new NotSupportedException();

                _position = (byte*) Utilities.Write((IntPtr)(_buffer + _position), data, offset, count) - _buffer;
            }
        }

        public override bool CanRead
        {
            get { return _canRead; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return _canWrite; }
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

        public override long Length
        {
            get { return _size; }
        }
        public override long Position
        {
            get { return _position; }
            set { Seek(value, SeekOrigin.Begin); }
        }
        public IntPtr PositionPointer
        {
            get
            {
                unsafe
                {
                    return (IntPtr) (_buffer + _position);
                }
            }
        }

        public long RemainingLength
        {
            get { return (_size - _position); }
        }

        public static implicit operator DataPointer(DataStream from)
        {
            return new DataPointer(from.PositionPointer, (int)from.RemainingLength);
        }
    }
}