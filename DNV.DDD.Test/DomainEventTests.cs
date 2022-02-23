using DNV.Application.Abstractions;
using DNVGL.Domain.Seedwork;
using Xunit;

namespace DNV.DDD.Test
{
	public class TestDomainEventTypes : EventType
	{
		private TestDomainEventTypes(string eventName): base("TestDomainEvents", eventName, eventName) {}

		public static readonly TestDomainEventTypes EntityCreated = new TestDomainEventTypes(nameof(EntityCreated));

		public static readonly TestDomainEventTypes EntityChanged = new TestDomainEventTypes(nameof(EntityChanged));
	}

	public class EntityCreated : Event
	{
		public EntityCreated(Entity eventSource) : base(eventSource)
		{
		}

		public override EventType EventType => TestDomainEventTypes.EntityCreated;
	}

	public class EntityChanged : Event
	{
		public EntityChanged(Entity eventSource) : base(eventSource)
		{
		}

		public override EventType EventType => TestDomainEventTypes.EntityCreated;
	}

	public class DomainEventsHandler1 : IEventHandler<EntityCreated>, IEventHandler<EntityChanged>
	{
		public static int EventCount = 0;

		public Task HandleAsync(EntityCreated @event, CancellationToken cancellationToken = default)
		{
			EventCount++;
			Console.WriteLine(@event.EventType.Id);
			return Task.CompletedTask;
		}

		public Task HandleAsync(EntityChanged @event, CancellationToken cancellationToken = default)
		{
			EventCount++;
			Console.WriteLine(@event.EventType.Id);
			return Task.CompletedTask;
		}
	}

	public class DomainEventsHandler2 : IEventHandler<EntityCreated>, IEventHandler<EntityChanged>
	{
		public static int EventCount = 0;

		public Task HandleAsync(EntityCreated @event, CancellationToken cancellationToken = default)
		{
			EventCount++;
			Console.WriteLine(@event.EventType.Id);
			return Task.CompletedTask;
		}

		public Task HandleAsync(EntityChanged @event, CancellationToken cancellationToken = default)
		{
			EventCount++;
			Console.WriteLine(@event.EventType.Id);
			return Task.CompletedTask;
		}
	}


	public class DomainEventTests
    {
	    private readonly IEventHub _eventHub;

	    public DomainEventTests(IEventHub eventHub)
	    {
		    _eventHub = eventHub;

	    }

		[Fact]
	    public async Task RaiseDomainEvent()
	    {
		    await _eventHub.PublishAsync(new EntityCreated(null));
		    await _eventHub.PublishAsync(new EntityChanged(null));

			Assert.True(DomainEventsHandler1.EventCount > 1 & DomainEventsHandler2.EventCount > 1);
	    }
    }
}