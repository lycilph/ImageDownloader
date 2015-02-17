using WebCrawler.Sitemap;

namespace ImageDownloader.Model
{
    public class SiteInformation
    {
        public string Url { get; set; }
        public SitemapNode Sitemap { get; set; }
    }
}
