#if NET452
#else
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CacheCow.Server
{
    public interface ICachingPipeline
    {
        bool ApplyNoCacheNoStoreForNonCacheableResponse { get; set; }
        TimeSpan? ConfiguredExpiry { get; set; }
        Task After(HttpContext context, object viewModel);
        Task<bool> Before(HttpContext context);

    }

    public interface ICachingPipeline<TViewModel> : ICachingPipeline
    {

    }
}
#endif
