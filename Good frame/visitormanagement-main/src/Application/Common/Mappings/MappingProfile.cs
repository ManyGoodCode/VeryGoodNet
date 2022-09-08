using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using AutoMapper;

namespace CleanArchitecture.Blazor.Application.Common.Mappings
{

    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            List<Type>? types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
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
