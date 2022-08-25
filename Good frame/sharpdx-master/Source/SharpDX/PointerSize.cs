﻿using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SharpDX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PointerSize : IEquatable<PointerSize>
    {
        private IntPtr _size;

        public static readonly PointerSize Zero = new PointerSize(0);

        public PointerSize(IntPtr size)
        {
            _size = size;
        }

        private unsafe PointerSize(void* size)
        {
            _size = new IntPtr(size);
        }

        public PointerSize(int size)
        {
            _size = new IntPtr(size);
        }

        public PointerSize(long size)
        {
            _size = new IntPtr(size);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}", _size);
        }

        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "{0}", _size.ToString(format));
        }

        public override int GetHashCode()
        {
            return _size.GetHashCode();
        }

        public bool Equals(PointerSize other)
        {
            return _size.Equals(other._size);
        }

        public override bool Equals(object value)
        {
            if(ReferenceEquals(null, value)) return false;
            return value is PointerSize && Equals((PointerSize)value);
        }

        public static PointerSize operator +(PointerSize left, PointerSize right)
        {
            return new PointerSize(left._size.ToInt64() + right._size.ToInt64());
        }

        public static PointerSize operator +(PointerSize value)
        {
            return value;
        }

        public static PointerSize operator -(PointerSize left, PointerSize right)
        {
            return new PointerSize(left._size.ToInt64() - right._size.ToInt64());
        }

        public static PointerSize operator -(PointerSize value)
        {
            return new PointerSize(-value._size.ToInt64());
        }

        public static PointerSize operator *(int scale, PointerSize value)
        {
            return new PointerSize(scale*value._size.ToInt64());
        }

        public static PointerSize operator *(PointerSize value, int scale)
        {
            return new PointerSize(scale*value._size.ToInt64());
        }

        public static PointerSize operator /(PointerSize value, int scale)
        {
            return new PointerSize(value._size.ToInt64()/scale);
        }

        public static bool operator ==(PointerSize left, PointerSize right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointerSize left, PointerSize right)
        {
            return !left.Equals(right);
        }

        public static implicit operator int(PointerSize value)
        {
            return value._size.ToInt32();
        }

        public static implicit operator long(PointerSize value)
        {
            return value._size.ToInt64();
        }

        public static implicit operator PointerSize(int value)
        {
            return new PointerSize(value);
        }

        public static implicit operator PointerSize(long value)
        {
            return new PointerSize(value);
        }

        public static implicit operator PointerSize(IntPtr value)
        {
            return new PointerSize(value);
        }

        public static implicit operator IntPtr(PointerSize value)
        {
            return value._size;
        }

        public static unsafe implicit operator PointerSize(void* value)
        {
            return new PointerSize(value);
        }

        public static unsafe implicit operator void*(PointerSize value)
        {
            return (void*) value._size;
        }
    }
}