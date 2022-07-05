using Azure.Messaging.ServiceBus;
using DNV.Context.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DNV.Context.ServiceBus
{
    public class LocalContextAccessor<T> : IContextAccessor<T>, IContextCreator<T> where T : class
    {
        public static readonly string HeaderKey = $"X-Ambient-Context-{typeof(T).Name}";

        private readonly AsyncLocalContext<T> _asyncLocalContext;
        private readonly Func<ServiceBusReceivedMessage, (bool, T?)> _payloadCreator;

        public LocalContextAccessor(Func<ServiceBusReceivedMessage, (bool, T?)> payloadCreator)
        {
            _asyncLocalContext = new AsyncLocalContext<T>();
            _payloadCreator = payloadCreator;
        }


        public bool Initialized => _asyncLocalContext.HasValue;

        public IAmbientContext<T>? Context => _asyncLocalContext;

        public void Initialize(ServiceBusReceivedMessage serviceBusMessage, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            if (Initialized) return;

            var contextFromMessage = ParseContextFromMessage(serviceBusMessage, jsonSerializerOptions);

            if(contextFromMessage == null) return;

            InitializeContext(contextFromMessage.Value.payload, contextFromMessage.Value.correlationId, contextFromMessage.Value.items);

        }

        internal (T? payload, string? correlationId, IDictionary<object, object>? items)? ParseContextFromMessage(ServiceBusReceivedMessage serviceBusMessage
            , JsonSerializerOptions? jsonSerializerOptions = null)
        {

            if (serviceBusMessage.ApplicationProperties.TryGetValue(HeaderKey, out var ctxJsonStr))
            {
                var ctx = JsonSerializer.Deserialize<AsyncLocalContext<T>.ContextHolder>(ctxJsonStr.ToString(), jsonSerializerOptions);

                if (ctx?.Payload == null) return null;

                return (ctx.Payload, serviceBusMessage.CorrelationId ?? ctx.CorrelationId, ctx.Items);
            }
            else
            {
                var (succeeded, payload) = _payloadCreator(serviceBusMessage);
                if (!succeeded || payload == null)
                    return null;

                return (payload, serviceBusMessage.CorrelationId ?? serviceBusMessage.MessageId, null);
            }
        }

        public void InitializeContext(T? payload, string? correlationId, IDictionary<object, object>? items = null)
        {
            if (Initialized) return;

            _asyncLocalContext.CreateContext(payload, correlationId, items);
        }


    }
}
