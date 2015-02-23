using WebCrawler.LinkExtractor;
using WebCrawler.PageProcessor;
using WebCrawler.PageProvider;

namespace WebCrawler.Crawler
{
    public class CrawlerOptions
    {
        public string Url { get; set; }
        public int MaxThreadCount { get; set; }
        public int ThreadDelay { get; set; }
        public ILinkExtractor LinkExtractor { get; set; }
        public IPageProcessor PageProcessor { get; set; }
        public IPageProviderFactory PageProviderFactory { get; set; }
    }
}
