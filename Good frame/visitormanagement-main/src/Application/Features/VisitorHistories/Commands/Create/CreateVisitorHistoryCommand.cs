using CleanArchitecture.Blazor.Application.Features.VisitorHistories.DTOs;
using CleanArchitecture.Blazor.Application.Features.VisitorHistories.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using CleanArchitecture.Blazor.Application.Features.VisitorHistories.Constants;
using System.Collections.Generic;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.VisitorHistories.Commands.Create
{
    public class CreateVisitorHistoryCommand : VisitorHistoryDto, IRequest<Result<int>>, ICacheInvalidator
    {
        public int? CompanionCount { get; set; }
        public int[]? CheckinCompanion { get; set; }
        public string? QrCode { get; set; }
        public int? SiteId { get; set; }
        public string? CurrentStatus { get; set; }
        public List<VisitorHistoryDto> Histories { get; set; } = new List<VisitorHistoryDto>();
        public string CacheKey => VisitorHistoryCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorHistoryCacheKey.SharedExpiryTokenSource();
    }

    public class CreateVisitorHistoryCommandHandler : IRequestHandler<CreateVisitorHistoryCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<CreateVisitorHistoryCommand> localizer;
        private readonly ILogger<CreateVisitorHistoryCommandHandler> logger;

        public CreateVisitorHistoryCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateVisitorHistoryCommand> localizer,
            ILogger<CreateVisitorHistoryCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(
            CreateVisitorHistoryCommand request,
            CancellationToken cancellationToken)
        {
            VisitorHistory item = mapper.Map<VisitorHistory>(request);
            context.VisitorHistories.Add(item);
            Visitor visitor = await context.Visitors.FirstAsync(x => x.Id == request.VisitorId);
            visitor.DomainEvents.Add(new UpdatedEvent<Visitor>(visitor));
            if (item.Stage == CheckStage.Checkin)
            {
                if (visitor.CheckinDate is null)
                {
                    visitor.CheckinDate = item.TransitDateTime;
                    if (visitor.Status != VisitorStatus.PendingCheckout)
                    {
                        if (!request.Companions.Any(x => x.Checked == false && x.CheckinDateTime is null))
                        {
                            visitor.Status = VisitorStatus.PendingConfirm;
                        }
                    }
                }
                else
                {
                    if (visitor.Status != VisitorStatus.PendingCheckout)
                    {
                        if (!request.Companions.Any(x => x.Checked == false && x.CheckinDateTime is null))
                        {
                            visitor.Status = VisitorStatus.PendingConfirm;
                        }
                    }
                }
            }
            else if (item.Stage == CheckStage.Checkout)
            {
                if (visitor.CheckoutDate is null)
                {
                    visitor.CheckoutDate = item.TransitDateTime;
                    if (!request.Companions.Any(x => x.Checked == false && x.CheckoutDateTime is null))
                    {
                        visitor.Status = VisitorStatus.PendingFeedback;
                    }
                }
            }

            int[] companionId = request.Companions.Where(x => x.Checked).Select(x => x.Id).ToArray();
            if (companionId != null && companionId.Any())
            {
                List<Companion> companions = context.Companions.Where(x => companionId.Contains(x.Id)).ToList();
                foreach (Companion comp in companions)
                {
                    if (item.Stage == CheckStage.Checkin)
                    {
                        if (comp.CheckinDateTime is null)
                        {
                            comp.CheckinDateTime = item.TransitDateTime;
                        }
                    }
                    else
                    {
                        if (comp.CheckoutDateTime is null)
                        {
                            comp.CheckoutDateTime = item.TransitDateTime;
                        }
                    }

                    context.Companions.Update(comp);
                }
            }

            VisitorCacheKey.SharedExpiryTokenSource().Cancel();
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("{handler}: {Stage}: {TransitDateTime}", nameof(CreateVisitorHistoryCommandHandler), item.Stage, item.TransitDateTime);
            return Result<int>.Success(item.Id);
        }
    }
}

