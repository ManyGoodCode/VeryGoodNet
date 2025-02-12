using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Common.Mappings
{
    /// <summary>
    /// 映射扩展 非AutoMapper实现的另外一种方式
    /// </summary>
    public static class MappingExtensions
    {
        public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
            this IQueryable<TDestination> queryable,
            int pageNumber,
            int pageSize) where TDestination : class
            => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);

        public static Task<PaginatedData<TDestination>> PaginatedDataAsync<TDestination>(
            this IQueryable<TDestination> queryable,
            int pageNumber,
            int pageSize) where TDestination : class
                => PaginatedData<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);

        public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
            this IQueryable queryable,
            IConfigurationProvider configuration) where TDestination : class
                => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync();
    }
}
