// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Domain.Entities.Audit;
using MediatR;

namespace CleanArchitecture.Blazor.Application.AuditTrails.Queries.PaginationQuery
{

    public class AuditTrailsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<AuditTrailDto>>
    {
    }

    public class AuditTrailsQueryHandler : IRequestHandler<AuditTrailsWithPaginationQuery, PaginatedData<AuditTrailDto>>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public AuditTrailsQueryHandler(
            ICurrentUserService currentUserService,
            IApplicationDbContext context,
            IMapper mapper)
        {
            this.currentUserService = currentUserService;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PaginatedData<AuditTrailDto>> Handle(AuditTrailsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<AuditTrailDto> data = await context.AuditTrails
                .Where(x => x.TableName.Contains(request.Keyword))
                    //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                    .ProjectTo<AuditTrailDto>(mapper.ConfigurationProvider)
                    .PaginatedDataAsync(request.PageNumber, request.PageSize);

            return data;
        }
    }
}
