using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Server
{
    public class HttpCachingOptions
    {
        public bool DoNotEmitCacheCowHeader { get; set; }
        public bool EnableConfiguration { get; set; } = false;
    }
}
