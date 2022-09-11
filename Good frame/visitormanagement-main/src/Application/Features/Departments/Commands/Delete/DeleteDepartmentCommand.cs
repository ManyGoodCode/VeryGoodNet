// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Departments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Departments.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using MediatR;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Departments.Commands.Delete
{
    public class DeleteDepartmentCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => DepartmentCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => DepartmentCacheKey.SharedExpiryTokenSource();
        public DeleteDepartmentCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteDepartmentCommandHandler :
                 IRequestHandler<DeleteDepartmentCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteDepartmentCommandHandler> localizer;
        public DeleteDepartmentCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteDepartmentCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            List<Department> items = await context.Departments.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (Department item in items)
            {
                DepartmentDeletedEvent deleteevent = new DepartmentDeletedEvent(item);
                item.DomainEvents.Add(deleteevent);
                context.Departments.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

