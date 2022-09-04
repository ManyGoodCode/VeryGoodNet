// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.Caching;
using MediatR;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.Delete
{

    public class DeleteDocumentTypeCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public CancellationTokenSource SharedExpiryTokenSource => DocumentTypeCacheKey.SharedExpiryTokenSource;
        public DeleteDocumentTypeCommand(int[] id)
        {
            Id = id;
        }
    }


    public class DeleteDocumentTypeCommandHandler : IRequestHandler<DeleteDocumentTypeCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteDocumentTypeCommandHandler(
            IApplicationDbContext context
            )
        {
            _context = context;
        }
        public async Task<Result> Handle(DeleteDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.DocumentTypes.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
                _context.DocumentTypes.Remove(item);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }


    }
}
