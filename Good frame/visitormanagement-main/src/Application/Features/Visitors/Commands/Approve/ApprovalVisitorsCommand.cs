// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ApprovalVisitorsCommandHandler> _localizer;
        public ApprovalVisitorsCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IStringLocalizer<ApprovalVisitorsCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _currentUserService = currentUserService;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(ApprovalVisitorsCommand request, CancellationToken cancellationToken)
        {
            var userName = await _currentUserService.UserName();
            var items = await _context.Visitors.Where(x => request.VisitorId.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
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
                var approval = new ApprovalHistory()
                {
                    Comment = request.Comment,
                    Outcome = request.Outcome,
                    VisitorId = item.Id,
                    ProcessingDate = DateTime.Now,
                    ApprovedBy = userName
                };
                approval.DomainEvents.Add(new CreatedEvent<ApprovalHistory>(approval));
                _context.ApprovalHistories.Add(approval);
                item.DomainEvents.Add(new UpdatedEvent<Visitor>(item));
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(items.Count);
        }
    }
}

