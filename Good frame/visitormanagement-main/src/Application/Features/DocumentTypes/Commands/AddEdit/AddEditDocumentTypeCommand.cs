using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.Caching;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.AddEdit
{
    public class AddEditDocumentTypeCommand : DocumentTypeDto, IRequest<Result<int>>, ICacheInvalidator
    {
        public CancellationTokenSource? SharedExpiryTokenSource => DocumentTypeCacheKey.SharedExpiryTokenSource;
    }

    public class AddEditDocumentTypeCommandHandler : IRequestHandler<AddEditDocumentTypeCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public AddEditDocumentTypeCommandHandler(
            IApplicationDbContext context,
             IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                DocumentType documentType = await context.DocumentTypes.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = documentType ?? throw new NotFoundException($"Document Type {request.Id} Not Found.");
                documentType = mapper.Map(request, documentType);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(documentType.Id);
            }
            else
            {
                DocumentType documentType = mapper.Map<DocumentType>(request);
                context.DocumentTypes.Add(documentType);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(documentType.Id);
            }
        }
    }
}
