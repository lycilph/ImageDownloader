using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using ImageDownloader.Data;
using WebCrawler.Crawler;
using WebCrawler.Data;
using WebCrawler.Extensions;
using WebCrawler.LinkExtractor;
using WebCrawler.PageProcessor;
using WebCrawler.PageProvider;
using WebCrawler.Utils;

namespace ImageDownloader.Services
{
    [Export(typeof(CrawlerService))]
    public class CrawlerService
    {
        public async Task<ConcurrentQueue<Page>>  Crawl(string url, SiteOptions site_options, ProcessStatus status)
        {
            var pages = new ConcurrentQueue<Page>();

            var host = url.GetHost();
            var cache = await site_options.CacheTask;
            var crawler_options = new CrawlerOptions
            {
                Url = url,
                MaxThreadCount = Settings.MaxThreadCount,
                ThreadDelay = Settings.ThreadDelay,
                LinkExtractor = new AllInternalLinksExtractor(host),
                PageProcessor = new PageCollector(pages),
                PageProviderFactory = new PageProviderFactory(site_options.UseCache, cache, Settings.UserAgent, Settings.WebRequestTimeout)
            };
            var crawler = new Crawler(crawler_options, status);
            await crawler.Start();

            return pages;
        }
    }
}
