
namespace Servize.Utility.Logging

{
    /// <summary>
    /// This class contains all the attribute specified for Log records
    /// </summary>
    public class LogRecord
    {
        /// <summary>
        /// Specify log id of log record
        /// </summary>
        public int LogId { get; set; }

        /// <summary>
        /// Specify log level id of log record
        /// </summary>
        public int LogLevelId { get; set; }

        /// <summary>
        /// Specify message or information to be provided in log record
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Specify function name from where log is triggered
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// Specify severity level of Log
        /// </summary>
        public Level LogLevel { get; set; }

        /// <summary>
        /// Specify file name from where log is triggered
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Specify line number from where log is triggered
        /// </summary>
        public int LineNumber { get; set; }
    }
}
