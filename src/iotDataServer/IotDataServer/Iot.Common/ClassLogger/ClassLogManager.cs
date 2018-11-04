using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Iot.Common.Log;

namespace Iot.Common.ClassLogger
{
    public class ClassLogManager
    {
        private static readonly Lazy<ClassLogManager> _lazy = new Lazy<ClassLogManager>(() => new ClassLogManager());
        private static ClassLogManager Instance => _lazy.Value;

        private string _logFolder = "log";
        private readonly Dictionary<string, Iot.Common.ClassLogger.ClassLogger> _classLoggers = new Dictionary<string, Iot.Common.ClassLogger.ClassLogger>();
        private LogLevel _consoleLogLevel = LogLevel.Trace;
        private LogLevel _fileLogLevel = LogLevel.Info;

        private ClassLogManager()
        {
            var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            _logFolder = Path.Combine(baseFolder, "logs");
            Directory.CreateDirectory(_logFolder);
        }


        public static Iot.Common.ClassLogger.ClassLogger GetCurrentClassLogger()
        {
            string className = GetClassFullName();
            return Instance.GetCurrentClassLogger(className);
        }

        public static void SetLogLevel(LogLevel consoleLogLevel, LogLevel fileLogLevel)
        {
            Instance.SetLogLevelInternal(consoleLogLevel, fileLogLevel);
        }

        private void SetLogLevelInternal(LogLevel consoleLogLevel, LogLevel fileLogLevel)
        {
            _consoleLogLevel = consoleLogLevel;
            _fileLogLevel = fileLogLevel;
            UpdatedLogLevel();
        }

        private void UpdatedLogLevel()
        {
            foreach (Iot.Common.ClassLogger.ClassLogger classLogger in _classLoggers.Values)
            {
                classLogger.SetConsoleLogLevel(_consoleLogLevel);
                classLogger.SetFileLogLevel(_fileLogLevel);
            }
        }

        private Iot.Common.ClassLogger.ClassLogger GetCurrentClassLogger(string className)
        {
            Iot.Common.ClassLogger.ClassLogger logger = null;
            if (!_classLoggers.TryGetValue(className, out logger))
            {
                logger = new Iot.Common.ClassLogger.ClassLogger(className, _logFolder, _fileLogLevel, _consoleLogLevel);
                _classLoggers.Add(className, logger);
            }
            return logger;
        }

        private static string GetClassFullName()
        {
            string className;
            Type declaringType;
            int framesToSkip = 2;

            do
            {
#if SILVERLIGHT
                StackFrame frame = new StackTrace().GetFrame(framesToSkip);
#else
                StackFrame frame = new StackFrame(framesToSkip, false);
#endif
                MethodBase method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    className = method.Name;
                    break;
                }

                framesToSkip++;
                className = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return className;
        }
    }
}