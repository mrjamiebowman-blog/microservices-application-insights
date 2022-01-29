using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Text.Json;

namespace MrJB.MS.Common.Tests.Helpers
{
    public static class TelemetryHelper
    {
        public static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static TelemetryClient GetFakeTelemetryClient()
        {
            var fakeTelemetryChannel = new FakeTelemetryChannel();

            TelemetryConfiguration configuration = new TelemetryConfiguration
            {
                TelemetryChannel = fakeTelemetryChannel,
                InstrumentationKey = Guid.NewGuid().ToString(),
                DisableTelemetry = true
            };

            TelemetryClient telemetryClient = new TelemetryClient(configuration);
            return telemetryClient;
        }
    }
}
