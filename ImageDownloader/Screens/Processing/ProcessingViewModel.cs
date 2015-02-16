using System;
using System.Linq;
using System.Threading.Tasks;
using ImageDownloader.Controllers;
using ImageDownloader.Model;
using ImageDownloader.Utilities;
using Panda.ApplicationCore.Extensions;
using ReactiveUI;

namespace ImageDownloader.Screens.Processing
{
    public sealed class ProcessingViewModel : BaseViewModel
    {
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

        private ReactiveList<CrawlerViewModel> _Crawlers;
        public ReactiveList<CrawlerViewModel> Crawlers
        {
            get { return _Crawlers; }
            set { this.RaiseAndSetIfChanged(ref _Crawlers, value); }
        }

        public ProcessingViewModel(ApplicationController controller, SiteController site_controller) : base(controller, site_controller)
        {
            DisplayName = "Crawl";

            Crawlers = Enumerable.Range(1, Settings.MaxCrawlerThreadCount)
                                 .Select(i => new CrawlerViewModel {DisplayName = "Crawler " + i})
                                 .ToReactiveList();
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Url = site_controller.Url;
            CrawlerStatus = "Initializing crawler";
            SitemapStatus = "Waiting for crawler";
            controller.MainStatusText = "Processing " + Url;

            var progress = new ProcessingProgress()
            {
                SitemapProgress = new Progress<string>(str => SitemapStatus = "Sitemap: " + str),
                Progress = new Progress<string>(str => CrawlerStatus = "Crawler: " + str),
                TaskProgress = Crawlers.Select(c => new Progress<string>(str => c.Text = str)).Cast<IProgress<string>>().ToList()
            };
            controller.IsBusy = true;

            var aux_progress = new Progress<string>(str => controller.AuxiliaryStatusText = str);
            using (var timer = new Timer(aux_progress))
            {
                await site_controller.Process(progress);
                controller.MainStatusText = string.Format("Processed {0} in {1} ms [continuing in {2} seconds]", Url, timer.Elapsed, Settings.ScreenTransitionDelay/1000);
            }

            CrawlerStatus = "Crawler done";
            SitemapStatus = "Sitemap done";
            await Task.Delay(Settings.ScreenTransitionDelay);
            controller.IsBusy = false;
            controller.Next();
        }
    }
}
