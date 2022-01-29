using Microsoft.ApplicationInsights.Channel;
using System;
using System.Collections.Generic;

namespace MrJB.MS.Common.Tests.Helpers
{
    public class FakeTelemetryChannel : ITelemetryChannel
    {
        public IList<ITelemetry> Items
        {
            get;
            private set;
        }

        public bool? DeveloperMode { get; set; }

        public string EndpointAddress { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void Send(ITelemetry item)
        {
            // append to items collection
            Items.Add(item);
        }
    }
}
