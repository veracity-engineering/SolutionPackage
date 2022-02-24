using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DNV.Application.Abstractions;
using DNVGL.Domain.Seedwork;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNVGL.Domain.EventHub.MediatR.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMrEventHub(this IServiceCollection services, params Type [] types)
        {
	        return services.AddMrEventHub(types.Select(t => t.GetTypeInfo().Assembly).ToArray());
        }

        public static IServiceCollection AddMrEventHub(this IServiceCollection services, params Assembly [] assemblies)
        {
            var definitionType = typeof(IEventHandler<>);
            var eventTypeList = new List<Type>();
            foreach (var handlerType in assemblies.SelectMany(a => a.DefinedTypes)
	                     .Where(t => !t.IsAbstract && !t.IsInterface)
	                     .Distinct())
            {
	            var interfaceTypes = handlerType.FindInterfacesThatClose(definitionType).ToList();

	            if (!interfaceTypes.Any()) continue;

	            foreach(var eventType in interfaceTypes.Where(t => t.IsAssignableFrom(handlerType))
		                    .SelectMany(t => t.GetGenericArguments())
		                    .Where(t => typeof(Event).IsAssignableFrom(t))
		                    .Distinct())
	            {
                    services.AddEventHandler(eventType, handlerType);
                    eventTypeList.Add(eventType);
	            }
            }

            eventTypeList.Distinct()
	            .ToList()
	            .ForEach(eventType => services.AddMrEventHandler(eventType));

            ServiceRegistrar.AddRequiredServices(services, new MediatRServiceConfiguration());

            return services.AddSingleton<IEventHub, MrEventHub>();
        }

        private static IServiceCollection AddEventHandler(this IServiceCollection services, Type eventType, Type handlerType)
        {
	        var interfaceType = typeof(IEventHandler<>).MakeGenericType(eventType);
	        return services.AddTransient(interfaceType, handlerType);
        }

        private static IServiceCollection AddMrEventHandler(this IServiceCollection services, Type eventType)
        {
	        var wrapperType = typeof(MrEventWrapper<>).MakeGenericType(eventType);
	        var interfaceType = typeof(INotificationHandler<>).MakeGenericType(wrapperType);
	        var handlerType = typeof(MrEventHandler<>).MakeGenericType(eventType);
	        services.TryAddTransient(interfaceType, handlerType);
	        return services;
        }
	}
}
