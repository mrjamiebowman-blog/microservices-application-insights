namespace MrJB.MS.Common.Services;

public class ConsumerAzureServiceBus : IConsumerAzureServiceBus
{
    public ConsumerAzureServiceBus()
    {

    }

    public Task StartReceivingMessagesAsync(string queueOrTopic, string subscriptionName, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
