namespace Servize.Utility.Logging

{
    /// <summary>
    /// Class for StatisticsLogRecord. This class contains log record and stack trace related details.
    /// </summary>
    public class StatisticsLogRecord
    {
        /// <summary>
        /// Specify the log record details
        /// </summary>
        public LogRecord LogRecord { get; set; }

        /// <summary>
        /// Specify the stack trace details
        /// </summary>
        public string StackTrace { get; set; }
    }
}
