using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Services;

namespace MrJB.MS.Common.Extensions;

public static class AzureServiceBusExtensions
{
    public static IServiceCollection AddCustomAzureServiceBus(this IServiceCollection services)
    {

        return services;
    }

    public static IServiceCollection AddAzureServiceBusConsumerConfiguration(this IServiceCollection services, IConfiguration config)
    {
        // consumer configuration
        AzureServiceBusConsumerConfiguration azureServiceBusConfiguration = new AzureServiceBusConsumerConfiguration();
        config.GetSection(AzureServiceBusConsumerConfiguration.Position).Bind(azureServiceBusConfiguration);
        services.AddSingleton<AzureServiceBusConsumerConfiguration>(azureServiceBusConfiguration);

        // services
        services.AddTransient<IConsumerService, ConsumerAzureServiceBus>();

        return services;
    }

    public static IServiceCollection AddAzureServiceBusProducerConfiguration(this IServiceCollection services, IConfiguration config)
    {
        // producer configuration
        AzureServiceBusProducerConfiguration azureServiceBusConfiguration = new AzureServiceBusProducerConfiguration();
        config.GetSection(AzureServiceBusProducerConfiguration.Position).Bind(azureServiceBusConfiguration);
        services.AddSingleton<AzureServiceBusProducerConfiguration>(azureServiceBusConfiguration);

        // services
        services.AddTransient<IProducerService, ProducerAzureServiceBus>();

        return services;
    }
}
