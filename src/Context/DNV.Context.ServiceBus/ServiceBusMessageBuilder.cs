using Azure.Messaging.ServiceBus;
using DNV.Context.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace DNV.Context.ServiceBus
{

    public interface IServiceBusMessageBuilder<T> where T : class
	{
        ServiceBusMessage CreateServiceBusMessage();
        ServiceBusMessage CreateServiceBusMessage(BinaryData body);
        ServiceBusMessage CreateServiceBusMessage(ReadOnlyMemory<byte> body);
        ServiceBusMessage CreateServiceBusMessage(ServiceBusReceivedMessage receivedMessage);
        ServiceBusMessage CreateServiceBusMessage(string body);

    }

    public class ServiceBusMessageBuilder<T>: IServiceBusMessageBuilder<T> where T : class
    {
        private readonly IContextAccessor<T> _contextAccessor;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;

        public ServiceBusMessageBuilder(IContextAccessor<T> contextAccessor, IOptions<JsonSerializerOptions>? jsonSerializerOptions)
        {
            _contextAccessor = contextAccessor;
            _jsonSerializerOptions = jsonSerializerOptions?.Value;
        }

        public ServiceBusMessageBuilder(IContextAccessor<T> contextAccessor, JsonSerializerOptions? jsonSerializerOptions)
        {
            _contextAccessor = contextAccessor;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

		public ServiceBusMessage CreateServiceBusMessage()
		{
			return SerializeContextToMessage(new ServiceBusMessage());
		}

		public ServiceBusMessage CreateServiceBusMessage(BinaryData body)
		{
			return SerializeContextToMessage(new ServiceBusMessage(body));
		}

		public ServiceBusMessage CreateServiceBusMessage(ReadOnlyMemory<byte> body)
		{
			return SerializeContextToMessage(new ServiceBusMessage(body));
		}

		public ServiceBusMessage CreateServiceBusMessage(ServiceBusReceivedMessage receivedMessage)
		{
			return SerializeContextToMessage(new ServiceBusMessage(receivedMessage));
		}

		public ServiceBusMessage CreateServiceBusMessage(string body)
		{
			return SerializeContextToMessage(new ServiceBusMessage(body));
		}

		internal ServiceBusMessage SerializeContextToMessage(ServiceBusMessage message)
		{
			if (_contextAccessor.Context == null)
				return message;


			var json = JsonSerializer.Serialize(_contextAccessor.Context, _jsonSerializerOptions);

			message.CorrelationId = _contextAccessor.Context.CorrelationId;
			message.ApplicationProperties.Add(LocalContextAccessor<T>.HeaderKey, json);

			return message;
		}

	}
}
