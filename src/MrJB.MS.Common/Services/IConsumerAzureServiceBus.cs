namespace MrJB.MS.Common.Services;

public interface IConsumerAzureServiceBus
{
    Task StartReceivingMessagesAsync(string queueOrTopic, string subscriptionName, CancellationToken token);
}
