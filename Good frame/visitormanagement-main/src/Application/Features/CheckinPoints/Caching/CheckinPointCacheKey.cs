// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.Caching
{

    public static class CheckinPointCacheKey
    {
        public const string GetAllCacheKey = "all-CheckinPoints";
        public static string GetPagtionCacheKey(string parameters)
        {
            return $"CheckinPointsWithPaginationQuery,{parameters}";
        }
        static CheckinPointCacheKey()
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

