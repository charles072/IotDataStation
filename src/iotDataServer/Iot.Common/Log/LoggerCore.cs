using System;

namespace Iot.Common.Log
{
    public abstract class LoggerCore: ILogger
    {
        public string ModuleName { get; protected set; }


        protected LoggerCore(string moduleName)
        {
            ModuleName = moduleName;
        }

        public abstract void Log(LogLevel logLevel, string message, Exception exception = null);

        public void Trace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        public void Trace(object obj)
        {
            Log(LogLevel.Trace, obj.ToString());
        }

        public void Trace(Exception exception, string message)
        {
            Log(LogLevel.Trace, message, exception);
        }

        public void Trace(Exception exception, object obj)
        {
            Log(LogLevel.Trace, obj.ToString(), exception);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Debug(object obj)
        {
            Log(LogLevel.Debug, obj.ToString());
        }

        public void Debug(Exception exception, string message)
        {
            Log(LogLevel.Debug, message, exception);
        }

        public void Debug(Exception exception, object obj)
        {
            Log(LogLevel.Debug, obj.ToString(), exception);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Info(object obj)
        {
            Log(LogLevel.Info, obj.ToString());
        }

        public void Info(Exception exception, string message)
        {
            Log(LogLevel.Info, message, exception);
        }

        public void Info(Exception exception, object obj)
        {
            Log(LogLevel.Info, obj.ToString(), exception);
        }

        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }

        public void Warn(object obj)
        {
            Log(LogLevel.Warn, obj.ToString());
        }

        public void Warn(Exception exception, string message)
        {
            Log(LogLevel.Warn, message, exception);
        }

        public void Warn(Exception exception, object obj)
        {
            Log(LogLevel.Warn, obj.ToString(), exception);
        }

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Error(object obj)
        {
            Log(LogLevel.Error, obj.ToString());
        }

        public void Error(Exception exception, string message)
        {
            Log(LogLevel.Error, message, exception);
        }

        public void Error(Exception exception, object obj)
        {
            Log(LogLevel.Error, obj.ToString(), exception);
        }

        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        public void Fatal(object obj)
        {
            Log(LogLevel.Fatal, obj.ToString());
        }

        public void Fatal(Exception exception, string message)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        public void Fatal(Exception exception, object obj)
        {
            Log(LogLevel.Fatal, obj.ToString(), exception);
        }
    }
}
