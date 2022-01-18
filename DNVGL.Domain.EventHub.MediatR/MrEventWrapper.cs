using System;
using DNVGL.Domain.Seedwork;
using MediatR;

namespace DNVGL.Domain.EventHub.MediatR
{
	internal class MrEventWrapper: INotification
    {
        public Event DomainEvent { get; }

        private MrEventWrapper(Event @event)
        {
            DomainEvent = @event ?? throw new ArgumentNullException(nameof(@event));
        }

        public static MrEventWrapper Create(Event @event)
        {
            return new MrEventWrapper(@event);
        }
    }
}