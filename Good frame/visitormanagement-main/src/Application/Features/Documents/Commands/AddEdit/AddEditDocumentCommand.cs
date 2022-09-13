// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.AddEdit
{

    public class AddEditDocumentCommand : DocumentDto, IRequest<Result<int>>, ICacheInvalidator
    {
        public CancellationTokenSource? SharedExpiryTokenSource => DocumentCacheKey.SharedExpiryTokenSource;
        public UploadRequest? UploadRequest { get; set; }

    }

    public class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IUploadService uploadService;

        public AddEditDocumentCommandHandler(
            IApplicationDbContext context,
             IMapper mapper,
             IUploadService uploadService)
        {
            this.context = context;
            this.mapper = mapper;
            this.uploadService = uploadService;
        }
        public async Task<Result<int>> Handle(AddEditDocumentCommand request, CancellationToken cancellationToken)
        {

            if (request.Id > 0)
            {
                Document document = await context.Documents.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = document ?? throw new NotFoundException($"Document {request.Id} Not Found.");
                if (request.UploadRequest != null)
                {
                    document.URL = await uploadService.UploadAsync(request.UploadRequest);
                }

                document.Title = request.Title;
                document.Description = request.Description;
                document.IsPublic = request.IsPublic;
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(document.Id);
            }
            else
            {
                Document document = mapper.Map<Document>(request);
                if (request.UploadRequest != null)
                {
                    document.URL = await uploadService.UploadAsync(request.UploadRequest); ;
                }

                DocumentCreatedEvent createdevent = new DocumentCreatedEvent(document);
                document.DomainEvents.Add(createdevent);
                context.Documents.Add(document);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(document.Id);
            }
        }
    }
}
