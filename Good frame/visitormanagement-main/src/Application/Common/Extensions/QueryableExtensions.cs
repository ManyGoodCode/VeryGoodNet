using System;
using System.Linq;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Common.Extensions
{
    /// <summary>
    /// 通过聚合函数，将表达式进行重新组�?
    /// </summary>
    public static class QueryableExtensions
    {
        public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class, IEntity
        {
            IQueryable<T>? queryableResultWithIncludes = spec.Includes
               .Aggregate(seed: query, func: (current, include) => current.Include(include));
            IQueryable<T>? secondaryResult = spec.IncludeStrings
                .Aggregate(seed: queryableResultWithIncludes, func: (current, include) => current.Include(include));
            return secondaryResult.Where(spec.Criteria);
        }
    }
}
