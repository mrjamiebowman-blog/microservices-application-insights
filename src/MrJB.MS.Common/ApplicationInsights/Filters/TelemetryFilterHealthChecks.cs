using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace MrJB.MS.Common.ApplicationInsights.Filters
{
    public class TelemetryFilterHealthChecks : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        static readonly List<string> urls = new List<string>
        {
            "/hc/live",
            "/hc/ready",
            "/hc/startup"
        };

        public TelemetryFilterHealthChecks(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            var req = item as RequestTelemetry;

            if (req == null)
            {
                return;
            }

            // we don't want to ignore failed health checks.
            if (req.Url != null && urls.Any(x => req.Url.LocalPath.ToLower().Contains(x)) && req.Success == true)
            {
                return;
            }

            // send on
            this.Next.Process(item);
        }
    }
}
