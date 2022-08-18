using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CacheCow.Common;
#if NET452
using System.Runtime.Caching;
#else
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
#endif

namespace CacheCow.Client
{
    /// <summary>
    /// 通过 MemoryCache 实现 以 Key 为 uri， Value 为 headers 的缓存器
    /// </summary>
    public class InMemoryVaryHeaderStore : IVaryHeaderStore
    {
        private const string CacheName = "###_IVaryHeaderStore_###";
        private readonly ConcurrentDictionary<string, string[]> _varyHeaderCache = new ConcurrentDictionary<string, string[]>();

#if NET452
        private MemoryCache cache = new MemoryCache(CacheName);  
#else
        private MemoryCache cache = new MemoryCache(optionsAccessor: Options.Create(new MemoryCacheOptions()));
#endif

        /// <summary>
        /// 尝试获取  Key 为 uri 的 headers 值
        /// </summary>
        public bool TryGetValue(string uri, out IEnumerable<string> headers)
        {
            headers = (string[])cache.Get(key: uri);
            return headers != null;
        }

        /// <summary>
        /// 添加或修改  Key 为 uri 的 headers 值。超时时间为 DateTimeOffset.MaxValue
        /// </summary>
        public void AddOrUpdate(string uri, IEnumerable<string> headers)
        {
            cache.Set(key: uri, value: headers, absoluteExpiration: DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// 尝试移除  Key 为 uri 的 headers 值。
        /// </summary>
        public bool TryRemove(string uri)
        {
#if NET452
            return cache.Remove(uri) != null;
#endif
            cache.Remove(key: uri);
            return true;
        }

        /// <summary>
        /// 清除缓存器所有的键值对【先释放再新建】
        /// </summary>
        public void Clear()
        {
            ((IDisposable)cache).Dispose();
#if NET452
            cache = new MemoryCache(CacheName);  
#else
            cache = new MemoryCache(optionsAccessor: Options.Create(new MemoryCacheOptions()));
#endif
        }

        /// <summary>
        /// 释放缓存器
        /// </summary>
        public void Dispose()
        {
            cache.Dispose();
        }
    }
}
