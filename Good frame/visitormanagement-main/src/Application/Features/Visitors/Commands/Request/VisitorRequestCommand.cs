using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using System;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Create
{
    public class VisitorRequestCommand : VisitorDto, IRequest<Result<int>>, ICacheInvalidator
    {
        public VisitorDto? Visitor { get; set; }
        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class VisitorRequestCommandHandler : IRequestHandler<VisitorRequestCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<VisitorRequestCommandHandler> localizer;
        public VisitorRequestCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IStringLocalizer<VisitorRequestCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(VisitorRequestCommand request, CancellationToken cancellationToken)
        {
            string userName = await currentUserService.UserName();
            Visitor item = mapper.Map<Visitor>(request);
            item.Status = VisitorStatus.PendingVisitor;
            foreach (CompanionDto companionDto in request.Companions)
            {
                Companion companion = mapper.Map<Companion>(companionDto);
                companion.VisitorId = item.Id;
                companion.Visitor = item;
                item.Companions.Add(companion);
            }

            ApprovalHistory approval = new ApprovalHistory()
            {
                Comment = localizer[VisitorProcess.Request],
                VisitorId = item.Id,
                Visitor = item,
                ProcessingDate = DateTime.Now,
                ApprovedBy = userName
            };

            item.DomainEvents.Add(new CreatedEvent<Visitor>(item));
            context.Visitors.Add(item);
            approval.DomainEvents.Add(new CreatedEvent<ApprovalHistory>(approval));
            context.ApprovalHistories.Add(approval);
            await context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(item.Id);
        }
    }
}

