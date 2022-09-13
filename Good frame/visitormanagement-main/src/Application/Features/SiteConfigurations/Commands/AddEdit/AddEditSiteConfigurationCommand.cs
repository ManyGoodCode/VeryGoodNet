using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.DTOs;
using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using Microsoft.Extensions.Localization;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Exceptions;

namespace CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Commands.AddEdit
{
    public class AddEditSiteConfigurationCommand : SiteConfigurationDto, IRequest<Result<int>>, IMapFrom<SiteConfiguration>, ICacheInvalidator
    {
        public string CacheKey => SiteConfigurationCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => SiteConfigurationCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditSiteConfigurationCommandHandler : IRequestHandler<AddEditSiteConfigurationCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditSiteConfigurationCommandHandler> localizer;
        public AddEditSiteConfigurationCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditSiteConfigurationCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result<int>> Handle(
            AddEditSiteConfigurationCommand request,
            CancellationToken cancellationToken)
        {

            if (request.Id > 0)
            {
                SiteConfiguration item = await context.SiteConfigurations.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException("SiteConfiguration {request.Id} Not Found.");
                item = mapper.Map(request, item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                SiteConfiguration item = mapper.Map<SiteConfiguration>(request);
                context.SiteConfigurations.Add(item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

