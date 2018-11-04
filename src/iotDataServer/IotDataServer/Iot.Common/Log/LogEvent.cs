using System;
using Iot.Common.Util;

namespace Iot.Common.Log
{
    public class LogEvent
	{
	    public DateTime TimeStampUtc { get; set; }
	    public LogLevel LogLevel { get; set; }
	    public string ModuleName { get; set; }
		public string Message { get; set; }
	    public Exception Exception { get; set; }

        public LogEvent(string moduleName, LogLevel logLevel, string message, Exception exception = null)
        {
			TimeStampUtc = CachedDateTime.UtcNow;
            ModuleName = moduleName;
			LogLevel = logLevel;
			Message = message;
			Exception = exception;
		}

        public LogEvent(LogEvent logEvent): this(logEvent.ModuleName, logEvent.LogLevel, logEvent.Message, logEvent.Exception)
        {
            
        }
    }

    public class FileLogEvent
    {
        public string Filename { get; }
        public LogEvent LogEvent { get; }

        public FileLogEvent(string filename, LogEvent logEvent)
        {
            Filename = filename;
            LogEvent = logEvent;
        }
    }
}
