﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DNV.Context.AspNet
{
    public class AspNetContextAccessor<T>: IContextAccessor<T>, IContextCreator<T> where T: class
    {
	    public static readonly string HeaderKey = $"X-Ambient-Context-{typeof(T).Name}";

		private readonly AsyncLocalContext<T> _asyncLocalContext;
	    private readonly Func<HttpContext, (bool, T?)> _payloadCreator;

	    public AspNetContextAccessor(Func<HttpContext, (bool, T?)> payloadCreator)
	    {
		    _asyncLocalContext = new AsyncLocalContext<T>();
			_payloadCreator = payloadCreator;
	    }

	    public bool Initialized => _asyncLocalContext.HasValue;

		public IAmbientContext<T>? Context => _asyncLocalContext.HasValue? _asyncLocalContext: null;

        public void Initialize(HttpContext httpContext, JsonSerializerOptions? jsonSerializerOptions = null)
        {
	        if (Initialized) return;

	        if (httpContext.Request.Headers.TryGetValue(HeaderKey, out var ctxJsonStr))
	        {
				if (jsonSerializerOptions == null) jsonSerializerOptions = new JsonSerializerOptions();
				jsonSerializerOptions.Converters.Add(new DictionaryStringObjectJsonConverter());

				var ctx = JsonSerializer.Deserialize<AsyncLocalContext<T>.ContextHolder>(ctxJsonStr, jsonSerializerOptions);

				if (ctx?.Payload == null) return;

				_asyncLocalContext.CreateContext(ctx.Payload, ctx.CorrelationId, ctx.Items);
	        }
	        else
	        {
				var (succeeded, payload) = _payloadCreator(httpContext);
				if (!succeeded || payload == null)
					return;

				_asyncLocalContext.CreateContext(payload, httpContext.TraceIdentifier);
	        }
        }

		public void InitializeContext(T? payload, string? correlationId, IDictionary<string, object>? items = null)
		{
			if (Initialized) return;

			_asyncLocalContext.CreateContext(payload, correlationId, items);
		}
	}
}
