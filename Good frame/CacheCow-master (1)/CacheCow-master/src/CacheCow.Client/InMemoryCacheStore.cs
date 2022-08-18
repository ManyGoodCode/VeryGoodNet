using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using CacheCow.Common;
using System.Threading.Tasks;
using CacheCow.Common.Helpers;

#if NET452
using System.Runtime.Caching;
#else
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
#endif

namespace CacheCow.Client
{
    /// <summary>
    /// 通过  MemoryCache 构造缓存器。主键为加密的CacheKey，带有过期时间
    /// </summary>
    public class InMemoryCacheStore : ICacheStore
    {
        private const string CacheStoreEntryName = "###InMemoryCacheStore_###";
        private static TimeSpan MinCacheExpiry = TimeSpan.FromHours(6);
        private MemoryCache responseCache;

        private MessageContentHttpMessageSerializer messageSerializer = new MessageContentHttpMessageSerializer(true);
        private readonly TimeSpan minExpiry;

#if NET452
#else
        private readonly IOptions<MemoryCacheOptions> options;
#endif
        public InMemoryCacheStore()
            : this(minExpiry: MinCacheExpiry) { }

        public InMemoryCacheStore(TimeSpan minExpiry) :
#if NET452
            this(minExpiry:minExpiry, cache:new MemoryCache(CacheStoreEntryName))
#else
            this(minExpiry: minExpiry, options: Options.Create(new MemoryCacheOptions()))
#endif
        {

        }

        private InMemoryCacheStore(TimeSpan minExpiry, MemoryCache cache)
        {
            this.minExpiry = minExpiry;
            responseCache = cache;
        }

#if NET452
#else
        public InMemoryCacheStore(TimeSpan minExpiry, IOptions<MemoryCacheOptions> options) :
            this(minExpiry, new MemoryCache(options))
        {
            this.options = options;
        }
#endif


        public void Dispose()
        {
            responseCache.Dispose();
        }

        /// <summary>
        /// 通过 CacheKey 的加密字段做为主键 从 MemoryCache 中 得到byte[]数据 ，并通过 反序列化 得到最后的 HttpResponseMessage
        /// </summary>
        public async Task<HttpResponseMessage> GetValueAsync(CacheKey key)
        {
            byte[] result = (byte[])responseCache.Get(key.HashBase64);
            if (result == null)
                return null;

            return await messageSerializer.DeserializeToResponseAsync(new MemoryStream(result)).ConfigureAwait(false);
        }

        /// <summary>
        /// 通过 CacheKey 的加密字段做为主键 将 HttpResponseMessage 的数据存放到 MemoryCache 中，带有过期时间
        /// </summary>
        public async Task AddOrUpdateAsync(CacheKey key, HttpResponseMessage response)
        {
            HttpRequestMessage req = response.RequestMessage;
            response.RequestMessage = null;
            MemoryStream memoryStream = new MemoryStream();
            await messageSerializer.SerializeAsync(response, memoryStream).ConfigureAwait(false);
            byte[] buffer = memoryStream.ToArray();
            response.RequestMessage = req;
            DateTimeOffset suggestedExpiry = response.GetExpiry() ?? DateTimeOffset.UtcNow.Add(minExpiry);
            DateTimeOffset expiry = DateTimeOffset.UtcNow.Add(minExpiry);
            DateTimeOffset optimalExpiry = (suggestedExpiry > expiry) ? suggestedExpiry : expiry;
            responseCache.Set(key.HashBase64, buffer, optimalExpiry);
        }

        /// <summary>
        /// 通过 CacheKey 的加密字段做为主键 移除缓存器内的值
        /// </summary>
        public Task<bool> TryRemoveAsync(CacheKey key)
        {

#if NET452
            return Task.FromResult(responseCache.Remove(key.HashBase64) != null);
#else
            responseCache.Remove(key.HashBase64);
            return Task.FromResult(true);
#endif
        }

        /// <summary>
        /// 清除缓存器内的值。释放 MemoryCache 对象 ，并新建 MemoryCache 对象
        /// </summary>
        public Task ClearAsync()
        {
            responseCache.Dispose();
#if NET452
            responseCache = new MemoryCache(CacheStoreEntryName);
#else
            responseCache = new MemoryCache(options);
#endif
            return Task.FromResult(0);
        }
    }
}
