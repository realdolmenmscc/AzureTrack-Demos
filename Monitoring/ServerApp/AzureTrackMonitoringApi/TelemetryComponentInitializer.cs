using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace AzureTrackMonitoringApi
{
    public class TelemetryComponentInitializer : ITelemetryInitializer
    {
        /// <summary>
        /// Name of the component.
        /// </summary>
        public string ComponentName { get; set; } = "MyDemoApi";


        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is ISupportProperties properties)
            {

                properties.Properties["component"] = ComponentName;
            }
        }
    }
}
