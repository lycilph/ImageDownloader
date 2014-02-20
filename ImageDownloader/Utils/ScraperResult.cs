using System.Collections.Generic;

namespace ImageDownloader.Utils
{
    public class ScraperResult
    {
        public List<string> Pages { get; private set; }

        public ScraperResult(IEnumerable<string> pages)
        {
            Pages = new List<string>(pages);
        }
    }
}
