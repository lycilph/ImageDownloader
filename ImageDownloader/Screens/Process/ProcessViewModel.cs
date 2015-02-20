using System;
using System.ComponentModel.Composition;
using System.Linq;
using ImageDownloader.Controllers;
using ImageDownloader.Data;
using Panda.ApplicationCore;
using Panda.ApplicationCore.Extensions;
using ReactiveUI;
using WebCrawler.Crawler;
using WebCrawler.Extensions;
using WebCrawler.LinkExtractor;
using WebCrawler.PageProcessor;
using WebCrawler.Utils;

namespace ImageDownloader.Screens.Process
{
    [Export(typeof(StepScreenBase))]
    [Export(typeof(ProcessViewModel))]
    [ExportOrder(3)]
    public sealed class ProcessViewModel : StepScreenBase
    {
        private readonly SiteController site_controller;

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

        private ReactiveList<TaskInformationViewModel> _Crawlers;
        public ReactiveList<TaskInformationViewModel> Crawlers
        {
            get { return _Crawlers; }
            set { this.RaiseAndSetIfChanged(ref _Crawlers, value); }
        }

        public override bool CanNext { get; protected set; }

        public override bool CanPrevious
        {
            get { return true; }
            protected set { throw new NotSupportedException(); }
        }

        [ImportingConstructor]
        public ProcessViewModel(SiteController site_controller)
        {
            DisplayName = "Process";
            this.site_controller = site_controller;

            Crawlers = Enumerable.Range(1, Settings.MaxThreadCount)
                                 .Select(i => new TaskInformationViewModel { DisplayName = "Crawler " + i })
                                 .ToReactiveList();
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            Url = site_controller.Url;

            var host = Url.GetHost();
            var site_options = site_controller.SiteOptions;
            var cache = await site_controller.SiteCacheTask;
            var options = new CrawlerOptions
            {
                Url = Url,
                UseCache = site_options.UseCache,
                Cache = cache,
                MaxThreadCount = Settings.MaxThreadCount,
                ThreadDelay = Settings.ThreadDelay,
                UserAgent = Settings.UserAgent,
                RequestTimeout = Settings.WebRequestTimeout,
                LinkExtractor = new AllInternalLinksExtractor(host),
                PageProcessor = new NullPageProcessor()
            };
            var status = new ProcessStatus
            {
                OverallProgress = new Progress<string>(str => CrawlerStatus = str),
                TaskProgress = Crawlers.Select(c => new Progress<string>(str => c.Text = str))
                                       .Cast<IProgress<string>>()
                                       .ToList()
            };
            var crawler = new Crawler(options, status);
            await crawler.Start();
        }
    }
}
