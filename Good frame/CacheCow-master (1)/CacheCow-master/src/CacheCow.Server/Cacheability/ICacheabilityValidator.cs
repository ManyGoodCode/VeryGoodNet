﻿#if NET452
using System.Net.Http;
#else
using Microsoft.AspNetCore.Http;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Server
{
    /// <summary>
    /// 是否可以缓存的验证器。验证 HttpRequestMessage 和  HttpRequest
    /// </summary>
    public interface ICacheabilityValidator
    {
#if NET452
        bool IsCacheable(HttpRequestMessage request);
#else
        bool IsCacheable(HttpRequest request);
#endif

#if NET452
        bool IsCacheable(HttpResponseMessage response);
#else
        bool IsCacheable(HttpResponse response);
#endif        

    }
}
