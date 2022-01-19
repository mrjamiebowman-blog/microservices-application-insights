using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            return services;
        }

        public static IServiceCollection AddCustomApplicationInsightsWorker(this IServiceCollection services, IConfiguration configuration)
        {
            // configuration
            AppInsightsConfiguration config = new AppInsightsConfiguration();
            configuration.GetSection(AppInsightsConfiguration.Position).Bind(config);

            return services;
        }
    }
}
