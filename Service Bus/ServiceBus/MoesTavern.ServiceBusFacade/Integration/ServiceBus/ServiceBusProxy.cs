using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoesTavern.ServiceBusFacade.Integration
{
    public class ServiceBusProxy
    {
        private const string TopicPath = "bar";

        private ServiceBusConnectionStringBuilder OrderQueueCnBuilder { get; }
        private ServiceBusConnectionStringBuilder BarTopicCnBuilder { get; }
        private ServiceBusConnectionStringBuilder NamespaceCnBuilder { get; }

        public ServiceBusProxy(IConfiguration configuration)
        {
            OrderQueueCnBuilder = new ServiceBusConnectionStringBuilder(configuration.GetValue<string>("Azure:ServiceBus:OrdersQueue"));
            BarTopicCnBuilder = new ServiceBusConnectionStringBuilder(configuration.GetValue<string>("Azure:ServiceBus:BarTopic"));
            NamespaceCnBuilder = new ServiceBusConnectionStringBuilder(configuration.GetValue<string>("Azure:ServiceBus:Namespace"));
        }

        #region Sending

        public async Task SendAsync<T>(T instance)
        {
            var client = new QueueClient(OrderQueueCnBuilder);

            Message message = GetMessage<T>(instance);

            await SendMessageAsync(client, message);
        }

        public async Task SendAsync<T>(T instance, KeyValuePair<string, object> userProperty)
        {
            var client = new TopicClient(BarTopicCnBuilder);

            Message message = GetMessage<T>(instance);

            message.UserProperties.Add(userProperty);

            await SendMessageAsync(client, message);
        }

        private static Message GetMessage<T>(T instance)
        {
            byte[] body = JsonSerializer.SerializeToUtf8Bytes(instance);
            return new Message(body);
        }

        private static async Task SendMessageAsync(ISenderClient client, Message message)
        {
            await client.SendAsync(message);
            await client.CloseAsync();
        }

        #endregion

        #region Retrieving

        public async Task<T> GetAsync<T>()
        {
            var client = new QueueClient(OrderQueueCnBuilder);

            return await GetMessageBodyAsync<T>(client);
        }

        public async Task<T> GetAsync<T>(string subscriptionName)
        {
            var client = new SubscriptionClient(BarTopicCnBuilder, subscriptionName);

            return await GetMessageBodyAsync<T>(client);
        }

        private static async Task<T> GetMessageBodyAsync<T>(IReceiverClient client)
        {
            string body = null;

            client.RegisterMessageHandler(async (msg, token) =>
            {
                Message message = msg;

                if(message != null)
                {
                    body = Encoding.UTF8.GetString(message.Body);

                    if (body.Contains("spoiled", StringComparison.OrdinalIgnoreCase))
                    {
                        await client.DeadLetterAsync(msg.SystemProperties.LockToken);
                    }
                    else
                    {
                        await client.CompleteAsync(msg.SystemProperties.LockToken);
                    }
                }
            }, new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            });

            await client.UnregisterMessageHandlerAsync(TimeSpan.FromMilliseconds(2000));
            await client.CloseAsync();

            if (body != null)
            {
                return JsonSerializer.Deserialize<T>(body);
            }

            return default;
        }

        public async Task<long> GetMessageCountAsync()
        {
            var client = new ManagementClient(NamespaceCnBuilder);

            QueueRuntimeInfo description =
                await client.GetQueueRuntimeInfoAsync("orders");
            return description.MessageCount;
        }

        public async Task<long> GetMessageCountAsync(string subscriptionName)
        {
            var client = new ManagementClient(NamespaceCnBuilder);

            SubscriptionRuntimeInfo description =
                await client.GetSubscriptionRuntimeInfoAsync(TopicPath, subscriptionName);
            return description.MessageCount;
        }

        #endregion

        #region Subscribing & unsubscribing

        public async Task SubscribeAsync(string subscriptionName)
        {
            var client = new ManagementClient(NamespaceCnBuilder);

            var description = new SubscriptionDescription(TopicPath, subscriptionName);
            var rule = new RuleDescription("MyDrinks", new SqlFilter($"OrderedFor = 'Everyone' OR OrderedFor = '{subscriptionName}'"));

            await client.CreateSubscriptionAsync(description, rule);
        }

        public async Task UnsubscribeAsync(string subscriptionName)
        {
            var client = new ManagementClient(NamespaceCnBuilder);

            if(await client.SubscriptionExistsAsync(TopicPath, subscriptionName))
            {
                await client.DeleteSubscriptionAsync(TopicPath, subscriptionName);
            }
        }

        #endregion

        #region Exception handling

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            // logging goes here
            return Task.CompletedTask;
        }

        #endregion
    }
}
