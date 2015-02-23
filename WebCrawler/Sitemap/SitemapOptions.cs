using WebCrawler.LinkExtractor;

namespace WebCrawler.Sitemap
{
    public class SitemapOptions
    {
        public int MaxThreadCount { get; set; }
        public int ThreadDelay { get; set; }
        public ILinkExtractor LinkExtractor { get; set; }
    }
}
