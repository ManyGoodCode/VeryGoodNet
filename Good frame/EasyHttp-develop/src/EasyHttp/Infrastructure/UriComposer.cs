using System;

namespace EasyHttp.Infrastructure
{
    public class UriComposer
    {
        readonly ObjectToUrlParameters objectToUrlParameters;
        readonly ObjectToUrlSegments objectToUrlSegments;

        public UriComposer()
        {
            objectToUrlParameters = new ObjectToUrlParameters();
            objectToUrlSegments = new ObjectToUrlSegments();
        }

        public string Compose(string baseuri, string uri, object query, bool parametersAsSegments)
        {
            string returnUri = uri;
            // 函数 String Concat(params String[] values) 形式像 String.Join 。意思是 将一系列字符串组合
            if (!string.IsNullOrEmpty(baseuri))
            {
                returnUri = baseuri.EndsWith("/") ? baseuri : string.Concat(baseuri, "/");
                returnUri += uri.StartsWith("/", StringComparison.InvariantCulture) ? uri.Substring(1) : uri;
            }

            if (parametersAsSegments)
            {
                returnUri = (query != null) 
                    ? string.Concat(returnUri, objectToUrlSegments.ParametersToUrl(query)) 
                    : returnUri;
            }
            else
            {
                returnUri = (query != null) 
                    ? string.Concat(returnUri, objectToUrlParameters.ParametersToUrl(query)) 
                    : returnUri;
            }

            return returnUri;
        }
    }
}