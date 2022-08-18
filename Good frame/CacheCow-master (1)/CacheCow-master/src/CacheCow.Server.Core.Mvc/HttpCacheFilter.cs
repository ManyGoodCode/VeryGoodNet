using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CacheCow.Server.Core.Mvc
{
    public class HttpCacheFilter : IAsyncResourceFilter
    {
        private ICacheabilityValidator _validator;
        private readonly HttpCachingOptions _options;
        private IConfiguration _config;
        private const string StreamName = "##__travesty_that_I_have_to_do_this__##";

        public HttpCacheFilter(
            ICacheabilityValidator validator,
            ICacheDirectiveProvider cacheDirectiveProvider,
            IOptions<HttpCachingOptions> options,
            IConfiguration configuration)
        {
            _validator = validator;
            CacheDirectiveProvider = cacheDirectiveProvider;
            ApplyNoCacheNoStoreForNonCacheableResponse = true;
            _options = options.Value;
            _config = configuration;
        }

        private HttpCacheSettings GetConfigSettings(ResourceExecutingContext context, HttpCacheSettings settings)
        {
            const string ControllerKey = "controller";
            const string ActionKey = "action";
            if (!context.RouteData.Values.ContainsKey(ControllerKey) ||
                !context.RouteData.Values.ContainsKey(ActionKey))
                return settings;

            string key = $"CacheCow:{context.RouteData.Values[ControllerKey]}:{context.RouteData.Values[ActionKey]}";
            IConfigurationSection section = _config.GetSection(key);
            if (section.Exists())
                section.Bind(settings);
            return settings;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            HttpCacheSettings settings = new HttpCacheSettings()
            {
                Expiry = ConfiguredExpiry
            };

            if (_options.EnableConfiguration)
                settings = GetConfigSettings(context, settings);

            if (!settings.Enabled)
            {
                await next();
                return;
            }

            CachingPipeline pipa = new CachingPipeline(_validator, CacheDirectiveProvider, _options)
            {
                ApplyNoCacheNoStoreForNonCacheableResponse = ApplyNoCacheNoStoreForNonCacheableResponse,
                ConfiguredExpiry = settings.Expiry
            };

            bool carryon = await pipa.Before(context.HttpContext);
            if (!carryon)
                return;

            var execCtx = await next(); // _______________________________________________________________________________
            ObjectResult or = execCtx.Result as ObjectResult;
            await pipa.After(context.HttpContext, or == null || or.Value == null ? null : or.Value);
        }

        public bool ApplyNoCacheNoStoreForNonCacheableResponse { get; set; }
        public ICacheDirectiveProvider CacheDirectiveProvider { get; set; }
        public TimeSpan? ConfiguredExpiry { get; set; }
        public bool IsConfigurationEnabled { get; set; }
    }

    public class HttpCacheFilter<T> : HttpCacheFilter
    {
        public HttpCacheFilter(
            ICacheabilityValidator validator,
            ICacheDirectiveProvider<T> cacheDirectiveProvider,
            IOptions<HttpCachingOptions> options,
            IConfiguration configuration) : base(validator, cacheDirectiveProvider, options, configuration)
        {
        }
    }
}
