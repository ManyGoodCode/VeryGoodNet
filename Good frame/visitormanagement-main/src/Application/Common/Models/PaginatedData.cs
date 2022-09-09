
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Common.Models
{
    /// <summary>
    /// 当前页 / 总项数 / 总页数 / 前页存在 / 后页存在 
    /// </summary>
    public class PaginatedData<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public IEnumerable<T> Items { get; set; }
        public PaginatedData(IEnumerable<T> items, int total, int pageIndex, int pageSize)
        {
            Items = items;
            TotalItems = total;
            CurrentPage = pageIndex;
            TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        }
        public static async Task<PaginatedData<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int count = await source.CountAsync();
            List<T> items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedData<T>(items, count, pageIndex, pageSize);
        }
    }
}
