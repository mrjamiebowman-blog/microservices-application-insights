namespace MrJB.MS.Common.Configuration;

public abstract class BaseAzrueServiceBusConfiguration
{
    public string ConnectionString { get; set; }

    public string QueueOrTopic { get; set; }

    public string SubscriptionName { get; set; }
}
