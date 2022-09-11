using CleanArchitecture.Blazor.Application.Features.CheckinPoints.DTOs;
using CleanArchitecture.Blazor.Application.Features.CheckinPoints.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.Commands.AddEdit
{

    public class AddEditCheckinPointCommand : CheckinPointDto, IRequest<Result<int>>, IMapFrom<CheckinPoint>, ICacheInvalidator
    {
        public string CacheKey => CheckinPointCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => CheckinPointCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditCheckinPointCommandHandler : IRequestHandler<AddEditCheckinPointCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditCheckinPointCommandHandler> localizer;
        public AddEditCheckinPointCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditCheckinPointCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(AddEditCheckinPointCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                CheckinPoint item = await context.CheckinPoints.FindAsync(new object[]
                {
                    request.Id
                }, cancellationToken);
                _ = item ?? throw new NotFoundException("CheckinPoint {request.Id} Not Found.");
                item = mapper.Map(request, item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                CheckinPoint item = mapper.Map<CheckinPoint>(request);
                context.CheckinPoints.Add(item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }

        }
    }
}

