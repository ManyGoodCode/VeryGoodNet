// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.MessageTemplates.DTOs;
using CleanArchitecture.Blazor.Application.Features.MessageTemplates.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.Queries.Pagination
{

    public class MessageTemplatesWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<MessageTemplateDto>>, ICacheable
    {
        public string CacheKey => MessageTemplateCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => MessageTemplateCacheKey.MemoryCacheEntryOptions;
    }

    public class MessageTemplatesWithPaginationQueryHandler :
         IRequestHandler<MessageTemplatesWithPaginationQuery, PaginatedData<MessageTemplateDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<MessageTemplatesWithPaginationQueryHandler> localizer;

        public MessageTemplatesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<MessageTemplatesWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<MessageTemplateDto>> Handle(MessageTemplatesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<MessageTemplateDto> data = await context.MessageTemplates.Where(x =>
                              x.Subject.Contains(request.Keyword) ||
                              x.Body.Contains(request.Keyword) ||
                              x.Description.Contains(request.Keyword))
                    //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                    .ProjectTo<MessageTemplateDto>(mapper.ConfigurationProvider)
                    .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}