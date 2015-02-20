using WebCrawler.Data;
using WebCrawler.LinkExtractor;
using WebCrawler.PageProcessor;

namespace WebCrawler.Crawler
{
    public class CrawlerOptions
    {
        public string Url { get; set; }
        public bool UseCache { get; set; }
        public Cache Cache { get; set; }
        public int MaxThreadCount { get; set; }
        public int ThreadDelay { get; set; }
        public string UserAgent { get; set; }
        public int RequestTimeout { get; set; }
        public ILinkExtractor LinkExtractor { get; set; }
        public IPageProcessor PageProcessor { get; set; }
    }
}
