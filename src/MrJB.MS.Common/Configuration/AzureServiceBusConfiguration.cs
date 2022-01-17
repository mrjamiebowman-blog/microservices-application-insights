namespace MrJB.MS.Common.Configuration;
    
public class AzureServiceBusConfiguration
{
    public const string Position = "AzureServiceBus";

    public string ConnectionString { get; set; }

    public string QueueOrTopic { get; set; }

    public string SubscriptionName { get; set; }
}
