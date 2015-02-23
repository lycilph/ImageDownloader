namespace WebCrawler.Sitemap
{
    public class SitemapOptions
    {
        public int MaxThreadCount { get; set; }

        public SitemapOptions()
        {
            MaxThreadCount = 4;
        }
    }
}
