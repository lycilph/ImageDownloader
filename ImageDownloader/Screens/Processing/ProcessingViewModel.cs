using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using ImageDownloader.Controllers;
using ImageDownloader.Model;
using ImageDownloader.Utilities;
using Panda.ApplicationCore.Extensions;
using ReactiveUI;
using WebCrawler;
using WebCrawler.Crawler;
using WebCrawler.Data;
using WebCrawler.Extensions;
using WebCrawler.LinkExtractor;
using WebCrawler.PageProcessor;
using WebCrawler.Sitemap;
using WebCrawler.Utils;

namespace ImageDownloader.Screens.Processing
{
    public sealed class ProcessingViewModel : BaseViewModel
    {
        private const int CrawlProcessingStep = 0;
        private const int BuildProcessingStep = 1;

        private ConcurrentQueue<Page> pages;

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set { this.RaiseAndSetIfChanged(ref _Url, value); }
        }

        private string _CrawlerStatus;
        public string CrawlerStatus
        {
            get { return _CrawlerStatus; }
            set { this.RaiseAndSetIfChanged(ref _CrawlerStatus, value); }
        }

        private string _SitemapStatus;
        public string SitemapStatus
        {
            get { return _SitemapStatus; }
            set { this.RaiseAndSetIfChanged(ref _SitemapStatus, value); }
        }

        private ReactiveList<TaskInformationViewModel> _Crawlers;
        public ReactiveList<TaskInformationViewModel> Crawlers
        {
            get { return _Crawlers; }
            set { this.RaiseAndSetIfChanged(ref _Crawlers, value); }
        }

        private ReactiveList<TaskInformationViewModel> _Builders;
        public ReactiveList<TaskInformationViewModel> Builders
        {
            get { return _Builders; }
            set { this.RaiseAndSetIfChanged(ref _Builders, value); }
        }

        private int _ProcessingStep; // 0 = crawling site, 1 = building sitemap
        public int ProcessingStep
        {
            get { return _ProcessingStep; }
            set { this.RaiseAndSetIfChanged(ref _ProcessingStep, value); }
        }

        public ProcessingViewModel(ApplicationController controller) : base(controller)
        {
            DisplayName = "Crawl";

            Crawlers = Enumerable.Range(1, Settings.MaxThreadCount)
                                 .Select(i => new TaskInformationViewModel { DisplayName = "Crawler " + i })
                                 .ToReactiveList();

            Builders = Enumerable.Range(1, Settings.MaxThreadCount)
                                 .Select(i => new TaskInformationViewModel { DisplayName = "Builder " + i })
                                 .ToReactiveList();
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Url = controller.SiteInformation.Url;
            CrawlerStatus = "Crawler: initializing";
            SitemapStatus = "Sitemap: Waiting for crawler";
            pages = new ConcurrentQueue<Page>();
            controller.IsBusy = true;

            ProcessingStep = CrawlProcessingStep;
            await CrawlSite();

            await Task.Delay(2000);
            
            ProcessingStep = BuildProcessingStep;
            await BuildSitemap();

            controller.IsBusy = false;
        }

        private Task BuildSitemap()
        {
            var options = new SitemapOptions
            {
                MaxThreadCount = Settings.MaxThreadCount
            };
            var progress = new ProcessProgress
            {
                Progress = new Progress<string>(str => SitemapStatus = "Sitemap: " + str),
                TaskProgress = Builders.Select(c => new Progress<string>(str => c.Text = str)).Cast<IProgress<string>>().ToList()
            };
            IProgress<string> main_progress = new Progress<string>(str => controller.MainStatusText = str);
            var aux_progress = new Progress<string>(str => controller.AuxiliaryStatusText = str);
            var link_extractor = new AllInternalFilesExtractor(Url.GetHost());
            var dispatcher = Dispatcher.CurrentDispatcher;

            main_progress.Report("Building sitemap for " + Url);
            return Task.Factory.StartNew(() =>
            {
                using (var timer = new Timer(dispatcher, aux_progress))
                {
                    var builder = new SitemapBuilder(Url, options, progress, link_extractor);
                    var builder_task = builder.Build(pages);
                    builder_task.Wait();
                    progress.Report("Done");
                    main_progress.Report(string.Format("Built sitemap for {0} in {1} ms", Url, timer.Elapsed));
                }
            });
        }

        private Task CrawlSite()
        {
            var options = new CrawlerOptions
            {
                DataFolder = controller.Settings.DataFolder,
                MaxThreadCount = Settings.MaxThreadCount,
            };
            var progress = new ProcessProgress
            {
                Progress = new Progress<string>(str => CrawlerStatus = "Crawler: " + str),
                TaskProgress = Crawlers.Select(c => new Progress<string>(str => c.Text = str)).Cast<IProgress<string>>().ToList()
            };
            IProgress<string> main_progress = new Progress<string>(str => controller.MainStatusText = str);
            var aux_progress = new Progress<string>(str => controller.AuxiliaryStatusText = str);
            var link_extractor = new AllInternalLinksExtractor(Url.GetHost());
            var page_processor = new PageCollector(pages);
            var dispatcher = Dispatcher.CurrentDispatcher;

            main_progress.Report("Crawling site " + Url);
            return Task.Factory.StartNew(() =>
            {
                using (var timer = new Timer(dispatcher, aux_progress))
                {
                    using (var crawler = new Crawler(Url, options, progress, link_extractor, page_processor))
                    {
                        crawler.Crawl().Wait();
                    }
                    main_progress.Report(string.Format("Crawled site {0} in {1} ms", Url, timer.Elapsed));
                    progress.Report("Done");
                }
            });
        }
    }
}
