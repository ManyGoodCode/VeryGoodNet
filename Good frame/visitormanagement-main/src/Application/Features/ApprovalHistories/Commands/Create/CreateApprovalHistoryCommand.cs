using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.DTOs;
using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Caching;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Commands.Create
{
    public class CreateApprovalHistoryCommand : ApprovalHistoryDto, IRequest<Result<int>>, ICacheInvalidator
    {
        public string CacheKey => ApprovalHistoryCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => ApprovalHistoryCacheKey.SharedExpiryTokenSource();
    }

    public class CreateApprovalHistoryCommandHandler : IRequestHandler<CreateApprovalHistoryCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateApprovalHistoryCommand> _localizer;
        public CreateApprovalHistoryCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateApprovalHistoryCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateApprovalHistoryCommand request, CancellationToken cancellationToken)
        {
            var item = _mapper.Map<ApprovalHistory>(request);
            item.DomainEvents.Add(new CreatedEvent<ApprovalHistory>(item));
            _context.ApprovalHistories.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(item.Id);
        }
    }
}

