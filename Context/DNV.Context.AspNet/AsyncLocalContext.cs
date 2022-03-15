using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DNV.Context.AspNet
{
	internal class AsyncLocalContext<T> : IAmbientContext<T> where T : class
	{
		internal record ContextHolder
		{
			public T? Payload { get; set; }

			public string? CorrelationId { get; set; }

			public ConcurrentDictionary<object, object>? Items { get; set; }
		}

		private readonly AsyncLocal<ContextHolder> _contextHolder;

		public AsyncLocalContext()
		{
			_contextHolder = new AsyncLocal<ContextHolder>();
		}

		public bool HasValue => _contextHolder.Value != null;

		public void CreateContext(T? payload, string? correlationId, IDictionary<object, object>? items = null)
		{
			_contextHolder.Value = new ContextHolder
			{
				Payload = payload,
				CorrelationId = correlationId,
				Items = new ConcurrentDictionary<object, object>(items ?? Enumerable.Empty<KeyValuePair<object, object>>())
			};
		}

		public T? Payload => _contextHolder.Value?.Payload;

		public string? CorrelationId => _contextHolder.Value?.CorrelationId;

		public IDictionary<object, object>? Items => _contextHolder.Value?.Items;
	}
}
