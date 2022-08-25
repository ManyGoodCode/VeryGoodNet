using System;
using System.Collections.Generic;

namespace SharpDX.Collections
{
    internal static class EqualityComparer
    {
        public static readonly IEqualityComparer<IntPtr> DefaultIntPtr = new IntPtrComparer();

        internal class IntPtrComparer : EqualityComparer<IntPtr>
        {
            public override bool Equals(IntPtr x, IntPtr y)
            {
                return x == y;
            }

            public override int GetHashCode(IntPtr obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}