using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace MrJB.MS.Common.ApplicationInsights.Filters
{
    public class TelemetryFilterProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;
        private readonly HashSet<string> _excludedTelemetryName;

        public TelemetryFilterProcessor(ITelemetryProcessor next, HashSet<string> excludedTelemetryNames = null)
        {
            _next = next;
            _excludedTelemetryName = excludedTelemetryNames ?? new HashSet<string>();
            _excludedTelemetryName.Add("ServiceBusReceiver.Receive");
        }

        public void Process(ITelemetry item)
        {
            if (OkToSend(item))
            {
                _next.Process(item);
            }
        }

        private bool OkToSend(ITelemetry item)
        {
            if (!(item is RequestTelemetry requestTelemetry))
            {
                return true;
            }

            return !_excludedTelemetryName.Contains(requestTelemetry.Name);
        }
    }
}
