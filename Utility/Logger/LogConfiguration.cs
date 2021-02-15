using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

using System;
namespace Servize.Utility.Logger
{
    public class LogConfiguration
    {
        private static LoggerConfiguration _loggerConfiguration;

        public static LoggerConfiguration GetConfiguration()
        {
            if (_loggerConfiguration == null)
                _loggerConfiguration = new LoggerConfiguration();

            _loggerConfiguration
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .MinimumLevel.Information().Enrich.FromLogContext();
    
            AddConsole();

            return _loggerConfiguration;
        }

        private void AddApplicationInsightSink(string instrumentKey)
        {
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = instrumentKey;
            _loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);

        }

        private static void AddConsole()
        {
            _loggerConfiguration.WriteTo.Console(outputTemplate:
          "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }
    }
}
