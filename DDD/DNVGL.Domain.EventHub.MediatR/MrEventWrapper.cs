using System;
using DNVGL.Domain.Seedwork;
using MediatR;

namespace DNVGL.Domain.EventHub.MediatR
{
	internal class MrEventWrapper<T>: INotification where T: Event
    {
        public T DomainEvent { get; }

        private MrEventWrapper(T @event)
        {
            DomainEvent = @event ?? throw new ArgumentNullException(nameof(@event));
        }

        public static MrEventWrapper<T> Create(T @event)
        {
            return new MrEventWrapper<T>(@event);
        }
    }
}