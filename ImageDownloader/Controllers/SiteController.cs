using System.Threading.Tasks;
using ImageDownloader.Model;
using ImageDownloader.Sitemap;
using ImageDownloader.Utilities;
using Panda.Utilities.Extensions;
using Panda.WebCrawler;
using Panda.WebCrawler.Extensions;

namespace ImageDownloader.Controllers
{
    public class SiteController
    {
        private readonly CrawlerOptions options;

        public string Url { get; set; }
        public SitemapNode Sitemap { get; set; }

        public SiteController(ApplicationController controller)
        {
            options = new CrawlerOptions
            {
                DataFolder = controller.Settings.DataFolder,
                MaxThreadCount = Settings.MaxCrawlerThreadCount,
            };
        }

        public Task Process(ProcessingProgress progress)
        {
            var link_extractor = new AllInternalLinksExtractor(Url.GetHost());
            var page_processor = new SitemapPageProcessor(Url, progress.SitemapProgress);

            var sitemap_task = page_processor.Generate();
            Task.Factory.StartNew(() =>
            {
                using (var crawler = new Crawler(Url, options, progress, link_extractor, page_processor))
                {
                    crawler.Crawl().Wait();
                }
                page_processor.CompleteAdding();
            });

            return sitemap_task.ContinueWith(parent => Sitemap = parent.Result);
        }

        public void Save(string filename)
        {
            JsonExtensions.ZipAndWriteToFile(filename, Sitemap);
        }

        public void Load(string filename)
        {
            Sitemap = JsonExtensions.ReadFromFileAndUnzip<SitemapNode>(filename);
        }
    }
}
