using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Caching
{
    public static class DocumentTypeCacheKey
    {
        public const string GetAllCacheKey = "all-documenttypes";
        static DocumentTypeCacheKey()
        {
            SharedExpiryTokenSource = new CancellationTokenSource(new TimeSpan(12, 0, 0));
        }
        public static CancellationTokenSource SharedExpiryTokenSource { get; private set; }
        public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
            new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource.Token));
    }
}
