using CleanArchitecture.Blazor.Application.Common.Behaviours;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.ApprovalHistories.EventHandlers;
using CleanArchitecture.Blazor.Application.Services.MessageService;
using CleanArchitecture.Blazor.Application.Services.Picklist;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace CleanArchitecture.Blazor.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            foreach (Assembly assembly in new[] { Assembly.GetExecutingAssembly() }) // add all your assemblies here
            {
                foreach (TypeInfo createdEvent in assembly
                    .DefinedTypes
                    .Where(dt => !dt.IsAbstract && dt.IsSubclassOf(typeof(ApprovalHistoryCreatedEventHandler)))
                )
                {
                    services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(createdEvent), typeof(ApprovalHistoryCreatedEventHandler));
                }
            }
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddLazyCache();
            services.AddScoped<SMSMessageService>();
            services.AddScoped<MailMessageService>();
            services.AddScoped<IPicklistService, PicklistService>();
            return services;
        }
    }
}
