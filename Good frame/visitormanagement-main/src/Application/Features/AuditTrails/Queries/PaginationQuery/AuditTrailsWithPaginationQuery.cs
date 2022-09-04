// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuditTrailsQueryHandler(
            ICurrentUserService currentUserService,
            IApplicationDbContext context,
            IMapper mapper
            )
        {
            _currentUserService = currentUserService;
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedData<AuditTrailDto>> Handle(AuditTrailsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.AuditTrails
                .Where(x => x.TableName.Contains(request.Keyword))
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                    .ProjectTo<AuditTrailDto>(_mapper.ConfigurationProvider)
                    .PaginatedDataAsync(request.PageNumber, request.PageSize);

            return data;
        }
    }
}
