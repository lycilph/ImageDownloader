using System;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace ImageDownloader
{
    public class WpfLogTarget : Target
    {
        public IProgress<string> Progress { get; set; }

        protected override void Write(AsyncLogEventInfo async_log_event)
        {
            Write(async_log_event.LogEvent);
        }

        protected override void Write(LogEventInfo log_event)
        {
            switch (log_event.Level.Name)
            {
                case "Trace":
                case "Debug":
                case "Info":
                case "Warn":
                case "Error":
                case "Fatal":
                    Progress.Report(log_event.FormattedMessage);
                    break;
            }
        }
    }
}
