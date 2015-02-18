using System.Collections.Generic;
using WebCrawler.Sitemap;

namespace ImageDownloader.Model
{
    public class SiteInformation
    {
        public string Url { get; set; }
        public SiteOptions SiteOptions { get; set; }
        public SitemapNode Sitemap { get; set; }
        public List<string> Files { get; set; }
    }
}
