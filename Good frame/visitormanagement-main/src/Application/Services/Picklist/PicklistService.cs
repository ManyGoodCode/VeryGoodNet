using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LazyCache;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace CleanArchitecture.Blazor.Application.Services.Picklist
{

    public class PicklistService : IPicklistService
    {
        private const string PicklistCacheKey = "PicklistCache";
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly IAppCache cache;
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public event Action? OnChange;
        public List<KeyValueDto> DataSource { get; private set; } = new List<KeyValueDto>();

        public PicklistService(
                       IAppCache cache,
                       IApplicationDbContext context, IMapper mapper
        )
        {
            this.cache = cache;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task Initialize()
        {
            await semaphore.WaitAsync();
            try
            {
                DataSource = await cache.GetOrAddAsync(PicklistCacheKey,
                    () => context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
                        .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
                        .ToListAsync(),
                      KeyValueCacheKey.MemoryCacheEntryOptions);

            }
            finally
            {
                semaphore.Release();
            }

        }
        public async Task Refresh()
        {
            await semaphore.WaitAsync();
            try
            {
                cache.Remove(PicklistCacheKey);
                DataSource = await cache.GetOrAddAsync(PicklistCacheKey,
                    () => context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
                        .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
                        .ToListAsync(),
                    KeyValueCacheKey.MemoryCacheEntryOptions
                      );
                OnChange?.Invoke();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
