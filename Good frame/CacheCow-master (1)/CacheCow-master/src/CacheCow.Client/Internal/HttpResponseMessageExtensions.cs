using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CacheCow.Client.Headers;

namespace CacheCow.Client.Internal
{
    internal static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// 查找  HttpResponseMessage Headers 中的 CacheCowHeader 信息如果存在该对象则移除重新添加 形参 CacheCowHeader，没有直接添加到Headers
        /// </summary>
        public static HttpResponseMessage AddCacheCowHeader(
            this HttpResponseMessage response,
            CacheCowHeader header)
        {
            CacheCowHeader previousCacheCowHeader = response.Headers.GetCacheCowHeader();
            if (previousCacheCowHeader != null)
            {
                TraceWriter.WriteLine("WARNING: Already had this header: {0} NOw setting this: {1}", TraceLevel.Warning, previousCacheCowHeader, header);
                response.Headers.Remove(CacheCowHeader.Name);
            }

            response.Headers.Add(CacheCowHeader.Name, header.ToString());
            return response;
        }

        /// <summary>
        /// 将 HttpResponseMessage other 的 Headers 头信息 拷贝到    HttpResponseMessage response 的 Headers 中【以x-cachecow开头才有效】
        /// 
        /// </summary>
        public static HttpResponseMessage CopyOtherCacheCowHeaders(
            this HttpResponseMessage response,
            HttpResponseMessage other)
        {
            foreach (KeyValuePair<string,IEnumerable<string>> h in other.Headers)
            {
                if(h.Key.StartsWith("x-cachecow"))
                {
                    if(response.Headers.Contains(h.Key))
                        response.Headers.Remove(h.Key);

                    response.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }
            }

            return response;
        }
    }
}
