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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MrJB.MS.Common.Tests
{
    public class ConsumerAzureServiceBusTests
    {
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

            // vars
            var rootOperationId = "operation-id-123";
            var parentId = "parent-id-123";

            // service bus message received
            var applicationProperties = new Dictionary<string, object>();
            applicationProperties.Add("OperationId", rootOperationId);
            applicationProperties.Add("ParentId", parentId);

            // service bus model factory
            var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
                    body: BinaryData.FromString(json),
                    properties: applicationProperties,
                    deliveryCount: 1);

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
                operationId.Should().Be(rootOperationId);
                parentId.Should().Be(parentId);

                return Task.CompletedTask;
            };

            // act
            typeof(ConsumerAzureServiceBus).GetMethod("MessageHandler", BindingFlags.Public | BindingFlags.Instance).Invoke(consumer, new Object[] { processMessageEventArgs });

            // assert logging messages.

            var message1 = $"Received Message (queueOrTopic: ({azureServiceBusConsumerConfiguration.QueueOrTopic}), subscriptionName: ({azureServiceBusConsumerConfiguration.SubscriptionName}), Operation ID: ({rootOperationId}), Parent ID: ({parentId}).";
            var message2 = $"{json}";

            logger.messages.ToString().Should().Contain(message1);
            logger.messages.ToString().Should().Contain(message2);
        }

        [Fact]
        public void ErrorHandlerTests()
        {
            // arrange
            CancellationTokenSource cts = new CancellationTokenSource();

            var exception = new Exception("Error message.");
            ServiceBusErrorSource errorSource = new ServiceBusErrorSource();

            // process error event args
            var processErrorEventArgs = new ProcessErrorEventArgs(exception, errorSource, "", "", cts.Token);

            // logging
            var logger = new FakeLogger<ConsumerAzureServiceBus>();
            var telemetryClient = TelemetryHelper.GetFakeTelemetryClient();

            // configuration
            var azureServiceBusConsumerConfiguration = new AzureServiceBusConsumerConfiguration();
            azureServiceBusConsumerConfiguration.QueueOrTopic = "queue-or-topic";
            azureServiceBusConsumerConfiguration.SubscriptionName = "subscription-name";

            // consumer
            var consumer = new ConsumerAzureServiceBus(logger, telemetryClient, azureServiceBusConsumerConfiguration);

            typeof(ConsumerAzureServiceBus).GetMethod("ErrorHandler", BindingFlags.Public | BindingFlags.Instance).Invoke(consumer, new Object[] { processErrorEventArgs });

            // assert
            var messages = logger.messages.ToString();

            messages.Should().Contain($"ErrorHandler: {exception.Message}");
        }
    }
}