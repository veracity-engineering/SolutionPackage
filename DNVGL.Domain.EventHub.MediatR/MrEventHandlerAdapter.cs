using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Domain.EventHub.MediatR
{
    internal class MrEventHandlerAdapter : INotificationHandler<MrEventWrapper>
    {
        private static readonly ConcurrentDictionary<Type, Delegate> TypesMapping = new ConcurrentDictionary<Type, Delegate>();

        private readonly IServiceProvider _serviceProvider;

        public MrEventHandlerAdapter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task Handle(MrEventWrapper notification, CancellationToken cancellationToken)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            var action = ExtractActionDelegate(notification.DomainEvent?.GetType());
            var handlers = _serviceProvider.GetServices(action.Method.GetParameters().First().ParameterType);

            var tasks = handlers
                .Select(h => action.DynamicInvoke(h, notification.DomainEvent, cancellationToken))
                .Cast<Task>()
                .ToArray();

            await Task.WhenAll(tasks);
        }

        private static Delegate ExtractActionDelegate(Type domainEventType)
        {
            if (domainEventType == null)
                throw new ArgumentNullException(nameof(domainEventType));

            return TypesMapping.GetOrAdd(domainEventType, k =>
            {
                var method = typeof(MrEventHandlerAdapter).GetMethod(nameof(DoActionAsync),
                    BindingFlags.Static | BindingFlags.NonPublic);

                if (method == null)
                    throw new ApplicationException($"Method: '{nameof(DoActionAsync)}' cannot be found.");

                var @delegate = method.MakeGenericMethod(k)
                    .CreateDelegate(
                        Expression.GetDelegateType(
                            method.GetParameters()
                                .Select(p => p.ParameterType)
                                .Concat(new[] { method.ReturnType })
                                .ToArray()
                        ));
                return @delegate;
            });

        }

        private static Task DoActionAsync<T>(IEventHandler<T> handler, T @event, CancellationToken cancellationToken) where T : Event
        {
            return handler.HandleAsync(@event, cancellationToken);
        }
    }
}