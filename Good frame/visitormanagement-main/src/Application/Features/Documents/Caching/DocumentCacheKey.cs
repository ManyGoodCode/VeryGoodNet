using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Caching
{

    public static class DocumentCacheKey
    {
        public const string GetAllCacheKey = "all-documents";
        static DocumentCacheKey()
        {
            SharedExpiryTokenSource = new CancellationTokenSource(new TimeSpan(12, 0, 0));
        }

        public static CancellationTokenSource SharedExpiryTokenSource { get; private set; }
        public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
            new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource.Token));
    }
}
