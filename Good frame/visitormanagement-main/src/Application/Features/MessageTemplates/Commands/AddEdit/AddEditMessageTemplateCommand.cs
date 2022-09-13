// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.MessageTemplates.DTOs;
using CleanArchitecture.Blazor.Application.Features.MessageTemplates.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Events;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Application.Common.Exceptions;

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.Commands.AddEdit
{

    public class AddEditMessageTemplateCommand : MessageTemplateDto, IRequest<Result<int>>, IMapFrom<MessageTemplate>, ICacheInvalidator
    {
        public string CacheKey => MessageTemplateCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => MessageTemplateCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditMessageTemplateCommandHandler : IRequestHandler<AddEditMessageTemplateCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditMessageTemplateCommandHandler> localizer;
        public AddEditMessageTemplateCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditMessageTemplateCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditMessageTemplateCommand request, CancellationToken cancellationToken)
        {

            if (request.Id > 0)
            {
                MessageTemplate item = await context.MessageTemplates.FindAsync(new object[] { request.Id }, cancellationToken) ??
                    throw new NotFoundException("MessageTemplate {request.Id} Not Found.");
                item = mapper.Map(request, item);
                item.DomainEvents.Add(new UpdatedEvent<MessageTemplate>(item));
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                MessageTemplate item = mapper.Map<MessageTemplate>(request);
                item.DomainEvents.Add(new CreatedEvent<MessageTemplate>(item));
                context.MessageTemplates.Add(item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

