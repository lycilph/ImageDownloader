using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ImageDownloader.Controllers;
using ImageDownloader.Data;
using Panda.Utilities.Extensions;
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
        private readonly StatusController status_controller;

        [ImportingConstructor]
        public CrawlerService(StatusController status_controller)
        {
            this.status_controller = status_controller;
        }

        public async Task<ConcurrentQueue<Page>> Crawl(string url, SiteOptions site_options, ProcessStatus status, CancellationToken token)
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
            
            var sw = Stopwatch.StartNew();
            await crawler.Start(token);
            sw.Stop();
            status_controller.MainStatusText = string.Format("{0} crawled in {1:F1} sec(s)", url, sw.Elapsed.TotalSeconds);

            return pages;
        }
    }
}
