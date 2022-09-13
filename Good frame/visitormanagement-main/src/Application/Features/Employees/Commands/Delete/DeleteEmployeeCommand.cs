// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Employees.DTOs;
using CleanArchitecture.Blazor.Application.Features.Employees.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using CleanArchitecture.Blazor.Domain.Events;
using Microsoft.Extensions.Localization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Employees.Commands.Delete
{

    public class DeleteEmployeeCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => EmployeeCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => EmployeeCacheKey.SharedExpiryTokenSource();
        public DeleteEmployeeCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteEmployeeCommandHandler :
                 IRequestHandler<DeleteEmployeeCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteEmployeeCommandHandler> localizer;
        public DeleteEmployeeCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteEmployeeCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }
        public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {

            List<Employee> items = await context.Employees.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (Employee item in items)
            {
                EmployeeDeletedEvent deleteevent = new EmployeeDeletedEvent(item);
                item.DomainEvents.Add(deleteevent);
                context.Employees.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
