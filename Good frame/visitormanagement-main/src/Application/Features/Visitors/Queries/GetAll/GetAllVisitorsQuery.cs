using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.GetAll
{
    public class GetAllVisitorsQuery : IRequest<IEnumerable<VisitorDto>>, ICacheable
    {
        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllVisitorsQueryHandler :
         IRequestHandler<GetAllVisitorsQuery, IEnumerable<VisitorDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllVisitorsQueryHandler> localizer;

        public GetAllVisitorsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllVisitorsQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<VisitorDto>> Handle(GetAllVisitorsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<VisitorDto> data = await context.Visitors
                         .ProjectTo<VisitorDto>(mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}


