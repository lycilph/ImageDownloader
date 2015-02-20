using System;
using System.Collections.Generic;
using System.Linq;

namespace WebCrawler.Utils
{
    public class ProcessStatus
    {
        public IProgress<string> OverallProgress { get; set; }
        public List<IProgress<string>> TaskProgress { get; set; }

        public IProgress<string> this[int i]
        {
            get
            {
                if (!TaskProgress.Any() || i >= TaskProgress.Count)
                    throw new ArgumentException();
                return TaskProgress[i];
            }
        }

        public ProcessStatus()
        {
            OverallProgress = new Progress<string>();
            TaskProgress = new List<IProgress<string>>();
        }

        public void Report(string str)
        {
            OverallProgress.Report(str);
        }

        public void Report(int i, string str)
        {
            this[i].Report(str);
        }
    }
}
