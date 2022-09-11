// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Designations.DTOs;
using CleanArchitecture.Blazor.Application.Features.Designations.Caching;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Events;

namespace CleanArchitecture.Blazor.Application.Features.Designations.Commands.AddEdit
{

    public class AddEditDesignationCommand : DesignationDto, IRequest<Result<int>>, IMapFrom<Designation>, ICacheInvalidator
    {
        public string CacheKey => DesignationCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => DesignationCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditDesignationCommandHandler : IRequestHandler<AddEditDesignationCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditDesignationCommandHandler> localizer;
        public AddEditDesignationCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditDesignationCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(
            AddEditDesignationCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                Designation item = await context.Designations.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = item ?? throw new NotFoundException("Designation {request.Id} Not Found.");
                item = mapper.Map(request, item);
                DesignationUpdatedEvent updateevent = new DesignationUpdatedEvent(item);
                item.DomainEvents.Add(updateevent);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                Designation item = mapper.Map<Designation>(request);
                context.Designations.Add(item);
                var careateevent = new DesignationCreatedEvent(item);
                item.DomainEvents.Add(careateevent);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

