using System;
using System.Collections.Generic;
using System.Text;

namespace F002434.Caching
{
    public interface IThrottleStore
    {
        bool TryGetValue(string key, out ThrottleEntry entry);

        /// <summary>
        /// 增加请求次数
        /// </summary>
        void IncrementRequests(string key);

        void Rollover(string key);
        void Clear();
    }
}
