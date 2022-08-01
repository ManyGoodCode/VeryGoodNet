using System;

namespace EasyHttp.Infrastructure
{
    public class UriComposer
    {
        readonly ObjectToUrlParameters _objectToUrlParameters;
        private readonly ObjectToUrlSegments _objectToUrlSegments;

        public UriComposer()
        {
            _objectToUrlParameters = new ObjectToUrlParameters();
            _objectToUrlSegments = new ObjectToUrlSegments();
        }

        public string Compose(string baseuri, string uri, object query, bool parametersAsSegments)
        {
            string returnUri = uri;
            if (!string.IsNullOrEmpty(baseuri))
            {
                returnUri = baseuri.EndsWith("/") ? baseuri : String.Concat(baseuri, "/");
                returnUri += uri.StartsWith("/", StringComparison.InvariantCulture) ? uri.Substring(1) : uri;
            }
            if (parametersAsSegments)
            {
                returnUri = query != null ? string.Concat(returnUri, _objectToUrlSegments.ParametersToUrl(query)) : returnUri;
            }
            else
            {
                returnUri = query != null ? string.Concat(returnUri, _objectToUrlParameters.ParametersToUrl(query)) : returnUri;
            }

            return returnUri;
        }
    }
}