using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Extensions;
using System.Diagnostics;

namespace MrJB.MS.Common.Services;

public class ConsumerAzureServiceBus : IConsumerService, IConsumerAzureServiceBus
{
    private readonly ILogger<ConsumerAzureServiceBus> _logger;
    private readonly TelemetryClient _telemetryClient;

    private readonly AzureServiceBusConsumerConfiguration _azureServiceBusConfiguration;

    private ServiceBusProcessor _processor;
    private CancellationToken _cancellationToken;

    // delegates and events for message processing
    public delegate Task MessageReceivedAsync(string message, string operationId, string parentId, CancellationToken cancellationToken);
    public event IConsumerService.MessageReceivedAsync ProcessMessageAsync;

    // object lock
    object objectLock = new object();

    public ConsumerAzureServiceBus(ILogger<ConsumerAzureServiceBus> logger, TelemetryClient telemetryClient, AzureServiceBusConsumerConfiguration azureServiceBusConsumerConfiguration)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
        _azureServiceBusConfiguration = azureServiceBusConsumerConfiguration;
    }

    event IConsumerService.MessageReceivedAsync IConsumerService.ProcessMessageAsync
    {
        add
        {
            lock (objectLock)
            {
                ProcessMessageAsync += value;
            }
        }

        remove
        {
            lock (objectLock)
            {
                ProcessMessageAsync -= value;
            }
        }
    }

    public void LogStartupInformation()
    {
        _logger.LogInformation("Azure Service Bus Consumer Starting.");
    }

    public Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        LogStartupInformation();
        return StartReceivingMessagesAsync(_azureServiceBusConfiguration.QueueOrTopic, _azureServiceBusConfiguration.SubscriptionName, cancellationToken);
    }

    public async Task StartReceivingMessagesAsync(string queueOrTopic, string subscriptionName, CancellationToken cancellationToken)
    {
        try
        {
            var client = new ServiceBusClient(_azureServiceBusConfiguration.ConnectionString);

            // create a processor
            _processor = client.CreateProcessor(queueOrTopic, subscriptionName, new ServiceBusProcessorOptions());

            // cancellation token
            _cancellationToken = cancellationToken;

            // process message handler
            _processor.ProcessMessageAsync += MessageHandler;

            // error handler
            _processor.ProcessErrorAsync += ErrorHandler;

            // start processing
            await _processor.StartProcessingAsync(_cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"ConsumerAzureServiceBus->ReceiveMessageAsync(queueOrTopic: ({queueOrTopic}), subscriptionName: ({subscriptionName}).", ex);
        }
    }

    public Task StopConsumingAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();

        // extract root operation id and parent id
        (var rootOperationId, var parentId) = args.Message.GetCorrelationIds();

        var activity = new Activity("ServiceBusProcessor.ProcessMessage");
        activity.SetParentId(parentId);

        using (var operation = _telemetryClient.StartOperation<RequestTelemetry>("Process", rootOperationId, activity.ParentId))
        {
            // log information
            _logger.LogInformation($"Received Message (queueOrTopic: ({_azureServiceBusConfiguration.QueueOrTopic}), subscriptionName: ({_azureServiceBusConfiguration.SubscriptionName}).");
            _logger.LogInformation($"{body}");

            // update parent id
            parentId = operation.Telemetry.Id;

            // process message
            if (ProcessMessageAsync != null)
            {
                await ProcessMessageAsync?.Invoke(body, rootOperationId, parentId, _cancellationToken);
            }

            // we can evaluate application logic and use that to determine how to settle the message.
            await args.CompleteMessageAsync(args.Message);
        }
    }

    public Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // the error source tells me at what point in the processing an error occurred
        Console.WriteLine(args.ErrorSource);
        Console.WriteLine(args.FullyQualifiedNamespace);
        Console.WriteLine(args.EntityPath);
        Console.WriteLine(args.Exception.ToString());

        return Task.CompletedTask;
    }
}
