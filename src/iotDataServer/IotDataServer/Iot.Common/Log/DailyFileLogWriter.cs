using System;

namespace Iot.Common.Log
{
    public class DailyFileLogWriter
    {
        public LogLevel LogLevel { get; set; }
        public string LogFolder { get; }
        private readonly DailyFileLogWriterWorker _logWriterWorker;

        public DailyFileLogWriter(string logFolder, LogLevel logLevel = LogLevel.Trace)
        {
            LogFolder = logFolder;
            LogLevel = logLevel;
            _logWriterWorker = DailyFileLogWriterWorker.GetInstance();
            _logWriterWorker.AddLogFolder(LogFolder);
        }

        public void Log(LogEvent logEvent, string fileTitle = "")
        {
            if (LogLevel <= logEvent?.LogLevel)
            {
                LogWrite(logEvent, fileTitle);
            }
        }

        public void LogWrite(LogEvent logEvent, string fileTitle)
        {
            if (string.IsNullOrWhiteSpace(fileTitle))
            {
                fileTitle = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            }
            string logFileName = $"{LogFolder}\\{fileTitle}_{DateTime.Now:yyyy-MM-dd}.log";

            FileLogEvent fileLogEvent = new FileLogEvent(logFileName, logEvent);
	        _logWriterWorker.Enqueue(fileLogEvent);
		}
    }
}
