using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    static class PartitionExtensions
    {
        public static Tuple<IEnumerable<T>,IEnumerable<T>> PartitionByPredicate<T>(
            this IEnumerable<T> items,
            Func<T, bool> pred)
        {
            List<T> yes = new List<T>();
            List<T> no = new List<T>();
            foreach (T item in items) {
                List<T> list = pred(item) ? yes : no;
                list.Add(item);
            }
            return Tuple.Create<IEnumerable<T>,IEnumerable<T>>(yes, no);
        }
    }
}
