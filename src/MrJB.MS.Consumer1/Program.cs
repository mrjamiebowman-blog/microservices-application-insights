using Microsoft.ApplicationInsights.WorkerService;
using MrJB.MS.Common.Configuration;
using MrJB.MS.Common.Extensions;
using MrJB.MS.Consumer1;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        // app insights
        var options = new ApplicationInsightsServiceOptions();
        options.EnableHeartbeat = true;
        options.InstrumentationKey = builder.Configuration.GetValue<string>($"{AppInsightsConfiguration.Position}:{nameof(AppInsightsConfiguration.InstrumentationKey)}");
        services.AddApplicationInsightsTelemetryWorkerService(options);
        services.AddCustomApplicationInsightsWorker(builder.Configuration);

        services.AddAzureServiceBusConsumerConfiguration(builder.Configuration);
        services.AddAzureServiceBusProducerConfiguration(builder.Configuration);
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
