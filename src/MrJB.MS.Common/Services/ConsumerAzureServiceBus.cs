namespace MrJB.MS.Common.Services;

public class ConsumerAzureServiceBus : IConsumerService, IConsumerAzureServiceBus
{
    public ConsumerAzureServiceBus()
    {

    }

    public Task StartConsumingAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task StartReceivingMessagesAsync(string queueOrTopic, string subscriptionName, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task StopConsumingAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
