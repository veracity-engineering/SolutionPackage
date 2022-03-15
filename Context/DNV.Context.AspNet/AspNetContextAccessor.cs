using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DNV.Context.AspNet
{
    public class AspNetContextAccessor<T>: IContextAccessor<T> where T: class
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

		public IAmbientContext<T>? Context => _asyncLocalContext;

        internal void Initialize(HttpContext httpContext, JsonSerializerSettings? jsonSerializerSettings)
        {
	        if (Initialized) return;

	        if (httpContext.Request.Headers.TryGetValue(HeaderKey, out var ctxJsonStr))
	        {
		        var serializer = JsonSerializer.CreateDefault(jsonSerializerSettings);

		        using var sr = new StringReader(ctxJsonStr);
		        using var jr = new JsonTextReader(sr);
		        var ctx = serializer.Deserialize<AsyncLocalContext<T>.ContextHolder>(jr);

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
    }
}
