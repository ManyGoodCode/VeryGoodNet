using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using System;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Approve
{
    public class ConfirmVisitorCommand : IRequest<Result<int>>, ICacheInvalidator
    {
        public int[] VisitorId { get; private set; }

        public ConfirmVisitorCommand(int[] visitorId)
        {
            VisitorId = visitorId;
        }
        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class ConfirmVisitorCommandCommandHandler : IRequestHandler<ConfirmVisitorCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<ConfirmVisitorCommandCommandHandler> localizer;
        public ConfirmVisitorCommandCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IStringLocalizer<ConfirmVisitorCommandCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(ConfirmVisitorCommand request, CancellationToken cancellationToken)
        {
            string userName = await currentUserService.UserName();
            List<Visitor> items = await context.Visitors.Where(x => request.VisitorId.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (Visitor item in items)
            {
                item.Status = VisitorStatus.PendingCheckout;
                ApprovalHistory approval = new ApprovalHistory()
                {
                    Comment = localizer[VisitorProcess.Confirm],
                    VisitorId = item.Id,
                    ProcessingDate = DateTime.Now,
                    ApprovedBy = userName
                };

                approval.DomainEvents.Add(new CreatedEvent<ApprovalHistory>(approval));
                context.ApprovalHistories.Add(approval);
                item.DomainEvents.Add(new UpdatedEvent<Visitor>(item));
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(items.Count);
        }
    }
}

