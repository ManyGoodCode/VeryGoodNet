using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CacheCow.Client.Headers;
using CacheCow.Client.Internal;
using CacheCow.Common;
using CacheCow.Common.Helpers;

namespace CacheCow.Client
{
    public class CachingHandler : DelegatingHandler
    {
        private readonly ICacheStore cacheStore;
        private Func<HttpRequestMessage, bool> ignoreRequestRules;
        private bool disposeCacheStore = false;
        private bool disposeVaryStore = false;

        public IVaryHeaderStore VaryHeaderStore { get; set; }
        public string[] DefaultVaryHeaders { get; set; }
        public string[] StarVaryHeaders { get; set; } // TODO: populate and use
        public bool UseConditionalPutPatchDelete { get; set; }
        public bool MustRevalidateByDefault { get; set; }
        public Func<HttpResponseMessage, ResponseValidationResult> ResponseValidator { get; set; }
        public Action<HttpResponseMessage> ResponseStoragePreparationRules { get; set; }
        public bool DoNotEmitCacheCowHeader { get; set; } = false;
        public static Action<Exception> IgnoreExceptionPolicy { get; private set; }

        private static HttpStatusCode[] cacheableStatuses = new HttpStatusCode[]
        {
             HttpStatusCode.OK,
             HttpStatusCode.NonAuthoritativeInformation,
             HttpStatusCode.PartialContent,
             HttpStatusCode.MultipleChoices,
             HttpStatusCode.MovedPermanently,
             HttpStatusCode.Gone
      };

        public CachingHandler()
            : this(new InMemoryCacheStore())
        {
            disposeCacheStore = true;
        }

        public CachingHandler(ICacheStore cacheStore)
            : this(cacheStore, new InMemoryVaryHeaderStore())
        {
            disposeVaryStore = true;
        }

        public CachingHandler(ICacheStore cacheStore, IVaryHeaderStore varyHeaderStore)
        {
            this.cacheStore = cacheStore;
            UseConditionalPutPatchDelete = true;
            MustRevalidateByDefault = true;
            VaryHeaderStore = varyHeaderStore;
            DefaultVaryHeaders = new string[] { HttpHeaderNames.Accept };
            ResponseValidator = (response) =>
            {
                if (!response.StatusCode.IsIn(cacheableStatuses))
                    return ResponseValidationResult.NotCacheable;

                if (!response.IsSuccessStatusCode ||
                    response.Headers.CacheControl == null ||
                    response.Headers.CacheControl.NoStore)
                    return ResponseValidationResult.NotCacheable;

                if (response.Headers.Date == null)
                    TraceWriter.WriteLine("Response date is NULL", TraceLevel.Warning);

                response.Headers.Date = response.Headers.Date ?? DateTimeOffset.UtcNow;
                DateTimeOffset? dateTimeOffset = response.Headers.Date;
                TimeSpan age = TimeSpan.Zero;
                if (response.Headers.Age.HasValue)
                    age = response.Headers.Age.Value;
                TraceWriter.WriteLine(
                    string.Format("CachedResponse date was => {0} - compared to UTC.Now => {1}", dateTimeOffset, DateTimeOffset.UtcNow), TraceLevel.Verbose);

                if (response.Content == null)
                    return ResponseValidationResult.NotCacheable;

                if (response.Headers.CacheControl.MaxAge == null &&
                    response.Headers.CacheControl.SharedMaxAge == null &&
                    response.Content.Headers.Expires == null)
                    return ResponseValidationResult.NotCacheable;

                if (response.Headers.CacheControl.NoCache)
                    return ResponseValidationResult.MustRevalidate;

                if (response.RequestMessage?.Headers?.CacheControl != null &&
                    response.RequestMessage.Headers.CacheControl.NoCache)
                    return ResponseValidationResult.MustRevalidate;

                if (response.Headers.CacheControl.MaxAge != null &&
                    DateTimeOffset.UtcNow > response.Headers.Date.Value.Add(response.Headers.CacheControl.MaxAge.Value.Subtract(age)))
                    return response.Headers.CacheControl.ShouldRevalidate(MustRevalidateByDefault)
                        ? ResponseValidationResult.MustRevalidate : ResponseValidationResult.Stale;

                if (response.Headers.CacheControl.SharedMaxAge != null &&
                    DateTimeOffset.UtcNow > response.Headers.Date.Value.Add(response.Headers.CacheControl.SharedMaxAge.Value.Subtract(age)))
                    return response.Headers.CacheControl.ShouldRevalidate(MustRevalidateByDefault)
                        ? ResponseValidationResult.MustRevalidate : ResponseValidationResult.Stale;

                if (response.Content.Headers.Expires != null &&
                    response.Content.Headers.Expires < DateTimeOffset.UtcNow)
                    return response.Headers.CacheControl.ShouldRevalidate(MustRevalidateByDefault)
                        ? ResponseValidationResult.MustRevalidate : ResponseValidationResult.Stale;

                return ResponseValidationResult.OK;
            };

            ignoreRequestRules = (request) =>
            {
                if (request.Method.IsCacheIgnorable())
                    return true;
                if (request.Headers.CacheControl != null)
                {
                    if (request.Headers.CacheControl.NoStore)
                        return true;
                }

                return false;
            };

            ResponseStoragePreparationRules = (response) =>
            {
                if (response.Content.Headers.Expires != null &&
                    (response.Headers.CacheControl.MaxAge != null || response.Headers.CacheControl.SharedMaxAge != null))
                {
                    response.Content.Headers.Expires = null;
                }
            };

        }

        static CachingHandler()
        {
            IgnoreExceptionPolicy = (e) => { };
        }

        internal static bool? IsFreshOrStaleAcceptable(HttpResponseMessage cachedResponse, HttpRequestMessage request)
        {
            TimeSpan staleness = TimeSpan.Zero;
            TimeSpan age = TimeSpan.Zero;
            if (cachedResponse == null)
                throw new ArgumentNullException("cachedResponse");
            if (request == null)
                throw new ArgumentNullException("request");

            if (cachedResponse.Content == null)
                return null;

            if (cachedResponse.Headers.Age.HasValue)
                age = cachedResponse.Headers.Age.Value;

            DateTimeOffset? responseDate = cachedResponse.Headers.Date ?? cachedResponse.Content.Headers.LastModified; // Date should have a value
            if (responseDate == null)
                return null;

            if (cachedResponse.Headers.CacheControl == null)
                return null;

            if (cachedResponse.Content.Headers.Expires != null)
            {
                staleness = DateTimeOffset.Now.Subtract(cachedResponse.Content.Headers.Expires.Value);
            }

            if (cachedResponse.Headers.CacheControl.MaxAge.HasValue) // Note: this is MaxAge for response
            {
                staleness = DateTimeOffset.Now.Subtract(responseDate.Value.Subtract(age).Add(cachedResponse.Headers.CacheControl.MaxAge.Value));
            }

            if (request.Headers.CacheControl == null)
                return staleness < TimeSpan.Zero;

            if (request.Headers.CacheControl.MinFresh.HasValue)
                return -staleness > request.Headers.CacheControl.MinFresh.Value; // staleness is negative if still fresh

            if (request.Headers.CacheControl.MaxStale) // stale acceptable
                return true;

            if (request.Headers.CacheControl.MaxStaleLimit.HasValue)
                return staleness < request.Headers.CacheControl.MaxStaleLimit.Value;

            if (request.Headers.CacheControl.MaxAge.HasValue)
                return responseDate.Value.Subtract(age).Add(request.Headers.CacheControl.MaxAge.Value) > DateTimeOffset.Now;

            return false;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CacheCowHeader cacheCowHeader = new CacheCowHeader();
            string uri = request.RequestUri.ToString();
            List<KeyValuePair<string, IEnumerable<string>>> originalHeaders = request.Headers.ToList();
            TraceWriter.WriteLine("{0} - Starting SendAsync", TraceLevel.Verbose, uri);
            if (ignoreRequestRules(request))
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false); // EXIT !! _________________

            IEnumerable<string> varyHeaders;
            if (!VaryHeaderStore.TryGetValue(uri, out varyHeaders))
            {
                varyHeaders = DefaultVaryHeaders;
            }

            CacheKey cacheKey = new CacheKey(uri,
                originalHeaders.Where(x => varyHeaders.Any(y => y.Equals(x.Key,
                    StringComparison.CurrentCultureIgnoreCase)))
                    .SelectMany(z => z.Value)
                );

            HttpResponseMessage cachedResponse;
            ResponseValidationResult validationResultForCachedResponse = ResponseValidationResult.NotExist;
            TraceWriter.WriteLine("{0} - Before TryGetValue", TraceLevel.Verbose, uri);
            cachedResponse = await cacheStore.GetValueAsync(cacheKey).ConfigureAwait(false);
            cacheCowHeader.DidNotExist = cachedResponse == null;
            TraceWriter.WriteLine("{0} - After TryGetValue: DidNotExist => {1}", TraceLevel.Verbose, uri, cacheCowHeader.DidNotExist);

            if (!cacheCowHeader.DidNotExist.Value) // so if it EXISTS in cache
            {
                TraceWriter.WriteLine("{0} - Existed in the cache. CacheControl Headers => {1}", TraceLevel.Verbose, uri,
                    cachedResponse.Headers.CacheControl.ToString());
                cachedResponse.RequestMessage = request;
                validationResultForCachedResponse = ResponseValidator(cachedResponse);
            }

            TraceWriter.WriteLine("{0} - After ResponseValidator => {1}",
                TraceLevel.Verbose, request.RequestUri, validationResultForCachedResponse);


            if (request.Method.IsPutPatchOrDelete() && validationResultForCachedResponse.IsIn(
                 ResponseValidationResult.OK, ResponseValidationResult.MustRevalidate))
            {
                ApplyPutPatchDeleteValidationHeaders(request, cacheCowHeader, cachedResponse);
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }

            if (validationResultForCachedResponse == ResponseValidationResult.OK)
            {
                cacheCowHeader.RetrievedFromCache = true;
                if (!DoNotEmitCacheCowHeader)
                    cachedResponse.AddCacheCowHeader(cacheCowHeader);
                return cachedResponse;
            }

            else if (validationResultForCachedResponse == ResponseValidationResult.Stale)
            {
                cacheCowHeader.WasStale = true;
                bool? isFreshOrStaleAcceptable = IsFreshOrStaleAcceptable(cachedResponse, request);
                if (isFreshOrStaleAcceptable.HasValue && isFreshOrStaleAcceptable.Value) // similar to OK
                {
                    if (!DoNotEmitCacheCowHeader)
                        cachedResponse.AddCacheCowHeader(cacheCowHeader);
                    return cachedResponse;
                }
                else
                    validationResultForCachedResponse = ResponseValidationResult.MustRevalidate; // revalidate
            }
            else if (validationResultForCachedResponse == ResponseValidationResult.MustRevalidate)
            {
                ApplyGetCacheValidationHeaders(request, cacheCowHeader, cachedResponse);
            }


            HttpResponseMessage serverResponse = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (serverResponse.Content != null)
            {
                TraceWriter.WriteLine($"Content Size: {serverResponse.Content.Headers.ContentLength}", TraceLevel.Verbose);
                if (serverResponse.Content.Headers.ContentType == null)
                {
                    serverResponse.Content.Headers.Add("Content-Type", "application/octet-stream");
                }
            }


            TraceWriter.WriteLine("{0} - After getting response", TraceLevel.Verbose, uri);

            if (request.Method != HttpMethod.Get)
                return serverResponse;

            if (validationResultForCachedResponse == ResponseValidationResult.MustRevalidate &&
                serverResponse.StatusCode == HttpStatusCode.NotModified)
            {
                TraceWriter.WriteLine("{0} - Got 304 from the server and ResponseValidationResult.MustRevalidate",
                    TraceLevel.Verbose, uri);

                cachedResponse.RequestMessage = request;
                cacheCowHeader.RetrievedFromCache = true;
                TraceWriter.WriteLine("{0} - NotModified", TraceLevel.Verbose, uri);

                await UpdateCachedResponseAsync(cacheKey, cachedResponse, serverResponse, cacheStore).ConfigureAwait(false);
                ConsumeAndDisposeResponse(serverResponse);
                if (!DoNotEmitCacheCowHeader)
                    cachedResponse.AddCacheCowHeader(cacheCowHeader).CopyOtherCacheCowHeaders(serverResponse);
                return cachedResponse;
            }

            var validationResult = ResponseValidator(serverResponse);
            switch (validationResult)
            {
                case ResponseValidationResult.MustRevalidate:
                case ResponseValidationResult.OK:

                    TraceWriter.WriteLine("{0} - ResponseValidationResult.OK or MustRevalidate",
                        TraceLevel.Verbose, uri);


                    ResponseStoragePreparationRules(serverResponse);
                    if (serverResponse.Headers.Vary != null)
                    {
                        varyHeaders = serverResponse.Headers.Vary.Select(x => x).ToArray();
                        IEnumerable<string> temp;
                        if (!VaryHeaderStore.TryGetValue(uri, out temp))
                        {
                            VaryHeaderStore.AddOrUpdate(uri, varyHeaders);
                        }
                    }

                    cacheKey = new CacheKey(uri,
                        originalHeaders.Where(x => varyHeaders.Any(y => y.Equals(x.Key,
                            StringComparison.CurrentCultureIgnoreCase)))
                            .SelectMany(z => z.Value)
                        );

                    CheckForCacheCowHeader(serverResponse);
                    await cacheStore.AddOrUpdateAsync(cacheKey, serverResponse).ConfigureAwait(false);

                    TraceWriter.WriteLine("{0} - After AddOrUpdate", TraceLevel.Verbose, uri);


                    break;
                default:
                    TraceWriter.WriteLine("{0} - ResponseValidationResult. Other",
                        TraceLevel.Verbose, uri);

                    TraceWriter.WriteLine("{0} - Before TryRemove", TraceLevel.Verbose, uri);
                    await cacheStore.TryRemoveAsync(cacheKey);
                    TraceWriter.WriteLine("{0} - After TryRemoveAsync", TraceLevel.Verbose, uri);

                    cacheCowHeader.NotCacheable = true;

                    break;
            }
            TraceWriter.WriteLine("{0} - Before returning response",
                TraceLevel.Verbose, request.RequestUri.ToString());

            if (!DoNotEmitCacheCowHeader)
                serverResponse.AddCacheCowHeader(cacheCowHeader);

            return serverResponse;
        }

        private void ApplyPutPatchDeleteValidationHeaders(HttpRequestMessage request, CacheCowHeader cacheCowHeader,
            HttpResponseMessage cachedResponse)
        {
            if (UseConditionalPutPatchDelete)
            {
                cacheCowHeader.CacheValidationApplied = true;
                if (cachedResponse.Headers.ETag != null)
                {
                    request.Headers.Add(HttpHeaderNames.IfMatch,
                        cachedResponse.Headers.ETag.ToString());
                }
                else if (cachedResponse.Content.Headers.LastModified != null)
                {
                    request.Headers.Add(HttpHeaderNames.IfUnmodifiedSince,
                        cachedResponse.Content.Headers.LastModified.Value.ToString("r"));
                }
            }
        }

        internal async static Task UpdateCachedResponseAsync(CacheKey cacheKey,
            HttpResponseMessage cachedResponse,
            HttpResponseMessage serverResponse,
            ICacheStore store)
        {
            TraceWriter.WriteLine("CachingHandler.UpdateCachedResponseAsync - response: " + serverResponse.Headers.ToString(), TraceLevel.Verbose);
            if (serverResponse.Headers.CacheControl != null && (!serverResponse.Headers.CacheControl.NoCache)) // added to cover issue #139
            {
                TraceWriter.WriteLine("CachingHandler.UpdateCachedResponseAsync - CacheControl: " + serverResponse.Headers.CacheControl.ToString(), TraceLevel.Verbose);
                cachedResponse.Headers.CacheControl = serverResponse.Headers.CacheControl;
            }
            else
            {
                TraceWriter.WriteLine("CachingHandler.UpdateCachedResponseAsync - CacheControl missing from server. Applying sliding expiration. Date => " + DateTimeOffset.UtcNow, TraceLevel.Verbose);
            }

            cachedResponse.Headers.Date = DateTimeOffset.UtcNow; // very important
            CheckForCacheCowHeader(cachedResponse);
            await store.AddOrUpdateAsync(cacheKey, cachedResponse).ConfigureAwait(false);
        }

        private static void CheckForCacheCowHeader(HttpResponseMessage responseMessage)
        {
            CacheCowHeader header = responseMessage.Headers.GetCacheCowHeader();
            if (header != null)
            {
                TraceWriter.WriteLine("!!WARNING!! response stored with CacheCowHeader!!", TraceLevel.Warning);
            }
        }

        private static void ApplyGetCacheValidationHeaders(HttpRequestMessage request, CacheCowHeader cacheCowHeader,
            HttpResponseMessage cachedResponse)
        {
            cacheCowHeader.CacheValidationApplied = true;
            cacheCowHeader.WasStale = true;
            if (cachedResponse.Headers.ETag != null)
            {
                request.Headers.Add(HttpHeaderNames.IfNoneMatch,
                    cachedResponse.Headers.ETag.ToString());
            }
            else if (cachedResponse.Content.Headers.LastModified != null)
            {
                request.Headers.Add(HttpHeaderNames.IfModifiedSince,
                    cachedResponse.Content.Headers.LastModified.Value.ToString("r"));
            }
        }

        private void ConsumeAndDisposeResponse(HttpResponseMessage response)
        {
            response.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (VaryHeaderStore != null && disposeVaryStore)
                    VaryHeaderStore.Dispose();

                if (cacheStore != null && disposeCacheStore)
                    cacheStore.Dispose();
            }
        }

    }
}
