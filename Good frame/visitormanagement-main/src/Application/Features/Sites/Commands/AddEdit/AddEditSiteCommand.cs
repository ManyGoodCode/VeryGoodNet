using CleanArchitecture.Blazor.Application.Features.Sites.DTOs;
using CleanArchitecture.Blazor.Application.Features.Sites.Caching;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Sites.Commands.AddEdit
{
    public class AddEditSiteCommand : SiteDto, IRequest<Result<int>>, IMapFrom<Site>, ICacheInvalidator
    {
        public string CacheKey => SiteCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => SiteCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditSiteCommandHandler : IRequestHandler<AddEditSiteCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditSiteCommandHandler> localizer;
        public AddEditSiteCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditSiteCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditSiteCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                Site item = await context.Sites.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = item ?? throw new NotFoundException($"Site {request.Id} Not Found.");
                item = mapper.Map(request, item);
                SiteUpdatedEvent updateevent = new SiteUpdatedEvent(item);
                item.DomainEvents.Add(updateevent);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                Site item = mapper.Map<Site>(request);
                SiteCreatedEvent createevent = new SiteCreatedEvent(item);
                item.DomainEvents.Add(createevent);
                context.Sites.Add(item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

