using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Helpers;
using System.Text.Json;

namespace MrJB.MS.Common.Services;
    
public class ProducerAzureServiceBus : IProducerService
{
    // services
    private readonly ILogger<ProducerAzureServiceBus> _logger;
    private readonly TelemetryClient _telemetryClient;

    // configuration
    AzureServiceBusProducerConfiguration _azureServiceBusConfiguration;

    public ProducerAzureServiceBus(ILogger<ProducerAzureServiceBus> logger, TelemetryClient telemetryClient, AzureServiceBusProducerConfiguration azureServiceBusConfiguration)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
        _azureServiceBusConfiguration = azureServiceBusConfiguration;
    }

    public virtual ServiceBusClient GetServiceBusClient()
    {
        return new ServiceBusClient(_azureServiceBusConfiguration.ConnectionString);
    }

    public async Task ProduceAsync<T>(T data, string queue, string operationId, string parentId, CancellationToken cancellationToken)
    {
        var operation = _telemetryClient.StartOperation<DependencyTelemetry>($"enqueue {queue}");
        operation.Telemetry.Type = "Azure Service Bus";
        operation.Telemetry.Data = $"Enqueue {queue}";
        operation.Telemetry.Context.Operation.Id = operationId;
        operation.Telemetry.Context.Operation.ParentId = parentId;

        try
        {
            // create an Azure Service Bus client
            await using var serviceBusClient = GetServiceBusClient();

            // create a sender for the queue
            ServiceBusSender sender = serviceBusClient.CreateSender(queue);

            // json
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(data, serializerOptions);

            // create a message that we can send
            ServiceBusMessage message = new ServiceBusMessage(json);
            message.ApplicationProperties.Add("OperationId", operation.Telemetry.Context.Operation.Id);
            message.ApplicationProperties.Add("ParentId", operation.Telemetry.Id);

            // send the message
            await sender.SendMessageAsync(message, cancellationToken);

            // log event
            _logger.LogInformation($"Produced data to Queue: ({queue}).", new Dictionary<string, string> { { "Message", JsonSerializer.Serialize<T>(data, JsonSerializerHelper.HumanReadable) } });

        } catch (Exception ex)
        {
            var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
            _logger.LogError($"ProducerAzureServiceBus->ProduceAsync(queue: ({queue}), operationId: ({operationId}), parentId: ({parentId}), Data: ({jsonData}).");
            _telemetryClient.TrackException(ex, new Dictionary<string, string> { { "Message", JsonSerializer.Serialize(data, JsonSerializerHelper.Message) } });
            operation.Telemetry.Success = false;
            throw;
        }
        finally
        {
            _telemetryClient.StopOperation(operation);
        }
    }
}
