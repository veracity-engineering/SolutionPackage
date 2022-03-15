using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

			public ConcurrentDictionary<object, object> Items { get; } = new();
		}

		private readonly AsyncLocal<ContextHolder> _contextHolder;

		public AsyncLocalContext()
		{
			_contextHolder = new AsyncLocal<ContextHolder>
			{
				Value = new ContextHolder()
			};
		}

		public T? Payload
		{
			get => _contextHolder.Value.Payload;
			internal set => _contextHolder.Value.Payload = value;
		}

		public string? CorrelationId
		{
			get => _contextHolder.Value.CorrelationId;
			internal set => _contextHolder.Value.CorrelationId = value;
		}

		public IDictionary<object, object> Items => _contextHolder.Value.Items;
	}
}
