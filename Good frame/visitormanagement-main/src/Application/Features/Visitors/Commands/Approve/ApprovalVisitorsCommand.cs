using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Approve
{
    public class ApprovalVisitorsCommand : IRequest<Result<int>>, ICacheInvalidator
    {
        public int[] VisitorId { get; private set; }
        public string Outcome { get; private set; }
        public string? Comment { get; private set; }
        public ApprovalVisitorsCommand(string outcome, int[] visitorId, string? comment = null)
        {
            Outcome = outcome;
            VisitorId = visitorId;
            Comment = comment;
        }
        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class ApprovalVisitorsCommandHandler : IRequestHandler<ApprovalVisitorsCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<ApprovalVisitorsCommandHandler> localizer;
        public ApprovalVisitorsCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IStringLocalizer<ApprovalVisitorsCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(ApprovalVisitorsCommand request, CancellationToken cancellationToken)
        {
            string userName = await currentUserService.UserName();
            List<Visitor> items = await context.Visitors.Where(x => request.VisitorId.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (Visitor item in items)
            {
                item.ApprovalOutcome = request.Outcome;
                item.ApprovalComment = request.Comment;
                item.Apppoved = true;
                if (item.ApprovalOutcome == ApprovalOutcome.Approved)
                {
                    item.Status = VisitorStatus.PendingChecking;
                }
                else
                {
                    item.Status = VisitorStatus.Canceled;
                }
                ApprovalHistory approval = new ApprovalHistory()
                {
                    Comment = request.Comment,
                    Outcome = request.Outcome,
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

