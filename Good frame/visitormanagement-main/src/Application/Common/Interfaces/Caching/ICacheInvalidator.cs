using System.Threading;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Caching
{

    public interface ICacheInvalidator
    {
        string CacheKey { get => string.Empty; }
        CancellationTokenSource? SharedExpiryTokenSource { get => null; }
    }
}
