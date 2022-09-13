using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Exceptions;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.AddEdit
{
    public class AddEditVisitorCommand : VisitorDto, IRequest<Result<int>>, ICacheInvalidator
    {
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditVisitorCommandHandler : IRequestHandler<AddEditVisitorCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditVisitorCommandHandler> localizer;
        public AddEditVisitorCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditVisitorCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditVisitorCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                Visitor item = await context.Visitors.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = item ?? throw new NotFoundException("Visitor {request.Id} Not Found.");
                item = mapper.Map(request, item);
                foreach (CompanionDto companiondto in request.Companions)
                {
                    switch (companiondto.TrackingState)
                    {
                        case TrackingState.Added:
                            Companion companionToAdd = mapper.Map<Companion>(companiondto);
                            companionToAdd.VisitorId = item.Id;
                            context.Companions.Add(companionToAdd);
                            break;

                        case TrackingState.Modified:
                            Companion companionToUpdate = await context.Companions.FindAsync(new object[] { companiondto.Id }, cancellationToken);
                            if (companionToUpdate is null) continue;
                            companionToUpdate = mapper.Map(companiondto, companionToUpdate);
                            break;

                        case TrackingState.Deleted:
                            Companion companionToDelete = await context.Companions.FindAsync(new object[] { companiondto.Id }, cancellationToken);
                            if (companionToDelete is null) continue;
                            context.Companions.Remove(companionToDelete);
                            break;
                    }
                }

                item.DomainEvents.Add(new UpdatedEvent<Visitor>(item));
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                Visitor item = mapper.Map<Visitor>(request);
                item.Status = VisitorStatus.PendingApproval;
                foreach (CompanionDto companiondto in request.Companions)
                {
                    Companion companion = mapper.Map<Companion>(companiondto);
                    companion.Visitor = item;
                    companion.VisitorId = item.Id;
                    item.Companions.Add(companion);
                }

                item.DomainEvents.Add(new CreatedEvent<Visitor>(item));
                context.Visitors.Add(item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

