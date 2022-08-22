// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Examples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExampleLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Enumerates all examples in the assembly.
    /// </summary>
    public static class Examples
    {
        /// <summary>
        /// 获取 用 Type 分类类型下【递归父类也算】
        /// 所有用  ExampleAttribute 修饰的 方法类型。返回 ExampleInfo集合
        /// </summary>
        public static IEnumerable<ExampleInfo> GetCategory(Type categoryType)
        {
            TypeInfo typeInfo = categoryType.GetTypeInfo();
            ExamplesAttribute typeExamplesAttribute = typeInfo.GetCustomAttributes<ExamplesAttribute>().FirstOrDefault();
            TagsAttribute typeExamplesTagsAttribute = typeInfo.GetCustomAttributes<TagsAttribute>().FirstOrDefault() ?? new TagsAttribute();

            List<Type> types = new List<Type>();
            TypeInfo baseType = typeInfo;
            while (baseType != null)
            {
                types.Add(baseType.AsType());
                baseType = baseType.BaseType?.GetTypeInfo();
            }

            foreach (Type t in types)
            {
                foreach (MethodInfo method in t.GetRuntimeMethods())
                {
                    ExampleAttribute methodExampleAttribute = method.GetCustomAttributes<ExampleAttribute>().FirstOrDefault();
                    if (methodExampleAttribute != null)
                    {
                        TagsAttribute methodExampleTags = method.GetCustomAttributes<TagsAttribute>().FirstOrDefault() ?? new TagsAttribute();
                        List<string> tags = new List<string>(typeExamplesTagsAttribute.Tags);
                        tags.AddRange(methodExampleTags.Tags);
                        yield return
                            new ExampleInfo(
                               category: typeExamplesAttribute.Category,
                               title: methodExampleAttribute.Title,
                               tags: tags.ToArray(),
                               method: method,
                               excludeFromAutomatedTests: methodExampleAttribute.ExcludeFromAutomatedTests);
                    }
                }
            }
        }

        /// <summary>
        /// 获取 ExampleLibrary 命名 空间下，利用 ExamplesAttribute 修饰的Type类型的
        /// 所有用  ExampleAttribute 修饰的 方法类型。返回 ExampleInfo集合
        /// </summary>
        public static IEnumerable<ExampleInfo> GetList()
        {
            IEnumerable<TypeInfo> typeInfos = typeof(ExampleLibrary.Examples).GetTypeInfo().Assembly.DefinedTypes;
            foreach (TypeInfo typeInfo in typeInfos)
            {
                if (!typeInfo.GetCustomAttributes<ExampleLibrary.ExamplesAttribute>().Any())
                {
                    continue;
                }

                Type categoryType = typeInfo.AsType();
                IEnumerable<ExampleLibrary.ExampleInfo> exampleInfos = GetCategory(categoryType: categoryType);
                foreach (ExampleLibrary.ExampleInfo example in exampleInfos)
                {
                    yield return example;
                }
            }
        }

        /// <summary>
        /// Gets all examples suitable for automated test.
        /// </summary>
        public static IEnumerable<ExampleInfo> GetListForAutomatedTest()
        {
            return GetList().Where(ex => !ex.ExcludeFromAutomatedTests);
        }

        /// <summary>
        /// Gets the first example of each category suitable for automated test.
        /// </summary>
        public static IEnumerable<ExampleInfo> GetFirstExampleOfEachCategoryForAutomatedTest()
        {
            return GetListForAutomatedTest()
                .GroupBy(example => example.Category)
                .Select(group => group.First());
        }

        /// <summary>
        /// Gets the 'rendering capabilities' examples suitable for automated test.
        /// </summary>
        public static IEnumerable<ExampleInfo> GetRenderingCapabilitiesForAutomatedTest()
        {
            return GetCategory(typeof(RenderingCapabilities)).Where(ex => !ex.ExcludeFromAutomatedTests);
        }
    }
}
