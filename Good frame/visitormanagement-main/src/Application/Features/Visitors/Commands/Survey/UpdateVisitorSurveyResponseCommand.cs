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
using Microsoft.Extensions.Localization;
using AutoMapper;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Update
{
    public class UpdateVisitorSurveyResponseCommand : IRequest<Result>, ICacheInvalidator
    {
        public int Id { get; set; }
        public int? ResponseValue { get; set; }
        public UpdateVisitorSurveyResponseCommand(int id, int? responseValue)
        {
            Id = id;
            ResponseValue = responseValue;
        }

        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class UpdateVisitorSurveyResponseCommandHandler : IRequestHandler<UpdateVisitorSurveyResponseCommand, Result>
    {
        private readonly IApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<UpdateVisitorCommandHandler> localizer;
        public UpdateVisitorSurveyResponseCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IStringLocalizer<UpdateVisitorCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(UpdateVisitorSurveyResponseCommand request, CancellationToken cancellationToken)
        {
            string userName = await currentUserService.UserName();
            Visitor item = await context.Visitors.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item != null)
            {
                ApprovalHistory approval = new ApprovalHistory()
                {
                    Comment = "Customer Survey Response",
                    Outcome = $"Response value: {request.ResponseValue}",
                    VisitorId = item.Id,
                    ProcessingDate = DateTime.Now,
                    ApprovedBy = userName
                };

                approval.DomainEvents.Add(new CreatedEvent<ApprovalHistory>(approval));
                context.ApprovalHistories.Add(approval);
                item.Status = VisitorStatus.Finished;
                item.SurveyResponseValue = request.ResponseValue;
                UpdatedEvent<Visitor> updateevent = new UpdatedEvent<Visitor>(item);
                item.DomainEvents.Add(updateevent);
                await context.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}

