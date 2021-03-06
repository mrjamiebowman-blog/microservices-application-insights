using Microsoft.ApplicationInsights;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Models;
using MrJB.MS.Common.Services;
using System.Text.Json;

namespace MrJB.MS.Consumer2;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly TelemetryClient _telemetryClient;

    private readonly IApplicationLifetime _lifetime;

    // services
    private readonly IConsumerService _consumerService;
    private readonly IProducerService _producerService;

    // configuration
    private readonly AzureServiceBusProducerConfiguration _azureServiceBusProducerConfiguration;

    public Worker(ILogger<Worker> logger, TelemetryClient telemetryClient, IApplicationLifetime lifetime, IConsumerService consumerService, IProducerService producerService, AzureServiceBusProducerConfiguration azureServiceBusProducerConfiguration)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
        _lifetime = lifetime;
        _consumerService = consumerService;
        _producerService = producerService;
        _azureServiceBusProducerConfiguration = azureServiceBusProducerConfiguration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumerService.ProcessMessageAsync += ProcessMessage;
        return _consumerService.StartConsumingAsync(cancellationToken);
    }

    /// <summary>
    /// I don't normally use delegates and events
    /// </summary>
    private async Task ProcessMessage(string message, string operationId, string parentId, CancellationToken cancellationToken)
    {
        // log
        _logger.LogInformation($"Consumer 2: Received Message. Operation ID: ({operationId}), Parent ID: ({parentId}).");

        // serialize message
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var order = JsonSerializer.Deserialize<Order>(message, serializerOptions);

        // process (produce to next service)
        await _producerService.ProduceAsync<Order>(order, _azureServiceBusProducerConfiguration.QueueOrTopic, operationId, parentId, cancellationToken);

        // manually flush (will get to ai faster)
        _telemetryClient.Flush();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _consumerService.StopConsumingAsync(cancellationToken);
    }
}
