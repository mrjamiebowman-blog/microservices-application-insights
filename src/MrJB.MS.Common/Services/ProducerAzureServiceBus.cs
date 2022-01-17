using Microsoft.Extensions.Logging;
using MrJB.MS.Common.Configuration;

namespace MrJB.MS.Common.Services;
    
public class ProducerAzureServiceBus : IProducerService
{
    // services
    ILogger<ProducerAzureServiceBus> _logger;

    // configuration
    AzureServiceBusConsumerConfiguration _azureServiceBusConfiguration;

    public ProducerAzureServiceBus(ILogger<ProducerAzureServiceBus> logger, AzureServiceBusConsumerConfiguration azureServiceBusConfiguration)
    {
        _logger = logger;
        _azureServiceBusConfiguration = azureServiceBusConfiguration;
    }

    public Task ProduceAsync(object message, string queue, string operationId, string parentId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
