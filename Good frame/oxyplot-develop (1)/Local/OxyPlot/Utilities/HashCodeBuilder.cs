namespace OxyPlot
{
    using System.Collections.Generic;
    using System.Linq;

    public static class HashCodeBuilder
    {
        public static int GetHashCode(IEnumerable<object> items)
        {

            unchecked
            {
                return items.Where(item => item != null).Aggregate(17, (current, item) => (current * 23) + item.GetHashCode());
            }
        }
    }
}