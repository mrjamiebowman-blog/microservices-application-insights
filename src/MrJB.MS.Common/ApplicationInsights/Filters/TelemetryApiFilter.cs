using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace MrJB.MS.Common.ApplicationInsights.Filters
{
    public class TelemetryApiFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        static readonly List<string> names = new List<string>
        {
            "favicon.ico",
            "GET /docs/index.html",
            "/swagger/v1/swagger.json"
        };

        public TelemetryApiFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetryProcessor item)
        {
            Next = Next;
        }

        public void Process(ITelemetry item)
        {
            var req = item as RequestTelemetry;

            if (req != null && names.Any(x => req.Name.Contains(x)) && req.Success == true)
            {
                return;
            }

            // send on
            this.Next.Process(item);
        }
    }
}
