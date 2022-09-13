using CleanArchitecture.Blazor.Application.Features.Sites.DTOs;
using CleanArchitecture.Blazor.Application.Features.Sites.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Domain.Events;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Sites.Commands.Delete
{
    public class DeleteSiteCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => SiteCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => SiteCacheKey.SharedExpiryTokenSource();
        public DeleteSiteCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteSiteCommandHandler :
                 IRequestHandler<DeleteSiteCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteSiteCommandHandler> localizer;
        public DeleteSiteCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteSiteCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
        {
            List<Site> items = await context.Sites.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (Site item in items)
            {
                SiteDeletedEvent deleteevent = new SiteDeletedEvent(item);
                item.DomainEvents.Add(deleteevent);
                context.Sites.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

