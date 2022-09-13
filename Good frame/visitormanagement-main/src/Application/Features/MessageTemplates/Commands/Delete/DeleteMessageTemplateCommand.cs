using CleanArchitecture.Blazor.Application.Features.MessageTemplates.DTOs;
using CleanArchitecture.Blazor.Application.Features.MessageTemplates.Caching;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;
using AutoMapper;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Domain.Entities;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.Commands.Delete
{
    public class DeleteMessageTemplateCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => MessageTemplateCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => MessageTemplateCacheKey.SharedExpiryTokenSource();
        public DeleteMessageTemplateCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteMessageTemplateCommandHandler :
                 IRequestHandler<DeleteMessageTemplateCommand, Result>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteMessageTemplateCommandHandler> localizer;
        public DeleteMessageTemplateCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteMessageTemplateCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }
        public async Task<Result> Handle(DeleteMessageTemplateCommand request, CancellationToken cancellationToken)
        {

            List<MessageTemplate> items = await context.MessageTemplates.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (MessageTemplate item in items)
            {
                item.DomainEvents.Add(new DeletedEvent<MessageTemplate>(item));
                context.MessageTemplates.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

