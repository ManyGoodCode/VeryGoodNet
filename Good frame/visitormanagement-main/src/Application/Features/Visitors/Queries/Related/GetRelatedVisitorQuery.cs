using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.Related
{

    public class GetRelatedVisitorQuery : IRequest<List<VisitorDto>?>, ICacheable
    {
        public int? EmployeeId { get; private set; }
        public GetRelatedVisitorQuery(int? employeeId)
        {
            EmployeeId = employeeId;
        }

        public string CacheKey => VisitorCacheKey.GetRelatedCacheKey(EmployeeId);
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }

    public class GetRelatedVisitorQueryHandler :
         IRequestHandler<GetRelatedVisitorQuery, List<VisitorDto>?>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetRelatedVisitorQueryHandler> localizer;

        public GetRelatedVisitorQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetRelatedVisitorQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<List<VisitorDto>?> Handle(
            GetRelatedVisitorQuery request,
            CancellationToken cancellationToken)
        {
            List<VisitorDto> data = await context.Visitors.Where(x => x.EmployeeId == request.EmployeeId && x.Status != VisitorStatus.Finished)
                                  .OrderByDescending(x => x.Id)
                                  .ProjectTo<VisitorDto>(mapper.ConfigurationProvider)
                                  .ToListAsync(cancellationToken);
            return data;
        }
    }
}


