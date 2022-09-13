using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.GetById
{
    public class GetByIdVisitorQuery : IRequest<VisitorDto?>, ICacheable
    {
        public int Id { get; private set; }
        public GetByIdVisitorQuery(int id)
        {
            Id = id;
        }


        public string CacheKey => VisitorCacheKey.GetByIdCacheKey(Id);
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }

    public class GetByIdVisitorQueryQueryHandler :
         IRequestHandler<GetByIdVisitorQuery, VisitorDto?>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetByIdVisitorQueryQueryHandler> localizer;

        public GetByIdVisitorQueryQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetByIdVisitorQueryQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<VisitorDto?> Handle(GetByIdVisitorQuery request, CancellationToken cancellationToken)
        {
            List<VisitorDto> data = await context.Visitors.Where(x => x.Id == request.Id)
                            .Include(x => x.Employee)
                            .Include(x => x.Companions)
                            .Include(x => x.ApprovalHistories)
                            .Include(x => x.VisitorHistories).ThenInclude(x => x.CheckinPoint).ThenInclude(x => x.Site)
                            .ProjectTo<VisitorDto>(mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);
            return data.FirstOrDefault();
        }
    }
}


