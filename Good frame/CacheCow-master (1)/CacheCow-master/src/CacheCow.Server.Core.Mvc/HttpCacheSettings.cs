using System;

namespace CacheCow.Server.Core.Mvc
{
    public class HttpCacheSettings
    {
        public bool Enabled { get; set; } = true;
        public TimeSpan? Expiry { get; set; }
    }
}
