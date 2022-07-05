using Azure.Messaging.ServiceBus;
using DNV.Context.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DNV.Context.ServiceBus
{
    public static class ServiceBusSenderExtensions
    {
        public static async Task SendMessageAsync<T>(this ServiceBusSender serviceBusSender, ServiceBusMessage message, IContextAccessor<T> contextAccessor, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default) where T : class
        {
            var serviceBusMessageBuilder = new ServiceBusMessageBuilder<T>(contextAccessor, jsonSerializerOptions);
            await serviceBusSender.SendMessageAsync(serviceBusMessageBuilder.SerializeContextToMessage(message), cancellationToken);
        }

        public static async Task SendMessagesAsync<T>(this ServiceBusSender serviceBusSender, IEnumerable<ServiceBusMessage> messages, IContextAccessor<T> contextAccessor, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default) where T : class
        {
            var serviceBusMessageBuilder = new ServiceBusMessageBuilder<T>(contextAccessor, jsonSerializerOptions);
            await serviceBusSender.SendMessagesAsync(messages?.Select(msg => serviceBusMessageBuilder.SerializeContextToMessage(msg)), cancellationToken);
        }
    }
}
