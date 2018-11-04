using System;
using Iot.Common.Log;

namespace Iot.Common.ClassLogger
{
    public class ClassLogger : LoggerCore
    {
        private readonly ConsoleLogWriter _consoleLogWriter;
        private readonly DailyFileLogWriter _dailyFileLogWriter;

        public delegate void LogEventHandler(ClassLogger logger, LogEvent logEvent);
        public event LogEventHandler OnLogEvent;
        public string LogFolder { get; }
        public LogLevel ConsoleLogLevel => _consoleLogWriter.LogLevel;
        public LogLevel FileLogLevel => _dailyFileLogWriter.LogLevel;

        public ClassLogger(string moduleName, string logFolder = "logs", LogLevel fileLogLevel = LogLevel.Trace, LogLevel consoleLogLevel = LogLevel.Trace) : base(moduleName)
        {
            _dailyFileLogWriter = new DailyFileLogWriter(logFolder, fileLogLevel);
            _consoleLogWriter = new ConsoleLogWriter(consoleLogLevel);
        }

        public virtual void SetFileLogLevel(LogLevel logLevel)
        {
            _dailyFileLogWriter.LogLevel = logLevel;
        }
        public virtual void SetConsoleLogLevel(LogLevel logLevel)
        {
            _consoleLogWriter.LogLevel = logLevel;
        }

        public override void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            LogEvent logEvent = new LogEvent(ModuleName, logLevel, message, exception);

            _consoleLogWriter.Log(logEvent);
            _dailyFileLogWriter.Log(logEvent);
            RaiseLogEvent(logEvent);
        }

        protected virtual void RaiseLogEvent(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                return;
            }
            OnLogEvent?.Invoke(this, logEvent);
        }
    }
}