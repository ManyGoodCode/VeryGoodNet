using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit
{
    public class AddEditProductCommand : ProductDto, IRequest<Result<int>>, IMapFrom<Product>, ICacheInvalidator
    {
        public string CacheKey => ProductCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => ProductCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditProductCommandHandler : IRequestHandler<AddEditProductCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        public AddEditProductCommandHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditProductCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                Product item = await context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = item ?? throw new NotFoundException($"Product {request.Id} Not Found.");
                item = mapper.Map(request, item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                Product item = mapper.Map<Product>(request);
                context.Products.Add(item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

