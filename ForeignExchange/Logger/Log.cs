using Microsoft.ApplicationInsights;
using System;

namespace ForeignExchange.Logger
{
    public class Log
    {
        public readonly TelemetryClient _telemetry;

        public Log(TelemetryClient telemetryClient)
        {
            _telemetry = telemetryClient;
        }

        public void TrackException(Exception exception)
        {
            _telemetry.TrackException(exception);
        }

        public void TrackTrace(string message)
        {
            _telemetry.TrackTrace($"Message : {message}");
        }
    }
}