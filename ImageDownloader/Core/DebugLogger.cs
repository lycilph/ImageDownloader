using Caliburn.Micro;
using System;
using System.Diagnostics;

namespace ImageDownloader.Core
{
    public enum LogLevel { Info, Warn, Error, None };

    public class DebugLogger : ILog
    {
        private readonly Type _type;
        private readonly LogLevel log_level;

        public DebugLogger(Type type, LogLevel level = LogLevel.Info)
        {
            _type = type;
            log_level = level;
        }

        private string CreateLogMessage(string format, params object[] args)
        {
            return string.Format("[{0}] {1}", DateTime.Now.ToShortTimeString(), string.Format(format, args));
        }

        public void Error(Exception exception)
        {
            Debug.WriteLine(CreateLogMessage(exception.ToString()), "ERROR");
        }

        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "INFO");
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "WARN");
        }
    }
}
