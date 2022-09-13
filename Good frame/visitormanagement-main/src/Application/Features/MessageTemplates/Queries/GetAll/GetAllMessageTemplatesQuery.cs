// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.MessageTemplates.DTOs;
using CleanArchitecture.Blazor.Application.Features.MessageTemplates.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.Queries.GetAll
{
    public class GetAllMessageTemplatesQuery : IRequest<IEnumerable<MessageTemplateDto>>, ICacheable
    {
        public string CacheKey => MessageTemplateCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => MessageTemplateCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllMessageTemplatesQueryHandler :
         IRequestHandler<GetAllMessageTemplatesQuery, IEnumerable<MessageTemplateDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllMessageTemplatesQueryHandler> localizer;

        public GetAllMessageTemplatesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllMessageTemplatesQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<MessageTemplateDto>> Handle(GetAllMessageTemplatesQuery request, CancellationToken cancellationToken)
        {

            IEnumerable<MessageTemplateDto> data = await context.MessageTemplates.OrderBy(x => x.SiteId)
                         .ProjectTo<MessageTemplateDto>(mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }

}

