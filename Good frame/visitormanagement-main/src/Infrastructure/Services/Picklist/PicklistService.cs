using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Picklist
{

    public class PicklistService : IPicklistService
    {
        private const string PicklistCacheKey = "PicklistCache";
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly IAppCache _cache;
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public event Action? OnChange;
        public List<KeyValueDto> DataSource { get; private set; } = new();

        public PicklistService(
          IAppCache cache,
        IApplicationDbContext context, IMapper mapper)
        {
            _cache = cache;
            _context = context;
            _mapper = mapper;
        }
        public async Task Initialize()
        {
            await _semaphore.WaitAsync();
            try
            {
                DataSource = await _cache.GetOrAddAsync(PicklistCacheKey,
                    () => _context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
                        .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
                        .ToListAsync(),
                      KeyValueCacheKey.MemoryCacheEntryOptions);

            }
            finally
            {
                _semaphore.Release();
            }

        }
        public async Task Refresh()
        {
            await _semaphore.WaitAsync();
            try
            {
                _cache.Remove(PicklistCacheKey);
                DataSource = await _cache.GetOrAddAsync(PicklistCacheKey,
                    () => _context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
                        .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
                        .ToListAsync(),
                    KeyValueCacheKey.MemoryCacheEntryOptions
                      );
                OnChange?.Invoke();
            }
            finally
            {
                _semaphore.Release();
            }

        }
    }
}
