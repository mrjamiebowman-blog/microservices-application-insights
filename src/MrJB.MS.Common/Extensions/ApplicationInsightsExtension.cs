﻿using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrJB.MS.Common.ApplicationInsights;
using MrJB.MS.Common.ApplicationInsights.Filters;
using MrJB.MS.Common.Configuration;

namespace MrJB.MS.Common.Extensions
{
    public static class ApplicationInsightsExtension
    {
        public static IServiceCollection AddCustomApplicationInsightsApi(this IServiceCollection services, IConfiguration configuration)
        {
            // configuration
            AppInsightsConfiguration config = new AppInsightsConfiguration();
            configuration.GetSection(AppInsightsConfiguration.Position).Bind(config);

            // telemetry initializer
            services.AddSingleton<ITelemetryInitializer>(new TelemetryInitializer(config.RoleName));

            return services;
        }

        public static IServiceCollection AddCustomApplicationInsightsWorker(this IServiceCollection services, IConfiguration configuration)
        {
            // configuration
            AppInsightsConfiguration config = new AppInsightsConfiguration();
            configuration.GetSection(AppInsightsConfiguration.Position).Bind(config);

            // telemetry initializer
            services.AddSingleton<ITelemetryInitializer>(new TelemetryInitializer(config.RoleName));

            return services;
        }
    }
}
