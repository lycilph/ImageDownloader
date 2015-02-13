using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImageDownloader.Model;
using ImageDownloader.Screens.Main;
using ImageDownloader.Shell;
using ImageDownloader.Sitemap;
using Panda.ApplicationCore.Extensions;
using Panda.Utilities.Extensions;
using Panda.WebCrawler;
using Panda.WebCrawler.Extensions;
using ReactiveUI;

namespace ImageDownloader.Screens.Crawl
{
    public sealed class CrawlViewModel : BaseViewModel
    {
        private readonly MainViewModel main;

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set { this.RaiseAndSetIfChanged(ref _Url, value); }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private ReactiveList<CrawlerViewModel> _Crawlers;
        public ReactiveList<CrawlerViewModel> Crawlers
        {
            get { return _Crawlers; }
            set { this.RaiseAndSetIfChanged(ref _Crawlers, value); }
        }

        public CrawlViewModel(Settings settings, ShellViewModel shell, MainViewModel main) : base(settings, shell)
        {
            this.main = main;
            DisplayName = "Crawl";

            Crawlers = Enumerable.Range(1, Settings.MaxThreadCount)
                                 .Select(i => new CrawlerViewModel {DisplayName = "Crawler " + i})
                                 .ToReactiveList();
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Url = shell.Selection.Text;
            Text = "Initializing crawler";
            shell.MainStatusText = "Crawling " + Url;

            var options = new CrawlerOptions
            {
                DataFolder = settings.DataFolder,
                MaxThreadCount = Settings.MaxThreadCount,
            };
            var progress = new CrawlerProgress
            {
                Progress = new Progress<string>(str => Text = str),
                TaskProgress = Crawlers.Select(c => new Progress<string>(str => c.Text = str)).Cast<IProgress<string>>().ToList()
            };
            var link_extractor = new AllInternalLinksExtractor(Url.GetHost());
            var page_processor = new SitemapProcessor();
            
            shell.IsBusy = true;
            await Task.Factory.StartNew(() =>
            {
                var sw = Stopwatch.StartNew();
                using (var crawler = new Crawler(Url, options, progress, link_extractor, page_processor))
                {
                    crawler.Crawl().Wait();
                    progress.Report("Finalizing crawler");
                }
                var elapsed = sw.StopAndGetElapsedMilliseconds();
                shell.MainStatusText = string.Format("Crawled {0} in {1} ms", Url, elapsed);
            });
            shell.IsBusy = false;

            Text = "Crawler done";
            await Task.Delay(2000);
            main.Next();
        }
    }
}
