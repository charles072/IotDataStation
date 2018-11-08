using System;

namespace Iot.Common.Log
{
    public interface ILogger
    {
        string ModuleName { get; }

        void Trace(string message);
        void Trace(object obj);
        void Trace(Exception exception, string message);
        void Trace(Exception exception, object obj);
        void Debug(string message);
        void Debug(object obj);
        void Debug(Exception exception, string message);
        void Debug(Exception exception, object obj);
        void Info(string message);
        void Info(object obj);
        void Info(Exception exception, string message);
        void Info(Exception exception, object obj);
        void Warn(string message);
        void Warn(object obj);
        void Warn(Exception exception, string message);
        void Warn(Exception exception, object obj);
        void Error(string message);
        void Error(object obj);
        void Error(Exception exception, string message);
        void Error(Exception exception, object obj);
        void Fatal(string message);
        void Fatal(object obj);
        void Fatal(Exception exception, string message);
        void Fatal(Exception exception, object obj);
    }
}
