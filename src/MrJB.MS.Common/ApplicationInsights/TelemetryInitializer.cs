using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace MrJB.MS.Common.ApplicationInsights
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        private readonly string RoleName;

        public TelemetryInitializer(string roleName)
        {
            RoleName = roleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = RoleName;
        }
    }
}
