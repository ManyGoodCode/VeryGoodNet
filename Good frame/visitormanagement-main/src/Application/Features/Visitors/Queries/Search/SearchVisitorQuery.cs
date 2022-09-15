using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.Search
{

    public class SearchVisitorQuery : IRequest<VisitorDto?>, ICacheable
    {
        public string Keyword { get; private set; }
        public SearchVisitorQuery(string keyword)
        {
            Keyword = keyword;
        }

        public string CacheKey => VisitorCacheKey.Search(Keyword);
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }

    public class SearchVisitorFuzzyQuery : IRequest<List<VisitorDto>>, ICacheable
    {
        public string Keyword { get; private set; }
        public SearchVisitorFuzzyQuery(string keyword)
        {
            Keyword = keyword;
        }

        public string CacheKey => VisitorCacheKey.SearchFuzzy(Keyword);
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }



    public class SearchVisitorQueryHandler :
         IRequestHandler<SearchVisitorFuzzyQuery, List<VisitorDto>>,
         IRequestHandler<SearchVisitorQuery, VisitorDto?>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<SearchVisitorQueryHandler> localizer;

        public SearchVisitorQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<SearchVisitorQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<VisitorDto?> Handle(SearchVisitorQuery request, CancellationToken cancellationToken)
        {
            Visitor item = await context.Visitors.OrderByDescending(x => x.Id).Include(x => x.Site).Include(x => x.Employee).Include(x => x.Companions).Include(x => x.ApprovalHistories).FirstOrDefaultAsync(x => x.PassCode == request.Keyword || x.Email == request.Keyword || x.PhoneNumber == request.Keyword || x.Name == request.Keyword);
            if (item is null)
                return null;
            VisitorDto dto = mapper.Map<VisitorDto>(item);
            return dto;
        }

        public async Task<List<VisitorDto>> Handle(SearchVisitorFuzzyQuery request, CancellationToken cancellationToken)
        {
            List<VisitorDto> result = await context.Visitors
                .OrderByDescending(x => x.Name)
                .Select(x => new VisitorDto()
                {
                    Name = x.Name,
                    CompanyName = x.CompanyName,
                    LicensePlateNumber = x.LicensePlateNumber,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    IdentificationNo = x.IdentificationNo,
                    Gender = x.Gender
                })
                .Distinct()
                .ToListAsync(cancellationToken);
            if (result is null)
                return new List<VisitorDto>();
            return result;
        }
    }
}


