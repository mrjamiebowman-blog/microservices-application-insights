using Azure.Core.Amqp;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Moq;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Models;
using MrJB.MS.Common.Services;
using MrJB.MS.Common.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
            Type[] paramTypes = new[] {
                typeof(AmqpAnnotatedMessage)
            };

            // create service bus received message
            ServiceBusReceivedMessage serviceBusReceivedMessage =
                (ServiceBusReceivedMessage) typeof(ServiceBusReceivedMessage)
                    .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, paramTypes, null)
                        ?.Invoke(new Object[] { amqpAnotatedMessage });

            // set delivery count
            var amqpMessage = serviceBusReceivedMessage.GetType()
                                                       .GetProperty("AmqpMessage", BindingFlags.NonPublic | BindingFlags.Instance)
                                                       .GetValue(serviceBusReceivedMessage);

            // message header
            var amqpMessageHeader = amqpMessage.GetType()
                                               .GetProperty("Header", BindingFlags.Public | BindingFlags.Instance)
                                               .GetValue(amqpMessage);

            // set the delivery count to 1
            uint deliveryCount = 1;
            amqpMessageHeader.GetType().GetProperty("DeliveryCount", BindingFlags.Public | BindingFlags.Instance).SetValue(amqpMessageHeader, deliveryCount);

            return serviceBusReceivedMessage;
        }

        /// <summary>
        /// I used this to create the json for the unit tests.
        /// </summary>
        //[Fact]
        [SkippableFact]
        public void CreateJsonTest()
        {
            // create order
            var order = new Order();
            order.OrderId = new Random().Next(1, 1000);
            order.Subtotal = new Random().Next(100, 500); ;
            order.Tax = order.Subtotal * 0.07M;
            order.Total = order.Subtotal + order.Tax;

            // billing address
            order.BillingAddress = new CustomerAddress()
            {
                FirstName = "Jamie",
                LastName = "Bowman",
                StreetAddress1 = "123 Street",
                StreetAddress2 = "Apt #2",
                City = "Saint Louis",
                State = "MO",
                Country = "USA",
                PostalCode = "12345"
            };

            // shipping address
            order.ShippingAddress = new CustomerAddress()
            {
                FirstName = "Jamie",
                LastName = "Bowman",
                StreetAddress1 = "123 Street",
                StreetAddress2 = "Apt #2",
                City = "Saint Louis",
                State = "MO",
                Country = "USA",
                PostalCode = "12345"
            };

            // json
            var json = JsonSerializer.Serialize(order);

            json.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void MessageHandlerTests()
        {
            // arrange
            CancellationTokenSource cts = new CancellationTokenSource();

            // json
            var json = "{\"OrderId\":92,\"BillingAddress\":{\"CustomerAddressId\":null,\"FirstName\":\"Jamie\",\"LastName\":\"Bowman\",\"StreetAddress1\":\"123 Street\",\"StreetAddress2\":\"Apt #2\",\"City\":\"Saint Louis\",\"State\":\"MO\",\"PostalCode\":\"12345\",\"Country\":\"USA\"},\"ShippingAddress\":{\"CustomerAddressId\":null,\"FirstName\":\"Jamie\",\"LastName\":\"Bowman\",\"StreetAddress1\":\"123 Street\",\"StreetAddress2\":\"Apt #2\",\"City\":\"Saint Louis\",\"State\":\"MO\",\"PostalCode\":\"12345\",\"Country\":\"USA\"},\"Subtotal\":404,\"Tax\":28.28,\"Total\":432.28}";

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

            // consumer
            var consumer = new ConsumerAzureServiceBus(logger, telemetryClient, azureServiceBusConsumerConfiguration);

            consumer.ProcessMessageAsync += (string message, string operationId, string parentId, CancellationToken cancellationToken) =>
            {
                // assertions

                return Task.CompletedTask;
            };

            // act
            typeof(ConsumerAzureServiceBus).GetMethod("MessageHandler", BindingFlags.Public | BindingFlags.Instance).Invoke(consumer, new Object[] { processMessageEventArgs }); ;
        }

        [Fact]
        public void ErrorHandlerTests()
        {

        }
    }
}