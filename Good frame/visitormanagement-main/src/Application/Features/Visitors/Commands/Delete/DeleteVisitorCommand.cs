// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Domain.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Delete
{

    public class DeleteVisitorCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
        public DeleteVisitorCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteVisitorCommandHandler :
                 IRequestHandler<DeleteVisitorCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteVisitorCommandHandler> localizer;
        public DeleteVisitorCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteVisitorCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(DeleteVisitorCommand request, CancellationToken cancellationToken)
        {
            List<Visitor> items = await context.Visitors.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (Visitor item in items)
            {
                DeletedEvent<Visitor> deleteevent = new DeletedEvent<Visitor>(item);
                item.DomainEvents.Add(deleteevent);
                List<Companion> companions = await context.Companions.Where(x => x.VisitorId == item.Id).ToListAsync(cancellationToken);
                foreach (Companion companion in companions)
                {
                    context.Companions.Remove(companion);
                }

                List<ApprovalHistory> approvalhistories = await context.ApprovalHistories.Where(x => x.VisitorId == item.Id).ToListAsync(cancellationToken);
                foreach (ApprovalHistory approvalhistory in approvalhistories)
                {
                    context.ApprovalHistories.Remove(approvalhistory);
                }

                context.Visitors.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

