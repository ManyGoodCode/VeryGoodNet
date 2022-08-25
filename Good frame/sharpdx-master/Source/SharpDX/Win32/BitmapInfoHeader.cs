﻿using System.Runtime.InteropServices;

namespace SharpDX.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfoHeader
    {
        public int SizeInBytes;
        public int Width;
        public int Height;
        public short PlaneCount;
        public short BitCount;
        public int Compression;
        public int SizeImage;
        public int XPixelsPerMeter;
        public int YPixelsPerMeter;
        public int ColorUsedCount;
        public int ColorImportantCount;
    }
}