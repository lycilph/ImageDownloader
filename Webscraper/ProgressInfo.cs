using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webscraper
{
    public class ProgressInfo
    {
        public string url;
        public bool accepted;

        public ProgressInfo(string url, bool accepted)
        {
            this.url = url;
            this.accepted = accepted;
        }

        public static ProgressInfo CreateAcceptedInfo(string url)
        {
            return new ProgressInfo(url, true);
        }

        public static ProgressInfo CreateRejectedInfo(string url)
        {
            return new ProgressInfo(url, false);
        }
    }
}
