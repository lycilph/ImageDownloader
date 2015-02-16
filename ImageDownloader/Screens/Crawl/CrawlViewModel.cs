using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using ImageDownloader.Controllers;
using ImageDownloader.Model;
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

        public CrawlViewModel(ApplicationController controller) : base(controller)
        {
            DisplayName = "Crawl";

            Crawlers = Enumerable.Range(1, Settings.MaxThreadCount)
                                 .Select(i => new CrawlerViewModel {DisplayName = "Crawler " + i})
                                 .ToReactiveList();
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Url = controller.Selection.Text;
            Text = "Initializing crawler";
            controller.Shell.MainStatusText = "Crawling " + Url;

            var start_time = DateTime.Now;
            var timer = new DispatcherTimer();
            timer.Tick += (o, a) => controller.Shell.AuxiliaryStatusText = Math.Round((DateTime.Now - start_time).TotalSeconds, 1).ToString("N1") + " sec(s)";
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();

            var options = new CrawlerOptions
            {
                DataFolder = controller.Settings.DataFolder,
                MaxThreadCount = Settings.MaxThreadCount,
            };
            var progress = new CrawlerProgress
            {
                Progress = new Progress<string>(str => Text = str),
                TaskProgress = Crawlers.Select(c => new Progress<string>(str => c.Text = str)).Cast<IProgress<string>>().ToList()
            };
            var link_extractor = new AllInternalLinksExtractor(Url.GetHost());
            var page_processor = new SitemapPageProcessor();
            
            controller.Shell.IsBusy = true;

            // Setup SitemapGenerator

            await Task.Factory.StartNew(() =>
            {
                var sw = Stopwatch.StartNew();
                using (var crawler = new Crawler(Url, options, progress, link_extractor, page_processor))
                {
                    crawler.Crawl().Wait();
                    progress.Report("Finalizing crawler");
                }
                var elapsed = sw.StopAndGetElapsedMilliseconds();
                controller.Shell.MainStatusText = string.Format("Crawled {0} in {1} ms", Url, elapsed);
            });

            controller.Shell.IsBusy = false;

            timer.Stop();

            Text = "Crawler done";
            await Task.Delay(Settings.ScreenTransitionDelay);
            controller.Main.Next();
        }
    }
}
