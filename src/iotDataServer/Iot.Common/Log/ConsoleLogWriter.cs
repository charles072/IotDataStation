using System;

namespace Iot.Common.Log
{
    public class ConsoleLogWriter
    {
        protected const ConsoleColor TraceColor = ConsoleColor.DarkGray;
        protected const ConsoleColor DebugColor = ConsoleColor.Gray;
        protected const ConsoleColor InfoColor = ConsoleColor.Green;
        protected const ConsoleColor WarnColor = ConsoleColor.DarkYellow;
        protected const ConsoleColor ErrorColor = ConsoleColor.DarkRed;
        protected const ConsoleColor FatalColor = ConsoleColor.Red;

        public LogLevel LogLevel { get; set; }

        public ConsoleLogWriter(LogLevel logLevel=LogLevel.Trace)
        {
            LogLevel = logLevel;
        }
        public void Log(LogEvent logEvent)
        {
            if (LogLevel <= logEvent?.LogLevel)
            {
                LogWrite(logEvent);
            }
        }

        private void LogWrite(LogEvent logEvent)
        {
            Console.ForegroundColor = GetForegroundColor(logEvent.LogLevel);
            Console.WriteLine($"{logEvent.TimeStampUtc.ToLocalTime():HH:mm:ss}: [{logEvent.ModuleName}][{logEvent.LogLevel}] {logEvent.Message}");

            if (logEvent.Exception != null)
            {
                Console.WriteLine(logEvent.Exception.ToString());
            }
            Console.ResetColor();
        }

        private ConsoleColor GetForegroundColor(LogLevel logLevel)
        {
            ConsoleColor resColor = Console.ForegroundColor;
            switch (logLevel)
            {
                case LogLevel.Trace:
                    resColor = TraceColor;
                    break;
                case LogLevel.Debug:
                    resColor = DebugColor;
                    break;
                case LogLevel.Info:
                    resColor = InfoColor;
                    break;
                case LogLevel.Warn:
                    resColor = WarnColor;
                    break;
                case LogLevel.Error:
                    resColor = ErrorColor;
                    break;
                case LogLevel.Fatal:
                    resColor = FatalColor;
                    break;
            }

            return resColor;
        }

    }
}
