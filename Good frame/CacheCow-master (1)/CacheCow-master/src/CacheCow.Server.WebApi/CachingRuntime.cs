﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;

namespace CacheCow.Server.WebApi
{
    public static class CachingRuntime
    {
        public static event EventHandler<HttpCacheCreatedEventArgs> CacheFilterCreated;

        internal static void OnHttpCacheCreated(HttpCacheCreatedEventArgs args)
        {
            if (CacheFilterCreated != null)
                CacheFilterCreated(args.FilterInstance, args);
        }

        public static void RegisterDefaultTypes(Action<Type, Type, bool> registerationStub)
        {           
            Add<ICacheabilityValidator, DefaultCacheabilityValidator>(registerationStub, false);
            Add<HttpCacheAttribute, HttpCacheAttribute>(registerationStub, true);
            Add<ISerialiser, JsonSerialiser>(registerationStub, false);
            Add<IHasher, Sha1Hasher>(registerationStub, true);
            Add<ITimedETagExtractor, DefaultTimedETagExtractor>(registerationStub, true);
            Add<ITimedETagQueryProvider, NullQueryProvider>(registerationStub, false);
            Add<ICacheDirectiveProvider, DefaultCacheDirectiveProvider>(registerationStub, true);
            registerationStub(typeof(ICacheDirectiveProvider<>), typeof(DefaultCacheDirectiveProvider<>), true);
        }

        private static void Add<TI, TC>(Action<Type, Type, bool> registerationStub, bool isTransient)
        {
            registerationStub(typeof(TI), typeof(TC), isTransient);
        }

        internal static ICacheDirectiveProvider GetCacheDirectiveProvider(this IDependencyResolver resolver, Type viewModelType)
        {
            Func<ICacheDirectiveProvider> defaultFactory = () => new DefaultCacheDirectiveProvider(new DefaultTimedETagExtractor(
                    new JsonSerialiser(), new Sha1Hasher()), new NullQueryProvider());

            if (resolver == null)
                defaultFactory();

            if (viewModelType == null)
                return (ICacheDirectiveProvider) resolver.GetService(typeof(ICacheDirectiveProvider)) ?? defaultFactory();

            Type t = typeof(ICacheDirectiveProvider<>);
            Type generic = t.MakeGenericType(viewModelType);

            ICacheDirectiveProvider res = (ICacheDirectiveProvider) resolver.GetService(generic) ?? defaultFactory();
            return res;
        }
    }
}
