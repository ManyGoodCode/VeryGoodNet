using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace F002222.Caching
{
    public class InMemoryThrottleStore : IThrottleStore
    {
        private readonly ConcurrentDictionary<string, ThrottleEntry> throttleStore
            = new ConcurrentDictionary<string, ThrottleEntry>();

        public bool TryGetValue(string key, out ThrottleEntry entry)
        {
            return throttleStore.TryGetValue(key, out entry);
        }

        public void IncrementRequests(string key)
        {
            throttleStore.AddOrUpdate(key: key,
                                       addValueFactory: k =>
                                       {
                                           return new ThrottleEntry()
                                           {
                                               Requests = 1 
                                           };
                                       },
                                       updateValueFactory: (k, e) =>
                                       {
                                           e.Requests++;
                                           return e;
                                       });
        }

        public void Rollover(string key)
        {
            ThrottleEntry dummy;
            throttleStore.TryRemove(key, out dummy);
        }

        public void Clear()
        {
            throttleStore.Clear();
        }
    }
}
