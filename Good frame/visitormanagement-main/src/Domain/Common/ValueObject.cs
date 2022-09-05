using System.Collections.Generic;
using System.Linq;

namespace CleanArchitecture.Blazor.Domain.Common
{
    public abstract class ValueObject
    {
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
            {
                return false;
            }

            return left?.Equals(right) != false;
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !EqualOperator(left, right);
        }

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            ValueObject other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Vs 2022 的好处
        /// 1. Net6.0 支持的版本为 Vs2022 其他版本不支持
        /// 2. Vs2022 支持函数查看源码，例如聚合函数  Aggregate
        /// </summary>
        public override int GetHashCode()
        {
            IEnumerable<int> hashCodes = GetEqualityComponents().Select(x => x != null ? x.GetHashCode() : 0);
            return hashCodes.Aggregate((x, y) => x ^ y);
        }
    }
}
