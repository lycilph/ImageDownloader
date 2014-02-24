using System.Collections.Generic;

namespace ImageDownloader.Utils
{
    public class Result
    {
        public List<string> Items { get; private set; }

        public Result(IEnumerable<string> items)
        {
            Items = new List<string>(items);
        }
    }
}
