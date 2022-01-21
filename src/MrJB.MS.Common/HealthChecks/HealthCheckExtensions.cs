using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MrJB.MS.Common.HealthChecks
{
    public static class HealthCheckExtensions
    {
        public static class Tags
        {
            public const string Startup = "Startup";

            public const string Readiness = "Readiness";

            public const string Liveness = "Liveness";
        }

        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { Tags.Startup, Tags.Liveness, Tags.Readiness });

            return services;
        }
    }
}
