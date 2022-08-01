using Azure.Messaging.ServiceBus;
using DNV.Context.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DNV.Context.ServiceBus
{
    public static class ServiceBusProcessorExtensions
    {
        public static Func<ProcessMessageEventArgs, Task> InitializeContext<T>(this Func<ProcessMessageEventArgs, Task> messageHandler
            , IContextCreator<T> contextCreator
            , Func<ServiceBusReceivedMessage, (bool succeeded, T? context)> ctxCreator
            , JsonSerializerOptions? jsonSerializerOptions = null) where T : class
        {
            return (ProcessMessageEventArgs args) =>
            {
                InitializeContext(args.Message, contextCreator, ctxCreator, jsonSerializerOptions);
                return messageHandler.Invoke(args);
            };
        }

        public static Func<ProcessSessionMessageEventArgs, Task> InitializeContext<T>(this Func<ProcessSessionMessageEventArgs, Task> messageHandler
            , IContextCreator<T> contextCreator
            , Func<ServiceBusReceivedMessage, (bool succeeded, T? context)> ctxCreator
            , JsonSerializerOptions? jsonSerializerOptions = null) where T : class
        {
            return (ProcessSessionMessageEventArgs args) =>
            {
                InitializeContext(args.Message, contextCreator, ctxCreator, jsonSerializerOptions);
                return messageHandler.Invoke(args);
            };
        }

        private static void InitializeContext<T>(ServiceBusReceivedMessage message
            , IContextCreator<T> contextCreator
            , Func<ServiceBusReceivedMessage, (bool succeeded, T? context)> ctxCreator
            , JsonSerializerOptions? jsonSerializerOptions = null) where T : class
        {
            var localContextAccessor = new LocalContextAccessor<T>(ctxCreator);
            var contextFromMessage = localContextAccessor.ParseContextFromMessage(message, jsonSerializerOptions);

            if (contextFromMessage != null)
            {
                contextCreator.InitializeContext(contextFromMessage.Value.payload, contextFromMessage.Value.correlationId, contextFromMessage.Value.items);
            }
        }
    }
}
