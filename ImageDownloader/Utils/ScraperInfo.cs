using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Utils
{
    public class ScraperInfo
    {
        public enum StateType { Undefined, Accepted, Rejected };

        public string Item { get; set; }
        public StateType State { get; set; }

        public ScraperInfo(string item, StateType state)
        {
            Item = item;
            State = state;
        }
    }
}
