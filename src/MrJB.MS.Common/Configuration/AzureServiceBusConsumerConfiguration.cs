namespace MrJB.MS.Common.Configuration;
    
public class AzureServiceBusConsumerConfiguration : BaseAzrueServiceBusConfiguration
{
    public const string Position = "AzureServiceBus:Consumer";

    public string SubscriptionName { get; set; }
}
