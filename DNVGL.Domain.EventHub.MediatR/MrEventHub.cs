using System;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;
using MediatR;

namespace DNVGL.Domain.EventHub.MediatR
{
    internal class MrEventHub : IEventHub
    {
        private readonly IMediator _mediator;

        public MrEventHub(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task PublishAsync(Event @event, CancellationToken cancellationToken = default)
        {
            return _mediator.Publish(MrEventWrapper.Create(@event), cancellationToken); 
        }
    }
}
