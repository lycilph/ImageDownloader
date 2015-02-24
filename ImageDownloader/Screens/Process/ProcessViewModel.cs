using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ImageDownloader.Controllers;
using ImageDownloader.Data;
using ImageDownloader.Services;
using ImageDownloader.Utils;
using Panda.ApplicationCore;
using Panda.ApplicationCore.Extensions;
using ReactiveUI;
using WebCrawler.Utils;

namespace ImageDownloader.Screens.Process
{
    [Export(typeof(StepScreen))]
    [Export(typeof(ProcessViewModel))]
    [ExportOrder(3)]
    public sealed class ProcessViewModel : StepScreen
    {
        private const int CrawlProcessingStep = 0;
        private const int BuildProcessingStep = 1;
        
        private readonly SiteController site_controller;
        private readonly StatusController status_controller;
        private readonly CrawlerService crawler_service;
        private readonly SitemapService sitemap_service;
        private readonly ProcessStatus crawler_status;
        private readonly ProcessStatus sitemap_status;

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

        private int _ProcessingStep;
        public int ProcessingStep
        {
            get { return _ProcessingStep; }
            set { this.RaiseAndSetIfChanged(ref _ProcessingStep, value); }
        }

        public override bool CanNext
        {
            get { return true; }
            protected set { throw new NotSupportedException(); }
        }

        public override bool CanPrevious
        {
            get { return true; }
            protected set { throw new NotSupportedException(); }
        }

        [ImportingConstructor]
        public ProcessViewModel(SiteController site_controller, StatusController status_controller, CrawlerService crawler_service, SitemapService sitemap_service)
        {
            DisplayName = "Process";
            this.site_controller = site_controller;
            this.status_controller = status_controller;
            this.crawler_service = crawler_service;
            this.sitemap_service = sitemap_service;

            Crawlers = Enumerable.Range(1, Settings.MaxThreadCount)
                                 .Select(i => new TaskInformationViewModel { DisplayName = "Crawler " + i })
                                 .ToReactiveList();

            Builders = Enumerable.Range(1, Settings.MaxThreadCount)
                                 .Select(i => new TaskInformationViewModel { DisplayName = "Builder " + i })
                                 .ToReactiveList();

            crawler_status = new ProcessStatus
            {
                OverallProgress = new Progress<string>(str => CrawlerStatus = str),
                TaskProgress = Crawlers.Select(c => new Progress<string>(str => c.Text = str))
                                       .Cast<IProgress<string>>()
                                       .ToList()
            };

            sitemap_status = new ProcessStatus
            {
                OverallProgress = new Progress<string>(str => SitemapStatus = str),
                TaskProgress = Builders.Select(c => new Progress<string>(str => c.Text = str))
                                       .Cast<IProgress<string>>()
                                       .ToList()
            };
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            CrawlerStatus = "Waiting for crawler service";
            SitemapStatus = "Waiting for sitemap service";

            Crawlers.Apply(c => c.Text = "Waiting");
            Builders.Apply(c => c.Text = "Waiting");
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            var progress = new Progress<string>(str => status_controller.AuxiliaryStatusText = str);
            using (var timer = new Timer(progress))
            {
                Url = site_controller.Url;
                status_controller.IsBusy = true;

                ProcessingStep = CrawlProcessingStep;
                await Task.Delay(500);
                var pages = await crawler_service.Crawl(Url, site_controller.SiteOptions, crawler_status);
                await Task.Delay(500);
                ProcessingStep = BuildProcessingStep;
                await Task.Delay(500);
                site_controller.Sitemap = await sitemap_service.Build(Url, pages, sitemap_status);

                status_controller.IsBusy = false;                
            }
        }
    }
}
