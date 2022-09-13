using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Create
{

    public class CreateVisitorCommand : VisitorDto, IRequest<Result<int>>, IMapFrom<Visitor>, ICacheInvalidator
    {
        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class CreateVisitorCommandHandler : IRequestHandler<CreateVisitorCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<CreateVisitorCommand> localizer;
        public CreateVisitorCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateVisitorCommand> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
        {
            Visitor item = mapper.Map<Visitor>(request);
            item.Status = VisitorStatus.PendingCheckin;
            foreach (CompanionDto companionDto in request.Companions)
            {
                Companion companion = mapper.Map<Companion>(companionDto);
                companion.VisitorId = item.Id;
                companion.Visitor = item;
                item.Companions.Add(companion);
            }

            CreatedEvent<Visitor> createevent = new CreatedEvent<Visitor>(item);
            item.DomainEvents.Add(createevent);
            context.Visitors.Add(item);
            await context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(item.Id);
        }
    }
}

