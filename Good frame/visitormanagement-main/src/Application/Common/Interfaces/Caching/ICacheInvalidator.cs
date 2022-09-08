using System.Threading;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Caching
{

    public interface ICacheInvalidator
    {
        string CacheKey { get => string.Empty; }
        System.Threading.CancellationTokenSource? SharedExpiryTokenSource { get => null; }
    }
}
