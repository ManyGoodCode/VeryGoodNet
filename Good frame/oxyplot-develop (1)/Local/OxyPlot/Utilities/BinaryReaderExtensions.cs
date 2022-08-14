namespace OxyPlot
{
    using System;
    using System.IO;
    using System.Text;

    public static class BinaryReaderExtensions
    {
        public static string ReadString(this BinaryReader r, int length, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return encoding.GetString(r.ReadBytes(length), 0, length);
        }

        public static uint ReadUInt32(this BinaryReader r, bool isLittleEndian)
        {
            return isLittleEndian ? r.ReadUInt32() : r.ReadBigEndianUInt32();
        }

        public static int ReadInt32(this BinaryReader r, bool isLittleEndian)
        {
            return isLittleEndian ? r.ReadInt32() : r.ReadBigEndianInt32();
        }

        public static ushort ReadUInt16(this BinaryReader r, bool isLittleEndian)
        {
            return isLittleEndian ? r.ReadUInt16() : r.ReadBigEndianUInt16();
        }

        public static double ReadDouble(this BinaryReader r, bool isLittleEndian)
        {
            return isLittleEndian ? r.ReadDouble() : r.ReadBigEndianDouble();
        }

        public static uint[] ReadUInt32Array(this BinaryReader r, int count, bool isLittleEndian)
        {
            uint[] result = new uint[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = isLittleEndian ? r.ReadUInt32() : r.ReadBigEndianUInt32();
            }

            return result;
        }

        public static ushort[] ReadUInt16Array(this BinaryReader r, int count, bool isLittleEndian)
        {
            var result = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = isLittleEndian ? r.ReadUInt16() : r.ReadBigEndianUInt16();
            }

            return result;
        }
        public static uint ReadBigEndianUInt32(this BinaryReader r)
        {
            byte[] a32 = r.ReadBytes(4);
            Array.Reverse(a32);
            return BitConverter.ToUInt32(a32, 0);
        }

        public static int ReadBigEndianInt32(this BinaryReader r)
        {
            byte[] a32 = r.ReadBytes(4);
            Array.Reverse(a32);
            return BitConverter.ToInt32(a32, 0);
        }

        public static ushort ReadBigEndianUInt16(this BinaryReader r)
        {
            byte[] a16 = r.ReadBytes(2);
            Array.Reverse(a16);
            return BitConverter.ToUInt16(a16, 0);
        }

        public static double ReadBigEndianDouble(this BinaryReader r)
        {
            byte[] a = r.ReadBytes(8);
            Array.Reverse(a);
            return BitConverter.ToDouble(a, 0);
        }
    }
}
