using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.DTOs;
using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Commands.Delete
{
    public class DeleteSiteConfigurationCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => SiteConfigurationCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => SiteConfigurationCacheKey.SharedExpiryTokenSource();
        public DeleteSiteConfigurationCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteSiteConfigurationCommandHandler :
                 IRequestHandler<DeleteSiteConfigurationCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteSiteConfigurationCommandHandler> localizer;
        public DeleteSiteConfigurationCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteSiteConfigurationCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(
            DeleteSiteConfigurationCommand request,
            CancellationToken cancellationToken)
        {
            List<SiteConfiguration> items = await context.SiteConfigurations.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (SiteConfiguration item in items)
            {
                context.SiteConfigurations.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

    }
}

