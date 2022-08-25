using System;
using System.IO;

namespace SharpDX.Multimedia
{
    public class RiffChunk
    {
        public RiffChunk(Stream stream, FourCC type, uint size, uint dataPosition, bool isList = false, bool isHeader = false)
        {
            Stream = stream;
            Type = type;
            Size = size;
            DataPosition = dataPosition;
            IsList = isList;
            IsHeader = isHeader;
        }

        public Stream Stream{ get; private set; }
        public FourCC Type { get; private set; }
        public uint Size { get; private set; }
        public uint DataPosition { get; private set; }

        public bool IsList { get; private set; }
        public bool IsHeader { get; private set; }
        public byte[] GetData()
        {
            byte[] data = new byte[Size];
            Stream.Position = DataPosition;
            Stream.Read(data, 0, (int)Size);
            return data;
        }

        public unsafe T GetDataAs<T>() where T : struct
        {
            T value = new T();
            byte[] data = GetData();
            fixed (void* ptr = data)
            {
                Utilities.Read((IntPtr) ptr, ref value);
            }
            return value;
        }

        public unsafe T[] GetDataAsArray<T>() where T : struct
        {
            int sizeOfT = Utilities.SizeOf<T>();
            if ((Size % sizeOfT) != 0)
                throw new ArgumentException("Size of T is incompatible with size of chunk");

            T[] values = new T[Size/sizeOfT];
            byte[] data = GetData();
            fixed (void* ptr = data)
            {
                Utilities.Read((IntPtr)ptr, values, 0, values.Length);
            }
            return values;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "Type: {0}, Size: {1}, Position: {2}, IsList: {3}, IsHeader: {4}", Type, Size, DataPosition, IsList, IsHeader);
        }
    }
}