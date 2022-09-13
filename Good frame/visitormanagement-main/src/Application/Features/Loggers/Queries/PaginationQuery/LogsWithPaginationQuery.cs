using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.Logs.DTOs;
using CleanArchitecture.Blazor.Domain.Entities.Log;
using MediatR;

namespace CleanArchitecture.Blazor.Application.Logs.Queries.PaginationQuery
{
    public class LogsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<LogDto>>
    {

    }

    public class LogsQueryHandler : IRequestHandler<LogsWithPaginationQuery, PaginatedData<LogDto>>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public LogsQueryHandler(
            ICurrentUserService currentUserService,
            IApplicationDbContext context,
            IMapper mapper)
        {
            this.currentUserService = currentUserService;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PaginatedData<LogDto>> Handle(LogsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<LogDto> data = await context.Loggers
                .Where(x => x.Message.Contains(request.Keyword) || x.Exception.Contains(request.Keyword))
                //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                    .ProjectTo<LogDto>(mapper.ConfigurationProvider)
                    .PaginatedDataAsync(request.PageNumber, request.PageSize);

            return data;
        }
    }
}
