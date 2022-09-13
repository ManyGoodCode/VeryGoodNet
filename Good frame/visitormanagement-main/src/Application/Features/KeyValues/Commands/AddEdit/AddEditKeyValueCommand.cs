using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit
{

    public class AddEditKeyValueCommand : KeyValueDto, IRequest<Result<int>>, ICacheInvalidator
    {
        public string CacheKey => KeyValueCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditKeyValueCommandHandler : IRequestHandler<AddEditKeyValueCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public AddEditKeyValueCommandHandler(
            IApplicationDbContext context,
             IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditKeyValueCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                KeyValue keyValue = await context.KeyValues.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = keyValue ?? throw new NotFoundException($"KeyValue Pair  {request.Id} Not Found.");
                keyValue = mapper.Map(request, keyValue);
                KeyValueChangedEvent changeEvent = new KeyValueChangedEvent(keyValue);
                keyValue.DomainEvents.Add(changeEvent);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(keyValue.Id);
            }
            else
            {
                KeyValue keyValue = mapper.Map<KeyValue>(request);
                KeyValueChangedEvent changeEvent = new KeyValueChangedEvent(keyValue);
                keyValue.DomainEvents.Add(changeEvent);
                context.KeyValues.Add(keyValue);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(keyValue.Id);
            }
        }
    }
}
