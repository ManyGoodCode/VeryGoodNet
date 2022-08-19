using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebApiContrib.ResponseMessages;

namespace WebApiContrib.MessageHandlers
{
    public class ETagHandler : DelegatingHandler
    {
        public static ConcurrentDictionary<string, EntityTagHeaderValue> ETagCache = new ConcurrentDictionary<string, EntityTagHeaderValue>();

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                string resource = request.RequestUri.ToString();
                ICollection<EntityTagHeaderValue> etags = request.Headers.IfNoneMatch;
                EntityTagHeaderValue actualEtag = null;
                if (etags.Count > 0 && ETagHandler.ETagCache.TryGetValue(resource, out actualEtag))
                {
                    if (etags.Any(etag => etag.Tag == actualEtag.Tag))
                    {
                        return Task.Factory.StartNew<HttpResponseMessage>(task => new NotModifiedResponse(), cancellationToken);
                    }
                }
            }
            else if (request.Method == HttpMethod.Put)
            {
                string resource = request.RequestUri.ToString();

                ICollection<EntityTagHeaderValue> etags = request.Headers.IfMatch;
                EntityTagHeaderValue actualEtag = null;
                if (etags.Count > 0 && ETagHandler.ETagCache.TryGetValue(resource, out actualEtag))
                {
                    bool matchFound = etags.Any(etag => etag.Tag == actualEtag.Tag);
                    if (!matchFound)
                    {
                        return Task.Factory.StartNew<HttpResponseMessage>(task => new ConflictResponse(), cancellationToken);
                    }
                }
            }
            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                HttpResponseMessage httpResponse = task.Result;
                string eTagKey = request.RequestUri.ToString();
                EntityTagHeaderValue eTagValue;
                if (!ETagCache.TryGetValue(eTagKey, out eTagValue) || request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
                {
                    eTagValue = new EntityTagHeaderValue("\"" + Guid.NewGuid().ToString() + "\"");
                    ETagCache.AddOrUpdate(eTagKey, eTagValue, (key, existingVal) => eTagValue);
                }

                httpResponse.Headers.ETag = eTagValue;
                return httpResponse;
            });
        }
    }
}