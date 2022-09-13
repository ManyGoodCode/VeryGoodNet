using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Common.Specification;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.PaginationQuery
{
    public class DocumentsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<DocumentDto>>, ICacheable
    {
        public string CacheKey => $"{nameof(DocumentsWithPaginationQuery)},{this}";
        public MemoryCacheEntryOptions? Options => DocumentCacheKey.MemoryCacheEntryOptions;
    }

    public class DocumentsQueryHandler : IRequestHandler<DocumentsWithPaginationQuery, PaginatedData<DocumentDto>>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public DocumentsQueryHandler(
            ICurrentUserService currentUserService,
            IApplicationDbContext context,
            IMapper mapper)
        {
            this.currentUserService = currentUserService;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PaginatedData<DocumentDto>> Handle(
            DocumentsWithPaginationQuery request,
            CancellationToken cancellationToken)
        {

            PaginatedData<DocumentDto> data = await context.Documents
                .Specify(new DocumentsQuery(await currentUserService.UserId()))
                .Where(x => x.Description.Contains(request.Keyword))
                //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectTo<DocumentDto>(mapper.ConfigurationProvider)
                .PaginatedDataAsync(pageNumber: request.PageNumber, pageSize: request.PageSize);

            return data;
        }

        internal class DocumentsQuery : Specification<Document>
        {
            public DocumentsQuery(string userId)
            {
                this.AddInclude(x => x.DocumentType);
                this.Criteria = p => (p.CreatedBy == userId && p.IsPublic == false) || p.IsPublic == true;
            }
        }
    }
}