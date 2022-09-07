using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Caching
{

    public interface ICacheable
    {
        string CacheKey { get => string.Empty; }
        MemoryCacheEntryOptions? Options { get; }
    }
}
