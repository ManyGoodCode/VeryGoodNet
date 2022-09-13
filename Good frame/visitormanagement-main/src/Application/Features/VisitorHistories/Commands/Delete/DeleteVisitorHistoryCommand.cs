using CleanArchitecture.Blazor.Application.Features.VisitorHistories.DTOs;
using CleanArchitecture.Blazor.Application.Features.VisitorHistories.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.VisitorHistories.Commands.Delete
{

    public class DeleteVisitorHistoryCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => VisitorHistoryCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorHistoryCacheKey.SharedExpiryTokenSource();
        public DeleteVisitorHistoryCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteVisitorHistoryCommandHandler :
                 IRequestHandler<DeleteVisitorHistoryCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteVisitorHistoryCommandHandler> localizer;
        public DeleteVisitorHistoryCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteVisitorHistoryCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }
        public async Task<Result> Handle(DeleteVisitorHistoryCommand request, CancellationToken cancellationToken)
        {
            List<VisitorHistory> items = await context.VisitorHistories.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (VisitorHistory item in items)
            {
                context.VisitorHistories.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

