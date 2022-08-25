using System;
using System.Runtime.InteropServices;

namespace SharpDX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Size2F : IEquatable<Size2F>
    {
        public static readonly Size2F Zero = new Size2F(0, 0);
        public static readonly Size2F Empty = Zero;
        public Size2F(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width;
        public float Height;

        public bool Equals(Size2F other)
        {
            return other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size2F))
                return false;

            return Equals((Size2F)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
            }
        }

        public static bool operator ==(Size2F left, Size2F right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Size2F left, Size2F right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", Width, Height);
        }
    }
}