using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using System;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Specification;
using CleanArchitecture.Blazor.Domain.Entities;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.Pagination
{

    public class VisitorsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<VisitorDto>>, ICacheable
    {
        public string? PassCode { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? LicensePlateNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? Purpose { get; set; }
        public string? Employee { get; set; }
        public DateTime? ExpectedDate1 { get; set; }
        public DateTime? ExpectedDate2 { get; set; }
        public string? Outcome { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()},Name:{Name},LicensePlateNumber:{LicensePlateNumber},CompanyName:{CompanyName},Purpose:{Purpose},Employee:{Employee},ExpectedDate1:{ExpectedDate1?.ToString()},ExpectedDate2:{ExpectedDate2?.ToString()},Outcome:{Outcome},Status:{Status}";
        }
        public string CacheKey => VisitorCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }

    public class VisitorsWithPaginationQueryHandler :
         IRequestHandler<VisitorsWithPaginationQuery, PaginatedData<VisitorDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<VisitorsWithPaginationQueryHandler> localizer;

        public VisitorsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<VisitorsWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<VisitorDto>> Handle(VisitorsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<VisitorDto> data = await context.Visitors.Specify(new SearchVisitorSpecification(request))
              //.OrderBy($"{request.OrderBy} {request.SortDirection}")
              .ProjectTo<VisitorDto>(mapper.ConfigurationProvider)
              .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }

    public class SearchVisitorSpecification : Specification<Visitor>
    {
        public SearchVisitorSpecification(VisitorsWithPaginationQuery query)
        {
            AddInclude(x => x.Employee);
            AddInclude(x => x.Designation);
            AddInclude(x => x.Companions);
            Criteria = q => q.Name != null;
            if (!string.IsNullOrEmpty(query.PassCode))
            {
                And(x => x.PassCode.Contains(query.PassCode));
            }
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                And(x => x.Name.Contains(query.Keyword) || x.Comment.Contains(query.Keyword) || x.CompanyName.Contains(query.Keyword));
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                And(x => x.Status == query.Status);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                And(x => x.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.LicensePlateNumber))
            {
                And(x => x.LicensePlateNumber.Contains(query.LicensePlateNumber));
            }
            if (!string.IsNullOrEmpty(query.CompanyName))
            {
                And(x => x.CompanyName.Contains(query.CompanyName));
            }
            if (!string.IsNullOrEmpty(query.Employee))
            {
                And(x => x.Employee.Name.Contains(query.Employee));
            }
            if (!string.IsNullOrEmpty(query.Purpose))
            {
                And(x => x.Purpose == query.Purpose);
            }
            if (query.Outcome != null)
            {
                And(x => x.ApprovalOutcome.Contains(query.Outcome));
            }
            if (query.ExpectedDate1 != null && query.ExpectedDate2 != null)
            {
                And(x => x.ExpectedDate >= query.ExpectedDate1 && x.ExpectedDate < query.ExpectedDate2.Value.AddDays(1));
            }
        }
    }
}