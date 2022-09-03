using System.Collections.Concurrent;

namespace WebApiContrib.Caching
{
    public class InMemoryThrottleStore : IThrottleStore
    {
        private readonly ConcurrentDictionary<string, ThrottleEntry> throttleStore = new ConcurrentDictionary<string, ThrottleEntry>();

        public bool TryGetValue(string key, out ThrottleEntry entry)
        {
            return throttleStore.TryGetValue(key, out entry);
        }

        public void IncrementRequests(string key)
        {
           throttleStore.AddOrUpdate(key,
                                       k =>
                                           {
                                               return new ThrottleEntry() {Requests = 1};
                                           },
                                       (k, e) =>
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