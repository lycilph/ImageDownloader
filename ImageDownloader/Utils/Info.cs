using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Utils
{
    public class Info
    {
        public enum StateType { Undefined, Accepted, Rejected };

        public string Item { get; set; }
        public StateType State { get; set; }

        public Info(string item, StateType state)
        {
            Item = item;
            State = state;
        }
    }
}
