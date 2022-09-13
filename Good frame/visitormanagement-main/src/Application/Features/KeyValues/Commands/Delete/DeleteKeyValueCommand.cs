// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Delete
{
    public class DeleteKeyValueCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => KeyValueCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.SharedExpiryTokenSource();
        public DeleteKeyValueCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteKeyValueCommandHandler : IRequestHandler<DeleteKeyValueCommand, Result>
    {
        private readonly IApplicationDbContext context;

        public DeleteKeyValueCommandHandler(
            IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result> Handle(DeleteKeyValueCommand request, CancellationToken cancellationToken)
        {
            List<KeyValue> items = await context.KeyValues.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (KeyValue item in items)
            {
                KeyValueChangedEvent changeEvent = new KeyValueChangedEvent(item);
                item.DomainEvents.Add(changeEvent);
                context.KeyValues.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
