using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace CacheCow.Common.Helpers
{
	public static class HttpHeadersExtensions
	{
        /// <summary>
        /// 判断HttpHeads是否添加了(name,value)的头
        /// </summary>
		public static void Parse(this HttpHeaders httpHeaders, string headers)
		{
			if (httpHeaders == null)
				throw new ArgumentNullException("httpHeaders");

			if (headers == null)
				throw new ArgumentNullException("headers");

			string name = null, value = null;
			foreach (string header in headers.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				int indexOfColon = header.IndexOf(":");
				name = header.Substring(0, indexOfColon);
				value = header.Substring(indexOfColon + 1).Trim();
				if(!httpHeaders.TryAddWithoutValidation(name, value))
					throw new InvalidOperationException(string.Format("Value {0} for header {1} not acceptable.", value, name));
			}
		}

        /// <summary>
        /// 提取HttpRequestHeads中名称包含在headNames中的项
        /// </summary>
        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ExtractHeadersValues(
            this HttpRequestHeaders headers, 
            params string[] headerNames)
        {
            return headers.Where(x =>
                headerNames.Any(h => h.Equals(x.Key, StringComparison.CurrentCultureIgnoreCase)));
        }
	}
}
