
namespace OxyPlot
{
    using System;
    using System.Collections.Generic;

    public class ElementCollectionChangedEventArgs<T> : EventArgs
    {
        public ElementCollectionChangedEventArgs(IEnumerable<T> addedItems, IEnumerable<T> removedItems)
        {
            this.AddedItems = new List<T>(addedItems ?? new T[] { });
            this.RemovedItems = new List<T>(removedItems ?? new T[] { });
        }

        public List<T> AddedItems { get; private set; }
        public List<T> RemovedItems { get; private set; }
    }
}
