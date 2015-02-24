using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ImageDownloader.Controllers;
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
        private readonly StatusController status_controller;

        [ImportingConstructor]
        public SitemapService(StatusController status_controller)
        {
            this.status_controller = status_controller;
        }

        public async Task<SitemapNode> Build(string url, ConcurrentQueue<Page> pages, ProcessStatus status, CancellationToken token)
        {
            var options = new SitemapOptions
            {
                MaxThreadCount = Settings.MaxThreadCount,
                ThreadDelay = Settings.ThreadDelay,
                LinkExtractor = new AllInternalFilesExtractor(url.GetHost())
            };
            var builder = new SitemapBuilder(url, options, status);

            var sw = Stopwatch.StartNew();
            var sitemap = await builder.Build(pages, token);
            sw.Stop();
            status_controller.MainStatusText = string.Format("Sitemap build for {0} in {1:F1} sec(s)", url, sw.Elapsed.TotalSeconds);

            return sitemap;
        }
    }
}
