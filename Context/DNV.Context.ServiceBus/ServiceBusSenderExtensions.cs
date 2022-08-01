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
        public static async Task SendMessageAsync<T>(this ServiceBusSender serviceBusSender
            , ServiceBusMessage message
            , IContextAccessor<T> contextAccessor
            , JsonSerializerOptions? jsonSerializerOptions = null
            , CancellationToken cancellationToken = default) where T : class
        {
            var serviceBusMessageBuilder = new ServiceBusMessageBuilder<T>(contextAccessor, jsonSerializerOptions);
            await serviceBusSender.SendMessageAsync(serviceBusMessageBuilder.SerializeContextToMessage(message), cancellationToken);
        }

        public static async Task SendMessagesAsync<T>(this ServiceBusSender serviceBusSender
            , IEnumerable<ServiceBusMessage> messages
            , IContextAccessor<T> contextAccessor
            , JsonSerializerOptions? jsonSerializerOptions = null
            , CancellationToken cancellationToken = default) where T : class
        {
            var serviceBusMessageBuilder = new ServiceBusMessageBuilder<T>(contextAccessor, jsonSerializerOptions);
            await serviceBusSender.SendMessagesAsync(messages?.Select(msg => serviceBusMessageBuilder.SerializeContextToMessage(msg)), cancellationToken);
        }

        public static async Task<long> ScheduleMessageAsync<T>(this ServiceBusSender serviceBusSender
            , ServiceBusMessage message
            , DateTimeOffset scheduledEnqueueTime
            , IContextAccessor<T> contextAccessor
            , JsonSerializerOptions? jsonSerializerOptions = null
            , CancellationToken cancellationToken = default) where T : class
        {
            var serviceBusMessageBuilder = new ServiceBusMessageBuilder<T>(contextAccessor, jsonSerializerOptions);
            return await serviceBusSender.ScheduleMessageAsync(serviceBusMessageBuilder.SerializeContextToMessage(message), scheduledEnqueueTime, cancellationToken);
        }

        public static async Task<IReadOnlyList<long>> ScheduleMessagesAsync<T>(this ServiceBusSender serviceBusSender
            , IEnumerable<ServiceBusMessage> messages
            , DateTimeOffset scheduledEnqueueTime
            , IContextAccessor<T> contextAccessor
            , JsonSerializerOptions? jsonSerializerOptions = null
            , CancellationToken cancellationToken = default) where T : class
        {
            var serviceBusMessageBuilder = new ServiceBusMessageBuilder<T>(contextAccessor, jsonSerializerOptions);
            return await serviceBusSender.ScheduleMessagesAsync(messages?.Select(msg => serviceBusMessageBuilder.SerializeContextToMessage(msg)), scheduledEnqueueTime, cancellationToken);
        }

    }
}
