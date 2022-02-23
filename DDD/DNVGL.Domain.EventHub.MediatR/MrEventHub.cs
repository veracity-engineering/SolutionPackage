using System;
using System.Threading;
using System.Threading.Tasks;
using DNV.Application.Abstractions;
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

        public Task PublishAsync<T>(T mrEvent, CancellationToken cancellationToken = default) where T : Event
        {
            return _mediator.Publish(MrEventWrapper<T>.Create(mrEvent), cancellationToken); 
        }
    }
}
