
namespace OxyPlot
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ElementCollection<T> : IList<T>, IReadOnlyList<T> where T : Element
    {
        private readonly Model parent;
        private readonly List<T> internalList = new List<T>();
        public ElementCollection(Model parent)
        {
            this.parent = parent;
        }

        [Obsolete("May be removed in v4.0 (#111)")]
        public event EventHandler<ElementCollectionChangedEventArgs<T>> CollectionChanged;

        public int Count
        {
            get
            {
                return this.internalList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                return this.internalList[index];
            }

            set
            {
                value.Parent = this.parent;
                this.internalList[index] = value;
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(T item)
        {
            if (item.Parent != null)
            {
                throw new InvalidOperationException("The element cannot be added, it already belongs to a PlotModel.");
            }

            item.Parent = this.parent;
            this.internalList.Add(item);

            this.RaiseCollectionChanged(new[] { item });
        }

        public void Clear()
        {
            var removedItems = new List<T>();

            foreach (var item in this.internalList)
            {
                item.Parent = null;
                removedItems.Add(item);
            }

            this.internalList.Clear();

            this.RaiseCollectionChanged(removedItems: removedItems);
        }

        public bool Contains(T item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }
        
        public bool Remove(T item)
        {
            item.Parent = null;
            var result = this.internalList.Remove(item);
            if (result)
            {
                this.RaiseCollectionChanged(removedItems: new[] { item });
            }

            return result;
        }

        public int IndexOf(T item)
        {
            return this.internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (item.Parent != null)
            {
                throw new InvalidOperationException("The element cannot be inserted, it already belongs to a PlotModel.");
            }

            item.Parent = this.parent;
            this.internalList.Insert(index, item);

            this.RaiseCollectionChanged(new[] { item });
        }

        public void RemoveAt(int index)
        {
            var item = this[index];
            item.Parent = null;

            this.internalList.RemoveAt(index);

            this.RaiseCollectionChanged(removedItems: new[] { item });
        }

        private void RaiseCollectionChanged(IEnumerable<T> addedItems = null, IEnumerable<T> removedItems = null)
        {
            var collectionChanged = this.CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new ElementCollectionChangedEventArgs<T>(addedItems, removedItems));
            }
        }
    }
}
