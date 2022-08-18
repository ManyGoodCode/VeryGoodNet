using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CacheCow.Client.Internal
{
    internal static class OtherExtensions
    {
        public static bool ShouldRevalidate(this CacheControlHeaderValue headerValue, bool defaultBehaviour)
        {
            if (headerValue == null)
                return false;
            return defaultBehaviour || headerValue.MustRevalidate || headerValue.NoCache;
        }

        public static bool IsPutPatchOrDelete(this HttpMethod method)
        {
            return method == HttpMethod.Delete ||
                method == HttpMethod.Put ||
                method.Method.ToUpper() == "PATCH";
        }

        /// <summary>
        /// 判断  HttpMethod 是否可以忽略缓存。HttpMethod.Delete/ HttpMethod.Put/HttpMethod.Get 不可以忽略
        /// </summary>
        public static bool IsCacheIgnorable(this HttpMethod method)
        {
            return !method.IsPutPatchOrDelete() && method != HttpMethod.Get;
        }
    }
}
