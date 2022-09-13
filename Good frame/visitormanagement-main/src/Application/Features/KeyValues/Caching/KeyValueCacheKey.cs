// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Caching
{

    public static class KeyValueCacheKey
    {
        public const string GetAllCacheKey = "all-keyvalues";
        public static string GetCacheKey(string name)
        {
            return $"{name}-keyvalues";
        }
        static KeyValueCacheKey()
        {
            tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));

        }

        private static CancellationTokenSource tokensource;

        public static CancellationTokenSource SharedExpiryTokenSource()
        {
            if (tokensource.IsCancellationRequested)
            {
                tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));
            }

            return tokensource;
        }
        public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
            new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
    }
}
