using System;
using Panda.WebCrawler;

namespace ImageDownloader.Utilities
{
    public class ProcessingProgress : CrawlerProgress
    {
        public IProgress<string> SitemapProgress { get; set; }
    }
}
