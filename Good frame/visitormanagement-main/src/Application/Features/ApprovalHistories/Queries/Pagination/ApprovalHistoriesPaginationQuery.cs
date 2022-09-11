// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.DTOs;
using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Queries.Pagination
{

    public class ApprovalHistoriesWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<ApprovalHistoryDto>>, ICacheable
    {
        public string CacheKey => ApprovalHistoryCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => ApprovalHistoryCacheKey.MemoryCacheEntryOptions;
    }

    public class ApprovalHistoriesWithPaginationQueryHandler :
         IRequestHandler<ApprovalHistoriesWithPaginationQuery, PaginatedData<ApprovalHistoryDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<ApprovalHistoriesWithPaginationQueryHandler> localizer;

        public ApprovalHistoriesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<ApprovalHistoriesWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<ApprovalHistoryDto>> Handle(
            ApprovalHistoriesWithPaginationQuery request,
            CancellationToken cancellationToken)
        {

            PaginatedData<ApprovalHistoryDto> data = await context.ApprovalHistories.Where(
                x => x.Visitor.Name.Contains(request.Keyword) ||
                x.ApprovedBy.Contains(request.Keyword))
                 .Include(x => x.Visitor)
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<ApprovalHistoryDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}