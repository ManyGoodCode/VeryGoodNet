using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Import
{
    public class ImportProductsCommand : IRequest<Result>, ICacheInvalidator
    {
        public string CacheKey => ProductCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => ProductCacheKey.SharedExpiryTokenSource();

        public string FileName { get; }
        public byte[] Data { get; }
        public ImportProductsCommand(string fileName, byte[] data)
        {
            FileName = fileName;
            Data = data;
        }
    }

    public class CreateProductsTemplateCommand : IRequest<byte[]>
    {

    }

    public class ImportProductsCommandHandler :
                 IRequestHandler<CreateProductsTemplateCommand, byte[]>,
                 IRequestHandler<ImportProductsCommand, Result>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<ImportProductsCommandHandler> localizer;
        private readonly IExcelService excelService;

        public ImportProductsCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportProductsCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.excelService = excelService;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
        {
            IResult<IEnumerable<ProductDto>> result = await excelService.ImportAsync(
                request.Data,
                mappers: new Dictionary<string, Func<DataRow, ProductDto, object>>
                {
                  { localizer["Brand Name"], (row,item) => item.Brand = row[localizer["Brand Name"]]?.ToString() },
                  { localizer["Product Name"], (row,item) => item.Name = row[localizer["Product Name"]]?.ToString() },
                  { localizer["Description"], (row,item) => item.Description = row[localizer["Description"]]?.ToString() },
                  { localizer["Unit"], (row,item) => item.Unit = row[localizer["Unit"]]?.ToString() },
                  { localizer["Price of unit"], (row,item) => item.Price =row.IsNull(localizer["Price of unit"])? 0m:Convert.ToDecimal(row[localizer["Price of unit"]]) },
                  { localizer["Pictures"], (row,item) => item.Pictures =row.IsNull(localizer["Pictures"])? null:row[localizer["Pictures"]].ToString().Split(",").ToList() },
                },
                localizer["Products"]);

            if (result.Succeeded)
            {
                foreach (ProductDto dto in result.Data)
                {
                    Product item = mapper.Map<Product>(dto);
                    await context.Products.AddAsync(item, cancellationToken);
                }

                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            else
            {
                return Result.Failure(result.Errors);
            }
        }

        public async Task<byte[]> Handle(CreateProductsTemplateCommand request, CancellationToken cancellationToken)
        {
            string[] fields = new string[] { };
            byte[] result = await excelService.CreateTemplateAsync(fields, localizer["Products"]);
            return result;
        }
    }
}

