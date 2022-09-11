// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.CheckinPoints.DTOs;
using CleanArchitecture.Blazor.Application.Features.CheckinPoints.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.Commands.Delete
{
    public class DeleteCheckinPointCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => CheckinPointCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => CheckinPointCacheKey.SharedExpiryTokenSource();
        public DeleteCheckinPointCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteCheckinPointCommandHandler :
                 IRequestHandler<DeleteCheckinPointCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteCheckinPointCommandHandler> localizer;
        public DeleteCheckinPointCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteCheckinPointCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(DeleteCheckinPointCommand request, CancellationToken cancellationToken)
        {
            List<CheckinPoint> items = await context.CheckinPoints.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (CheckinPoint item in items)
            {
                context.CheckinPoints.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

    }
}

