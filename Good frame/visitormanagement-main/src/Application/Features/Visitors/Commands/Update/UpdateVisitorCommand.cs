using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Entities;
using System.Threading;
using Microsoft.Extensions.Localization;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Update
{

    public class UpdateVisitorCommand : VisitorDto, IRequest<Result>, IMapFrom<Visitor>, ICacheInvalidator
    {
        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class UpdateVisitorCommandHandler : IRequestHandler<UpdateVisitorCommand, Result>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<UpdateVisitorCommandHandler> localizer;
        public UpdateVisitorCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<UpdateVisitorCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
        {
            Visitor item = await context.Visitors.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item != null)
            {
                item = mapper.Map(request, item);
                UpdatedEvent<Visitor> updateevent = new UpdatedEvent<Visitor>(item);
                item.DomainEvents.Add(updateevent);
                await context.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}

