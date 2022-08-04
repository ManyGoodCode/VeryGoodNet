using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002438.Entity
{
    /// <summary>
    /// 负责安全字典的职责
    /// </summary>
    public class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly object lck = new object();
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            set
            {
                lock (lck)
                {
                    TValue current;
                    if (dictionary.TryGetValue(key, out current))
                    {
                        IDisposable disposable = current as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                    }

                    dictionary[key] = value;
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (lck)
            {
                return dictionary.ContainsKey(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (lck)
            {
                return dictionary.TryGetValue(key, out value);
            }
        }

        public bool Remove(TKey key)
        {
            lock (lck)
            {
                return dictionary.Remove(key);
            }
        }

        public void Clear()
        {
            lock (lck)
            {
                dictionary.Clear();
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                return dictionary.Keys;
            }
        }

        public void Dispose()
        {
            lock (lck)
            {
                IEnumerable<IDisposable> disposableItems = from item in dictionary.Values
                                                           where item is IDisposable
                                                           select item as IDisposable;

                foreach (IDisposable item in disposableItems)
                {
                    item.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
