using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using ImageDownloader.Data;
using WebCrawler.Data;
using WebCrawler.Extensions;
using WebCrawler.LinkExtractor;
using WebCrawler.Sitemap;
using WebCrawler.Utils;

namespace ImageDownloader.Services
{
    [Export(typeof(SitemapService))]
    public class SitemapService
    {
        public Task<SitemapNode> Build(string url, ConcurrentQueue<Page> pages, ProcessStatus status)
        {
            var options = new SitemapOptions
            {
                MaxThreadCount = Settings.MaxThreadCount,
                ThreadDelay = Settings.ThreadDelay,
                LinkExtractor = new AllInternalFilesExtractor(url.GetHost())
            };
            var builder = new SitemapBuilder(url, options, status);
            return builder.Build(pages);
        }
    }
}
