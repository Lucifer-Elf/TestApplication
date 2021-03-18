using Elastic.CommonSchema.Serilog;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Servize.Utility.Logging

{
    /// <summary>
    /// class is used to initialize various sub "logger" specified in option
    /// </summary>
    public class LoggerOptionsConfiguration
    {
        private static LoggerConfiguration _loggerConfiguration;
        /// <summary>
        /// constructor of class
        /// </summary>
        /// <param name="options"></param>
        public LoggerOptionsConfiguration(LogOptions options)
        {
            _loggerConfiguration = new LoggerConfiguration();
            _loggerConfiguration.Enrich.WithMachineName().Enrich.WithProcessId().Enrich.WithThreadId()
                //.Enrich.WithElasticApmCorrelationInfo()
                .MinimumLevel.Information().Enrich.FromLogContext();


            if (options.EnableConsole)
            {
                EnableConsole();
            }
            if (!string.IsNullOrEmpty(options.FilePath))
            {
                EnableFile(options.FilePath);
            }
            if (!string.IsNullOrEmpty(options.ApplicationInsightInstrumentKey))
            {
                EnableApplicationInsight(options.ApplicationInsightInstrumentKey);
            }
            if (options.EnableElasticSearch)
            {
                EnableElasticSearch(options.ElasticSearchHost);
            }

        }
        /// <summary>
        /// Function set the logger configuration with file path
        /// </summary>
        /// <param name="filePath"></param>
        private void EnableFile(string filePath)
        {
            _loggerConfiguration.WriteTo.File(new EcsTextFormatter(), filePath);
        }
        /// <summary>
        ///  Function set the logger configuration with Application Insight
        /// </summary>
        /// <param name="instrumentKey"></param>
        private void EnableApplicationInsight(string instrumentKey)
        {
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = instrumentKey;
            _loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);

        }
        /// <summary>
        ///  Function set the logger configuration with Console 
        /// </summary>
        private void EnableConsole()
        {
            _loggerConfiguration.WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }
        /// <summary>
        ///  Function set the logger configuration with elastic search using it Host Address
        /// </summary>
        /// <param name="hostAdress"></param>

        private void EnableElasticSearch(string hostAdress = "")
        {
            try
            {
                _loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(hostAdress))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                    IndexFormat = "lisec-logs",
                    CustomFormatter = new EcsTextFormatter(),
                    OverwriteTemplate = true,
                    DetectElasticsearchVersion = true,
                    RegisterTemplateFailure = RegisterTemplateRecovery.FailSink,
                    FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),


                }).MinimumLevel.Information();
            }
            catch (Exception)
            {
                Console.WriteLine("There is no ElasticSearchAvaliable");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>return current log configuration</returns>
        public LoggerConfiguration Configuration()
        {
            return _loggerConfiguration;
        }
    }


}
