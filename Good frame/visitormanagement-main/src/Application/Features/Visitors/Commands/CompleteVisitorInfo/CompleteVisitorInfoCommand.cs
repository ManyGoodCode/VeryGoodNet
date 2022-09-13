using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Domain.Entities;
using System;
using CleanArchitecture.Blazor.Domain;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;
using AutoMapper;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.CompleteVisitorInfo
{
    public class CompleteVisitorInfoCommand : VisitorDto, IRequest<Result>, IMapFrom<Visitor>, ICacheInvalidator
    {
        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class CompleteVisitorInfoCommandHandler : IRequestHandler<CompleteVisitorInfoCommand, Result>
    {
        private readonly IApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<CompleteVisitorInfoCommandHandler> localizer;
        public CompleteVisitorInfoCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IStringLocalizer<CompleteVisitorInfoCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(CompleteVisitorInfoCommand request, CancellationToken cancellationToken)
        {
            string userName = await currentUserService.UserName();
            Visitor item = await context.Visitors.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item != null)
            {
                item = mapper.Map(request, item);
                item.Status = VisitorStatus.PendingApproval;
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
                            _ = mapper.Map(companiondto, companionToUpdate);
                            break;

                        case TrackingState.Deleted:
                            Companion companionToDelete = await context.Companions.FindAsync(new object[] { companiondto.Id }, cancellationToken);
                            if (companionToDelete is null) continue;
                            context.Companions.Remove(companionToDelete);
                            break;
                    }
                }

                ApprovalHistory approval = new ApprovalHistory()
                {
                    Comment = localizer[VisitorProcess.CompleteInfo],
                    VisitorId = item.Id,
                    ProcessingDate = DateTime.Now,
                    ApprovedBy = userName,
                };

                approval.DomainEvents.Add(new CreatedEvent<ApprovalHistory>(approval));
                context.ApprovalHistories.Add(approval);
                item.DomainEvents.Add(new UpdatedEvent<Visitor>(item));
                await context.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}

