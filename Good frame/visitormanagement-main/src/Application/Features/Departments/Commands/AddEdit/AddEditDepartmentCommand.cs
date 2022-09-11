// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Departments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Departments.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Application.Common.Models;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Events;

namespace CleanArchitecture.Blazor.Application.Features.Departments.Commands.AddEdit
{
    public class AddEditDepartmentCommand : DepartmentDto, IRequest<Result<int>>, IMapFrom<Department>, ICacheInvalidator
    {
        public string CacheKey => DepartmentCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => DepartmentCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditDepartmentCommandHandler : IRequestHandler<AddEditDepartmentCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditDepartmentCommandHandler> localizer;
        public AddEditDepartmentCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditDepartmentCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(
            AddEditDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                Department item = await context.Departments.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = item ?? throw new NotFoundException("Department {request.Id} Not Found.");
                DepartmentUpdatedEvent updateevent = new DepartmentUpdatedEvent(item);
                item = mapper.Map(request, item);
                item.DomainEvents.Add(updateevent);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                Department item = mapper.Map<Department>(request);
                DepartmentCreatedEvent createevent = new DepartmentCreatedEvent(item);
                item.DomainEvents.Add(createevent);
                context.Departments.Add(item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

