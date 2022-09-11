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
using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Commands.Create
{
    /// <summary>
    /// 请求审批命令
    /// </summary>
    public class CreateApprovalHistoryCommand : ApprovalHistoryDto, IRequest<Result<int>>, ICacheInvalidator
    {
        public string CacheKey => ApprovalHistoryCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => ApprovalHistoryCacheKey.SharedExpiryTokenSource();
    }

    /// <summary>
    /// 1. 将请求审批请求命令映射到实体，并且存储到数据库
    /// 2. 将请求审批请求命令添加到事件通知里
    /// </summary>
    public class CreateApprovalHistoryCommandHandler : IRequestHandler<CreateApprovalHistoryCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly AutoMapper.IMapper mapper;
        private readonly IStringLocalizer<CreateApprovalHistoryCommand> localizer;
        public CreateApprovalHistoryCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateApprovalHistoryCommand> localizer,
            AutoMapper.IMapper mapper
            )
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateApprovalHistoryCommand request, CancellationToken cancellationToken)
        {
            ApprovalHistory item = mapper.Map<ApprovalHistory>(source: request);
            item.DomainEvents.Add(new CreatedEvent<ApprovalHistory>(entity: item));
            context.ApprovalHistories.Add(item);
            await context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(item.Id);
        }
    }
}

