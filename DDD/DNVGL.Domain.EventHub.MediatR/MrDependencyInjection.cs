using DNV.Application.Abstractions;
using DNVGL.Domain.Seedwork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Domain.EventHub.MediatR
{
    /// <summary>
    /// 
    /// </summary>
    public static class MrDependencyInjection
    {
        public static IServiceCollection AddMidatREventHub(this IServiceCollection serviceCollection)
        {
            return serviceCollection
	            .AddSingleton<INotificationHandler<MrEventWrapper>, MrEventHandlerAdapter>()
                .AddTransient<IEventHub, MrEventHub>();
        }
    }
}
