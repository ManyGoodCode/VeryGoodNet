using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using MediatR;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.Delete
{

    public class DeleteDocumentCommand : IRequest<Result>, ICacheInvalidator
    {
        public CancellationTokenSource? SharedExpiryTokenSource => DocumentCacheKey.SharedExpiryTokenSource;
        public int[] Id { get; set; }
        public DeleteDocumentCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Result>

    {
        private readonly IApplicationDbContext context;

        public DeleteDocumentCommandHandler(
            IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            List<Document> items = await context.Documents.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
                context.Documents.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
