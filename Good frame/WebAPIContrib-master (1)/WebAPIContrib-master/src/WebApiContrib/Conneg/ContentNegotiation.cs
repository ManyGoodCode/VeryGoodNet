using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace WebApiContrib.Conneg
{
    /// <summary>
    /// 通过选择最合适的system.net.http.formating.mediatypeformatter执行内容协商.
    /// 输出传递给给定请求的格式化器，该格式化器可以序列化对象指定类型。 
    /// </summary>
    public static class ContentNegotiation
    {
        public static string Negotiate(
            this System.Net.Http.Formatting.IContentNegotiator contentNegotiator, 
            IEnumerable<string> supportedMediaTypes, 
            string accept)
        {
            return Negotiate(
                contentNegotiator, 
                supportedMediaTypes,
                // 将Select要执行的委托方法直接通过函数传入，返回类型 MediaTypeWithQualityHeaderValue
                accept.Split(',').Select(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse));
        }

        public static string Negotiate(
            this IContentNegotiator contentNegotiator,
            IEnumerable<string> supportedMediaTypes, 
            IEnumerable<string> accept)
        {
            return Negotiate(
                contentNegotiator, 
                supportedMediaTypes,
                // 将Select要执行的委托方法直接通过函数传入，返回类型 MediaTypeWithQualityHeaderValue
                accept.Select(MediaTypeWithQualityHeaderValue.Parse));
        }

        public static string Negotiate(
            this IContentNegotiator contentNegotiator, 
            IEnumerable<string> supportedMediaTypes,
            IEnumerable<MediaTypeWithQualityHeaderValue> accept)
        {
            IEnumerable<ConnegFormatter> formatters = supportedMediaTypes.Select(mt => new ConnegFormatter(mt));
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                foreach (MediaTypeWithQualityHeaderValue header in accept)
                    request.Headers.Accept.Add(header);

                ContentNegotiationResult result = contentNegotiator.Negotiate(typeof (object), request, formatters);
                return result.MediaType.MediaType;
            }
        }

        public static ContentNegotiationResult Negotiate(
            this IContentNegotiator contentNegotiator, 
            IEnumerable<MediaTypeFormatter> formatters, 
            IEnumerable<MediaTypeWithQualityHeaderValue> accept)
        {
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                foreach (MediaTypeWithQualityHeaderValue header in accept)
                    request.Headers.Accept.Add(header);

            	return contentNegotiator.Negotiate(typeof (object), request, formatters);
            }
        }

        private class ConnegFormatter : System.Net.Http.Formatting.MediaTypeFormatter
        {
            public ConnegFormatter(string mediaType)
            {
                SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType));
            }

        	public override bool CanReadType(Type type)
            {
                return true;
            }

            public override bool CanWriteType(Type type)
            {
                return true;
            }
        }
    }
}
