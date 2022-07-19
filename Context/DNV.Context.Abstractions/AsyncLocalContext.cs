using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DNV.Context.Abstractions;

namespace DNV.Context.Abstractions
{
	public class AsyncLocalContext<T> : IAmbientContext<T> where T : class
	{
		public record ContextHolder
		{
			public T? Payload { get; set; }

			public string? CorrelationId { get; set; }

			public Dictionary<string, object>? Items { get; set; }
		}

		private readonly AsyncLocal<ContextHolder> _contextHolder;

		public AsyncLocalContext()
		{
			_contextHolder = new AsyncLocal<ContextHolder>();
		}

		public bool HasValue => _contextHolder.Value != null;

		public void CreateContext(T? payload, string? correlationId, IDictionary<string, object>? items = null)
		{
			_contextHolder.Value = new ContextHolder
			{
				Payload = payload,
				CorrelationId = correlationId,
				Items = new Dictionary<string, object>(items)
			};
		}

		public T? Payload => _contextHolder.Value?.Payload;

		public string? CorrelationId => _contextHolder.Value?.CorrelationId;

		public IDictionary<string, object>? Items => _contextHolder.Value?.Items;
	}
}
