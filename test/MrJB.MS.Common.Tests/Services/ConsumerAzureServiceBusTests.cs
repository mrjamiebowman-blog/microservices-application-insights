using Azure.Core.Amqp;
using Azure.Messaging.ServiceBus;
using Moq;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Services;
using MrJB.MS.Common.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Xunit;

namespace MrJB.MS.Common.Tests
{
    public class ConsumerAzureServiceBusTests
    {

        /// <summary>
        /// This private method will instantiate a ServiceBusReceivedMessage using reflection.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private ServiceBusReceivedMessage GetServiceBusReceivedMessage(string json)
        {
            // read only bytes
            IEnumerable<ReadOnlyMemory<byte>> body = new List<ReadOnlyMemory<byte>>() { new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json)) };

            // amqp message
            AmqpAnnotatedMessage amqpAnotatedMessage = new AmqpAnnotatedMessage(new AmqpMessageBody(body));

            // constructor parameter types
            Type[] paramTypes = new[]
            {
                typeof(AmqpAnnotatedMessage)
            };

            // create service bus received message
            ServiceBusReceivedMessage serviceBusReceivedMessage =
                (ServiceBusReceivedMessage)typeof(ServiceBusReceivedMessage).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null)
                ?.Invoke(new object[] { amqpAnotatedMessage });

            // set delivery count
            var amqpMessage = serviceBusReceivedMessage.GetType()
                                                       .GetProperty("AmqpMessage", BindingFlags.NonPublic | BindingFlags.Instance)
                                                       .GetValue(serviceBusReceivedMessage);

            var amqpMessageHeader = amqpMessage.GetType()
                                                .GetProperty("Header", BindingFlags.NonPublic | BindingFlags.Instance)
                                                .GetValue(amqpMessage);

            // set the delivery count to 1
            uint deliveryCount = 1;
            amqpMessageHeader.GetType().GetProperty("DeliveryCount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(amqpMessageHeader, deliveryCount);

            return serviceBusReceivedMessage;
        }

        [Fact]
        public void MessageHandlerTests()
        {
            // arrange
            CancellationTokenSource cts = new CancellationTokenSource();

            // json
            var json = "";

            // mock
            var mockServiceBusReceiver = new Mock<ServiceBusReceiver>();

            // service bus message received
            var serviceBusReceivedMessage = GetServiceBusReceivedMessage(json);

            // proccess event args
            var processMessageEventArgs = new ProcessMessageEventArgs(serviceBusReceivedMessage, mockServiceBusReceiver.Object, cts.Token);

            // logging
            var logger = new FakeLogger<ConsumerAzureServiceBus>();
            var telemetryClient = TelemetryHelper.GetFakeTelemetryClient();

            // configuration
            var azureServiceBusConsumerConfiguration = new AzureServiceBusConsumerConfiguration();
            azureServiceBusConsumerConfiguration.QueueOrTopic = "queue-or-topic";
            azureServiceBusConsumerConfiguration.SubscriptionName = "subscription-name";

            // act
            Action action = () => typeof(ConsumerAzureServiceBus).GetMethod("MessageHandler", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(new ConsumerAzureServiceBus(logger, telemetryClient, azureServiceBusConsumerConfiguration), new object[] { processMessageEventArgs });

            // assert
            //action.Should().NotThrow();
        }

        [Fact]
        public void ErrorHandlerTests()
        {

        }
    }
}