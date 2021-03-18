
namespace Servize.Utility.Logging

{
    /// <summary>
    ///  Class contains all the options for serilog "logger"
    /// </summary>
    public class LogOptions
    {
        /// <summary>
        /// Enable sub log for console
        /// </summary>
        public bool EnableConsole { get; set; }
        /// <summary>
        /// Application insight key need to specify to Enable
        /// </summary>
        public string ApplicationInsightInstrumentKey { get; set; }
        /// <summary>
        /// true or false to enable elastic search
        /// </summary>
        public bool EnableElasticSearch { get; set; }
        /// <summary>
        /// specify file path to enable file logger 
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// specify elastic search Host address
        /// </summary>
        public string ElasticSearchHost { get; set; }


    }
}
