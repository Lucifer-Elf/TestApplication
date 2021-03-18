using Serilog;
using Servize.Utility.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servize.Utility.Logging
{

    /// <summary>
    /// Its a singleton class to create Serilog "logger" based on log options specified in configuration.
    /// </summary>
    public class LoggerBuildSingleton
    {
        private static LoggerBuildSingleton _instance;
        private static readonly object _padlock = new object();

        /// <summary>
        /// BUFFER_LOGFILE_PATH  contains path of json file where we want key log backup message untill Logger is configured. 
        /// </summary>
        public static string BUFFER_LOGS_FOLDERNAME = "Logs";

        /// <summary>
        /// Queue to store log records for statistics.
        /// </summary>
        private Queue<StatisticsLogRecord> StatisticsRecordQueue { get; } = new Queue<StatisticsLogRecord>();

        /// <summary>
        /// This function returns statistics log records from statistics record queue.
        /// </summary>
        /// <param name="recordsCount">Specify the record count to be returned. If less than 0 then all records will be returned.</param>
        /// <returns></returns>
        public List<StatisticsLogRecord> GetStatisticsRecord(int recordsCount = -1)
        {
            int count = StatisticsRecordQueue.Count();
            if (recordsCount >= 0)
                count = recordsCount;

            List<StatisticsLogRecord> logRecords = new List<StatisticsLogRecord>();
            foreach (StatisticsLogRecord record in StatisticsRecordQueue.Reverse())
            {
                if (record == null)
                    continue;

                if (count == 0)
                    break;

                count--;
                logRecords.Add(record);
            }

            return logRecords;
        }

        private void UpdateRecordToStatisticsQueue(StatisticsLogRecord statisticsRecord)
        {
            int StatisticsRecordQueueMaxLimit = Configuration.GetValue("base.statistics.logrecordslimit", 100);
            if (StatisticsRecordQueue.Count >= StatisticsRecordQueueMaxLimit)
            {
                // Remove log records above max limit
                while (StatisticsRecordQueue.Count != (StatisticsRecordQueueMaxLimit - 1))
                    StatisticsRecordQueue.Dequeue();
            }

            StatisticsRecordQueue.Enqueue(statisticsRecord);
        }

        /// <summary>
        /// constructor of class which create logger options based on Db configuration availablity 
        /// </summary>
        /// <param name="isDbConfigurationAvailable"></param>
        private LoggerBuildSingleton(bool isDbConfigurationAvailable)
        {
            LogOptions options;
           // if (isDbConfigurationAvailable)
            //{
                options = DefaultConfiguration();
            //}
            ///else
           // {
//options = ConfigureDefaultLogHandler();
           // }
            Logger.CreateLogger(options);
        }

        /// <summary>
        /// this function set basic handler to log builder
        /// </summary>
        /// <returns></returns>
       /* private static LogOptions ConfigureDefaultLogHandler()
        {
            return new LogOptions
            {
                EnableConsole = Configuration.GetValue<bool>("base.logging.console.enable", true),
               // FilePath = Configuration.GetValue<string>("base.logging.file.path", Utility.GetLogFilePath()),
            };
        }*/

        /// <summary>
        /// this function check the setting in database or environment and initlize he log handler
        /// </summary>
        /// <returns></returns>
        private static LogOptions DefaultConfiguration()
        {
            return new LogOptions
            {
                EnableConsole = Configuration.GetValue<bool>("ConsoleEnable", true),
                ApplicationInsightInstrumentKey = Configuration.GetValue<string>("applicationinsightInstrumentkey"),
                //EnableElasticSearch = Configuration.GetValue<bool>("base.logging.elasticsearch.enable", false),
                //ElasticSearchHost = Configuration.GetValue<string>("base.logging.elasticsearch.host", "http://localhost:9200/"),
                //FilePath = Configuration.GetValue<string>("base.logging.file.path"),
            };
        }

        /// <summary>
        /// create instance of LoggerSingleton with options based on DbConfiguration availablity 
        /// </summary>
        /// <param name="isDbConfigurationAvaliable"></param>
        /// <returns></returns>
        public static LoggerBuildSingleton Instance(bool isDbConfigurationAvaliable = false)
        {
            lock (_padlock)
            {
                if (_instance == null)
                    _instance = new LoggerBuildSingleton(isDbConfigurationAvaliable);
                return _instance;
            }
        }
        /// <summary>
        /// Relase the instance of this and set it to null
        /// </summary>

        public static void ReleaseInstance()
        {
            _instance = null;
        }

        /// <summary>
        /// function logs the event revieved from variaous class to serilog "logger" based on LogLevel or criteria
        /// </summary>
        /// <param name="logId">An id to correlate some meta information to</param>
        /// <param name="msg">the message to log</param>
        /// <param name="logLevel">severity level</param>
        /// <param name="e">Exception to log</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public void LogEvent(int logId, string msg, Level logLevel, Exception e = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LogRecord record = new LogRecord
            {
                LogId = logId,
                Message = msg,
                FunctionName = memberName,
                LogLevel = logLevel,
                LogLevelId = logLevel.GetHashCode(),
                FileName = fileName,
                LineNumber = lineNumber
            };

            string stackTrace = null;
            if (e == null)
            {
                LogEvent(logLevel, record);
            }
            else
            {
                LogEvent(logLevel, e, record);
                stackTrace = e.StackTrace;
            }

            StatisticsLogRecord statisticsRecord = new StatisticsLogRecord
            {
                LogRecord = record,
                StackTrace = stackTrace
            };

            if (logLevel == Level.FATAL || logLevel == Level.ERROR)
                UpdateRecordToStatisticsQueue(statisticsRecord);
        }

        private static void LogEvent(Level logLevel, Exception e, LogRecord record)
        {
            switch (logLevel)
            {
                case Level.FATAL:
                    Log.Fatal(e, "{@holder}", record);
                    break;
                case Level.WARNING:
                    Log.Warning(e, "{@holder}", record);
                    break;
                case Level.INFORMATION:
                    Log.Information(e, "{@holder}", record);
                    break;
                case Level.ERROR:
                    Log.Error(e, "{@holder}", record);
                    break;
            }
        }

        private static void LogEvent(Level logLevel, LogRecord record)
        {
            switch (logLevel)
            {
                case Level.FATAL:
                    Log.Fatal("{@holder}", record);
                    break;
                case Level.WARNING:
                    Log.Warning("{@holder}", record);
                    break;
                case Level.INFORMATION:
                    Log.Information("{@holder}", record);
                    break;
                case Level.ERROR:
                    Log.Error("{@holder}", record);
                    break;
            }
        }
    }
}
