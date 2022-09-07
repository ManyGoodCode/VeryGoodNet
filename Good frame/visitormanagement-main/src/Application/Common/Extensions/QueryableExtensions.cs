using System.Linq;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Common.Extensions
{

    public static class QueryableExtensions
    {
        public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class, IEntity
        {
            IQueryable<T>? queryableResultWithIncludes = spec.Includes
               .Aggregate(query, (current, include) => current.Include(include));
            IQueryable<T>? secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));
            return secondaryResult.Where(spec.Criteria);
        }
    }
}
