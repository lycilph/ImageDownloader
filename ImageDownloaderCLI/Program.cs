using Core;
using System;

namespace ImageDownloaderCLI
{
    class Program
    {
        static void Main()
        {
            CrawlSite();
            //LoadSite();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void LoadSite()
        {
            const string filename = @"C:\Private\Projects\ImageDownloader\Data\site.data";

            var site = JsonExtensions.ReadFromFileAndUnzip<Site>(filename);
            var node = SiteAnalyzer.CreateSiteMap(site);
        }

        private static void CrawlSite()
        {
            const string filename = @"C:\Private\Projects\ImageDownloader\Data\cache.data";

            using (var page_provider = new WebPageProvider())
            using (var cache = new CachedPageProvider(filename, page_provider))
            using (var crawler = new SiteCrawler(cache))
            {
                var site = crawler.Crawl("http://www.skovboernehave.dk");

                Console.WriteLine("Main Page:");
                Console.WriteLine(site.MainPage);

                Console.WriteLine("Cache hit: " + cache.CacheHit);
                Console.WriteLine("Cache miss: " + cache.CacheMiss);
            }
        }
    }
}
