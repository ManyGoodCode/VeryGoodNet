// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.DTOs;
using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Caching;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Queries.GetAll
{

    public class GetAllApprovalHistoriesQuery : IRequest<IEnumerable<ApprovalHistoryDto>>, ICacheable
    {
        public string CacheKey => ApprovalHistoryCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions Options => ApprovalHistoryCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllApprovalHistoriesQueryHandler :
         IRequestHandler<GetAllApprovalHistoriesQuery, IEnumerable<ApprovalHistoryDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllApprovalHistoriesQueryHandler> localizer;

        public GetAllApprovalHistoriesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllApprovalHistoriesQueryHandler> localizer
            )
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<ApprovalHistoryDto>> Handle(GetAllApprovalHistoriesQuery request, CancellationToken cancellationToken)
        {
            List<ApprovalHistoryDto> data = await context.ApprovalHistories
                         .ProjectTo<ApprovalHistoryDto>(configuration: mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}


