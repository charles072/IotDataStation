using System;

namespace Iot.Common.Log
{
    public class Logger: LoggerCore
    {
        
        public delegate void LogEventHandler(object sendder, LogEvent logEvent);
        public event LogEventHandler OnLogEvent;
        public LogLevel LogLevel { get; private set; }

        public Logger(string name, LogLevel logLevel = LogLevel.Trace) : base(name)
        {
            LogLevel = logLevel;
        }

        public virtual void SetLogLevel(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public override void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            LogEvent logEvent = new LogEvent(ModuleName, logLevel, message, exception);
            RaiseLogEvent(logEvent);
        }

        protected virtual void RaiseLogEvent(LogEvent logEvent)
        {
            if (LogLevel <= logEvent.LogLevel)
            {
                OnLogEvent?.Invoke(this, logEvent);
            }
        }
    }
}
