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

    public class MappingTests
    {
        private readonly IConfigurationProvider configuration;
        private readonly IMapper mapper;

        public MappingTests()
        {
            configuration = new MapperConfiguration(cfg =>
            {
            //cfg.Advanced.AllowAdditiveTypeMapCreation = true;
            cfg.AddProfile<MappingProfile>();
            });

            mapper = configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            configuration.AssertConfigurationIsValid();
        }

        [Test]
        [TestCase(typeof(DocumentType), typeof(DocumentTypeDto))]
        [TestCase(typeof(Document), typeof(DocumentDto))]
        [TestCase(typeof(KeyValue), typeof(KeyValueDto))]
        [TestCase(typeof(Product), typeof(ProductDto))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            object instance = GetInstanceOf(source);
            mapper.Map(instance, source, destination);
        }

        private object GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type);
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}

