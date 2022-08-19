using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiContrib.MessageHandlers
{
    public class SimpleCorsHandler : DelegatingHandler
    {
        private const string origin = "Origin";
        private const string accessControlRequestMethod = "Access-Control-Request-Method";
        private const string accessControlRequestHeaders = "Access-Control-Request-Headers";
        private const string accessControlAllowOrigin = "Access-Control-Allow-Origin";
        private const string accessControlAllowMethods = "Access-Control-Allow-Methods";
        private const string accessControlAllowHeaders = "Access-Control-Allow-Headers";

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            bool isCorsRequest = request.Headers.Contains(origin);
            bool isPreflightRequest = request.Method == HttpMethod.Options;
            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    return Task.Factory.StartNew(() =>
                            {
                                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                                response.Headers.Add(
                                    accessControlAllowOrigin,
                                    request.Headers.GetValues(origin).First());

                                var currentAccessControlRequestMethod =
                                    request.Headers.GetValues(accessControlRequestMethod).
                                        FirstOrDefault();

                                if (currentAccessControlRequestMethod != null)
                                {
                                    response.Headers.Add(accessControlAllowMethods,
                                                        currentAccessControlRequestMethod);
                                }

                                string requestedHeaders = string.Join(", ", request.Headers.GetValues(accessControlRequestHeaders));
                                if (!string.IsNullOrEmpty(requestedHeaders))
                                {
                                    response.Headers.Add(accessControlAllowHeaders,
                                                        requestedHeaders);
                                }

                                return response;
                            }, cancellationToken);
                }
                else
                {
                    return base.SendAsync(request, cancellationToken).ContinueWith(t =>
                            {
                                HttpResponseMessage resp = t.Result;
                                resp.Headers.Add(
                                    accessControlAllowOrigin,
                                    request.Headers.GetValues(origin).First());
                                
                                return resp;
                            });
                }
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}