using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Servize.Utility.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servize.Utility.Logging

{
    /// <summary>
    /// Class is used to Log message based on serverity of the message
    /// </summary>
    public class Logger
    {
      
       

        /// <summary>
        /// This function is to create Logger builder with all the option avaliable directly instead of using configuration.        /// 
        /// </summary>
        /// <param name="options">options are all the sublogger available </param>
        public static void CreateLogger(LogOptions options)
        {
            LoggerOptionsConfiguration loggerOptions = new LoggerOptionsConfiguration(options);
            Serilog.Log.Logger = loggerOptions.Configuration().CreateLogger();

        }
        /// <summary>
        /// This function is used to close Logger connection and flush all the value present in buffer to the output stream.
        /// </summary>
        public static void Flush()
        {
            Serilog.Log.CloseAndFlush();
        }

        /// <summary>
        /// This function returns statistics log records from statistics record queue.
        /// </summary>
        /// <param name="recordsCount">Specify the record count to be returned. If less than 0 then all records will be returned.</param>
        /// <returns></returns>
        public static List<StatisticsLogRecord> GetStatisticsRecord(int recordsCount = -1)
        {
            return LoggerBuildSingleton.Instance().GetStatisticsRecord(recordsCount);
        }

        /// <summary>
        /// This is to log information message in output stream
        /// </summary>
        /// <param name="logId">A log id</param>
        /// <param name="msg">The message to be logged</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public static void LogInformation(int logId, string msg,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LoggerBuildSingleton.Instance().LogEvent(logId, msg, Level.INFORMATION, null, memberName, Path.GetFileName(fileName), lineNumber);
        }

        /// <summary>
        ///This is to log error message in output stream
        /// </summary>
        /// <param name="logId">A log id</param>
        /// <param name="msg">The message to be logged</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public static void LogError(int logId, string msg,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LoggerBuildSingleton.Instance().LogEvent(logId, msg, Level.ERROR, null, memberName, Path.GetFileName(fileName), lineNumber);
        }

        /// <summary>
        /// This is to log Fatal message in output stream
        /// </summary>
        /// <param name="logId">A log id</param>
        /// <param name="msg">The message to be logged</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public static void LogFatal(int logId, string msg,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LoggerBuildSingleton.Instance().LogEvent(logId, msg, Level.FATAL, null, memberName, Path.GetFileName(fileName), lineNumber);
        }

        /// <summary>
        /// This is to log Warning message in output stream
        /// </summary>
        /// <param name="logId">A log id</param>
        /// <param name="msg">The message to be logged</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public static void LogWarning(int logId, string msg,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LoggerBuildSingleton.Instance().LogEvent(logId, msg, Level.WARNING, null, memberName, Path.GetFileName(fileName), lineNumber);
        }

        /// <summary>
        /// Log an exception with Information severity
        /// </summary>
        /// <param name="e">The exception to log</param>
        /// <param name="msg">Some custom message, if not given then e.ToString() is used</param>
        /// <param name="logId">An optional log id</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public static void LogInformation(Exception e, string msg = "", int logId = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LoggerBuildSingleton.Instance().LogEvent(logId, msg.Trim() == "" ? e.ToString() : msg, Level.INFORMATION, e, memberName, Path.GetFileName(fileName), lineNumber);
        }

        /// <summary>
        /// Log an exception with Error severity
        /// </summary>
        /// <param name="e">The exception to log</param>
        /// <param name="msg">Some custom message, if not given then e.ToString() is used</param>
        /// <param name="logId">An optional log id</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public static void LogError(Exception e, string msg = "", int logId = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LoggerBuildSingleton.Instance().LogEvent(logId, msg.Trim() == "" ? e.ToString() : msg, Level.ERROR, e, memberName, Path.GetFileName(fileName), lineNumber);
        }

        /// <summary>
        /// Log an exception with Fatal severity
        /// </summary>
        /// <param name="e">The exception to log</param>
        /// <param name="msg">Some custom message, if not given then e.ToString() is used</param>
        /// <param name="logId">An optional log id</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public static void LogFatal(Exception e, string msg = "", int logId = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LoggerBuildSingleton.Instance().LogEvent(logId, msg.Trim() == "" ? e.ToString() : msg, Level.FATAL, e, memberName, Path.GetFileName(fileName), lineNumber);
        }

        /// <summary>
        /// Log an exception with Warning severity
        /// </summary>
        /// <param name="e">The exception to log</param>
        /// <param name="msg">Some custom message, if not given then e.ToString() is used</param>
        /// <param name="logId">An optional log id</param>
        /// <param name="memberName">name of the function/method calling this log method</param>
        /// <param name="fileName">file where the function/method calling this log method is defined</param>
        /// <param name="lineNumber">line number in the file where the function/method calling this log method is defined</param>
        public static void LogWarning(Exception e, string msg = "", int logId = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string fileName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            LoggerBuildSingleton.Instance().LogEvent(logId, msg.Trim() == "" ? e.ToString() : msg, Level.WARNING, e, memberName, Path.GetFileName(fileName), lineNumber);
        }

       

        /// <summary>
        /// Function return serilog Ilogger . which can be used for services logging .
        /// </summary>
        /// <returns></returns>
        public static ILogger GetLogger()
        {
            return Serilog.Log.Logger;
        }

    }
}
