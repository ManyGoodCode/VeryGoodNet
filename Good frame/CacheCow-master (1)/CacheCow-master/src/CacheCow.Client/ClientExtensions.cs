using CacheCow.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CacheCow.Client
{
    public static class ClientExtensions
    {
        /// <summary>
        /// 创建 HttpClient 通过 缓存器 InMemoryCacheStore 和 HttpClientHandler
        /// </summary>
        public static HttpClient CreateClient(HttpMessageHandler handler = null)
        {
            return new HttpClient(handler: new CachingHandler()
            {
                InnerHandler = handler ?? new HttpClientHandler()
            });
        }

        /// <summary>
        /// 创建 HttpClient 通过 缓存器 ICacheStore 和 HttpClientHandler
        /// </summary>
        public static HttpClient CreateClient(this ICacheStore store, HttpMessageHandler handler = null)
        {
            return new HttpClient(handler: new CachingHandler(store)
            {
                InnerHandler = handler ?? new HttpClientHandler()
            });
        }
    }
}
