using System;
using System.Runtime.InteropServices;

namespace SharpDX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Size2 : IEquatable<Size2>
    {
        public static readonly Size2 Zero = new Size2(0, 0);
        public static readonly Size2 Empty = Zero;
        public Size2(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width;
        public int Height;

        public bool Equals(Size2 other)
        {
            return other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size2))
                return false;

            return Equals((Size2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }

        public static bool operator ==(Size2 left, Size2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Size2 left, Size2 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", Width, Height);
        }
    }
}