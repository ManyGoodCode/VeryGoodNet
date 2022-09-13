// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Employees.DTOs;
using CleanArchitecture.Blazor.Application.Features.Employees.Caching;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Events;

namespace CleanArchitecture.Blazor.Application.Features.Employees.Commands.AddEdit
{

    public class AddEditEmployeeCommand : EmployeeDto, IRequest<Result<int>>, IMapFrom<Employee>, ICacheInvalidator
    {
        public string CacheKey => EmployeeCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => EmployeeCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditEmployeeCommandHandler : IRequestHandler<AddEditEmployeeCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditEmployeeCommandHandler> localizer;
        public AddEditEmployeeCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditEmployeeCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditEmployeeCommand request, CancellationToken cancellationToken)
        {

            if (request.Id > 0)
            {
                Employee item = await context.Employees.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = item ?? throw new NotFoundException("Employee {request.Id} Not Found.");
                item = mapper.Map(request, item);
                EmployeeUpdatedEvent updateevent = new EmployeeUpdatedEvent(item);
                item.DomainEvents.Add(updateevent);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                Employee item = mapper.Map<Employee>(request);
                context.Employees.Add(item);
                EmployeeCreatedEvent createevent = new EmployeeCreatedEvent(item);
                item.DomainEvents.Add(createevent);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

