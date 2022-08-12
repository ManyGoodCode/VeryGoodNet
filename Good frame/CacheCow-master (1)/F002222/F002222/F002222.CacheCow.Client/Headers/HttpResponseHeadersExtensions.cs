using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace CacheCow.Client.Headers
{
    public static class HttpResponseHeadersExtensions
    {
        public static CacheCowHeader GetCacheCowHeader(this HttpResponseHeaders headers)
        {
            CacheCowHeader header = null;
            if (headers == null)
                return null;

            KeyValuePair<string, IEnumerable<string>> cacheCowHeader = headers.FirstOrDefault(x => x.Key == CacheCowHeader.Name);
            if (cacheCowHeader.Value != null && cacheCowHeader.Value.Count() > 0)
            {
                string last = cacheCowHeader.Value.Last();
                CacheCowHeader.TryParse(last, out header);
            }

            return header;
        }

    }
}
