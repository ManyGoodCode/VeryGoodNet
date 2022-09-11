// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Designations.DTOs;
using CleanArchitecture.Blazor.Application.Features.Designations.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Domain.Events;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Designations.Commands.Delete
{

    public class DeleteDesignationCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => DesignationCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => DesignationCacheKey.SharedExpiryTokenSource();
        public DeleteDesignationCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteDesignationCommandHandler :
                 IRequestHandler<DeleteDesignationCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteDesignationCommandHandler> localizer;
        public DeleteDesignationCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteDesignationCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(DeleteDesignationCommand request, CancellationToken cancellationToken)
        {

            List<Designation> items = await context.Designations.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (Designation item in items)
            {
                DesignationDeletedEvent deleteevent = new DesignationDeletedEvent(item);
                item.DomainEvents.Add(deleteevent);
                context.Designations.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

