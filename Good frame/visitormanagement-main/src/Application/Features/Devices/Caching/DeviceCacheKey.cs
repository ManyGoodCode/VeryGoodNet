// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;

namespace CleanArchitecture.Blazor.Application.Features.Devices.Caching
{

    public static class DeviceCacheKey
    {
        public const string GetAllCacheKey = "all-Devices";
        public static string GetPagtionCacheKey(string parameters)
        {
            return $"DevicesWithPaginationQuery,{parameters}";
        }
        static DeviceCacheKey()
        {
            _tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));
        }
        private static CancellationTokenSource _tokensource;
        public static CancellationTokenSource SharedExpiryTokenSource()
        {
            if (_tokensource.IsCancellationRequested)
            {
                _tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));
            }
            return _tokensource;
        }
        public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
    }
}

