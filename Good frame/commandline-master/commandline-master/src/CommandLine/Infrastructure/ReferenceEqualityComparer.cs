using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CommandLine.Infrastructure
{
    internal sealed class ReferenceEqualityComparer : IEqualityComparer, IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Default = new ReferenceEqualityComparer();

        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
