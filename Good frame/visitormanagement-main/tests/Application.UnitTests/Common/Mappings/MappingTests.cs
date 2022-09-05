using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;

using NUnit.Framework;
using System;
using System.Runtime.Serialization;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Mappings
{
    /// <summary>
    ///  测试Map功能
    ///  实现  Entities 与 Dto 数据之间的映射
    ///  TestCase 特性填充测试数据
    /// </summary>
    public class MappingTests
    {
        private readonly IConfigurationProvider configuration;
        private readonly IMapper mapper;

        public MappingTests()
        {
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            mapper = configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            configuration.AssertConfigurationIsValid();
        }

        /// <summary>
        /// TestCase 特性填充测试数据
        /// </summary>
        [Test]
        [TestCase(typeof(DocumentType), typeof(DocumentTypeDto))]
        [TestCase(typeof(Document), typeof(DocumentDto))]
        [TestCase(typeof(KeyValue), typeof(KeyValueDto))]
        [TestCase(typeof(Product), typeof(ProductDto))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            object instance = GetInstanceOf(source);
            object result = mapper.Map(instance, source, destination);
        }

        private object GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type);
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}

