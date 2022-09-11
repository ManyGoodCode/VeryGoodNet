// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.DTOs;
using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Queries.GetByVisitorId
{

    public class GetByVisitorIdApprovalHistoriesQuery : IRequest<List<ApprovalHistoryDto>>, ICacheable
    {
        public int Id { get; set; }
        public GetByVisitorIdApprovalHistoriesQuery(int id)
        {
            Id = id;
        }

        public string CacheKey => ApprovalHistoryCacheKey.GetByVisitorIdCacheKey(Id);
        public MemoryCacheEntryOptions? Options => ApprovalHistoryCacheKey.MemoryCacheEntryOptions;
    }

    public class GetByVisitorIdApprovalHistoriesQueryHandler :
         IRequestHandler<GetByVisitorIdApprovalHistoriesQuery, List<ApprovalHistoryDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetByVisitorIdApprovalHistoriesQueryHandler> localizer;

        public GetByVisitorIdApprovalHistoriesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetByVisitorIdApprovalHistoriesQueryHandler> localizer
            )
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<List<ApprovalHistoryDto>> Handle(GetByVisitorIdApprovalHistoriesQuery request, CancellationToken cancellationToken)
        {

            List<ApprovalHistoryDto> data = await context.ApprovalHistories.Where(x => x.VisitorId == request.Id)
                             .ProjectTo<ApprovalHistoryDto>(configuration:mapper.ConfigurationProvider)
                             .ToListAsync(cancellationToken);
            return data;
        }
    }
}


