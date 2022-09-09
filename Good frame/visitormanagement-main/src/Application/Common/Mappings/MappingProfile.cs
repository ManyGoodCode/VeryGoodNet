using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using AutoMapper;

namespace CleanArchitecture.Blazor.Application.Common.Mappings
{
    /// <summary>
    ///  查找指定程序集里面实现映射接口IMapFrom的类型，通过反射创建类型对象，并调用 void Mapping[AutoMapper.Profile profile] 方法，传入的参数为this
    /// </summary>
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            List<Type>? types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
            .ToList();

            foreach (Type type in types)
            {
                object? instance = Activator.CreateInstance(type: type);
                MethodInfo? methodInfo = type.GetMethod(name: "Mapping") ?? type.GetInterface(name: "IMapFrom`1")!.GetMethod(name: "Mapping");
                methodInfo?.Invoke(obj: instance, parameters: new object[] { this });
            }
        }
    }
}
