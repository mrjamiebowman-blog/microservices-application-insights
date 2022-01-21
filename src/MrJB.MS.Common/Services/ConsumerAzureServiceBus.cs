using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using MrJB.MS.Common.Configuration;

namespace MrJB.MS.Common.Services;

public class ConsumerAzureServiceBus : IConsumerService, IConsumerAzureServiceBus
{
    private readonly ILogger<ConsumerAzureServiceBus> _logger;
    private readonly TelemetryClient _telemetryClient;

    private readonly AzureServiceBusConsumerConfiguration _azureServiceBusConfiguration;

    private ServiceBusProcessor _processor;
    private CancellationToken _cancellationToken;

    public ConsumerAzureServiceBus(ILogger<ConsumerAzureServiceBus> logger, TelemetryClient telemetryClient, AzureServiceBusConsumerConfiguration azureServiceBusConsumerConfiguration)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
        _azureServiceBusConfiguration = azureServiceBusConsumerConfiguration;
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

        // we can evaluate application logic and use that to determine how to settle the message.
        await args.CompleteMessageAsync(args.Message);
    }

    public Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // the error source tells me at what point in the processing an error occurred
        Console.WriteLine(args.ErrorSource);
        // the fully qualified namespace is available
        Console.WriteLine(args.FullyQualifiedNamespace);
        // as well as the entity path
        Console.WriteLine(args.EntityPath);
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}
