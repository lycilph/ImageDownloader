using System.ComponentModel.Composition;
using ReactiveUI;
using System;
using ImageDownloader.Model;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ImageDownloader.Core.Utils;

namespace ImageDownloader.Contents.Job.ViewModels
{
    [Export(typeof(IJobStep))]
    [ExportMetadata("Order", 2)]
    public class JobStepExecuteViewModel : JobStepBase
    {
        private ICache cache;

        private ReactiveList<string> _Pages = new ReactiveList<string>();
        public ReactiveList<string> Pages
        {
            get { return _Pages; }
            set { this.RaiseAndSetIfChanged(ref _Pages, value); }
        }

        private ReactiveList<string> _Images = new ReactiveList<string>();
        public ReactiveList<string> Images
        {
            get { return _Images; }
            set { this.RaiseAndSetIfChanged(ref _Images, value); }
        }

        private ReactiveList<string> _Downloads = new ReactiveList<string>();
        public ReactiveList<string> Downloads
        {
            get { return _Downloads; }
            set { this.RaiseAndSetIfChanged(ref _Downloads, value); }
        }

        private bool _IsPageTaskBusy = false;
        public bool IsPageTaskBusy
        {
            get { return _IsPageTaskBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsPageTaskBusy, value); }
        }

        private bool _IsImageTaskBusy = false;
        public bool IsImageTaskBusy
        {
            get { return _IsImageTaskBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsImageTaskBusy, value); }
        }

        private bool _IsDownloadTaskBusy = false;
        public bool IsDownloadTaskBusy
        {
            get { return _IsDownloadTaskBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsDownloadTaskBusy, value); }
        }

        [ImportingConstructor]
        public JobStepExecuteViewModel(ICache cache)
        {
            DisplayName = "Execute";
            this.cache = cache;

            this.WhenAnyDynamic(new string[] { "Model", "Website" }, x => (string)x.Value)
                .Subscribe(x => IsEnabled = !string.IsNullOrWhiteSpace(x));
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Pages.Clear();
            Images.Clear();
            Downloads.Clear();

            var host = Model.Website.GetHostName();
            cache.Initialize(host);

            var page_collection = new BlockingCollection<string>();
            var image_collection = new BlockingCollection<string>();

            // Crawl site for pages
            IsPageTaskBusy = true;
            var page_progress = new Progress<string>(x => Pages.Add(x));
            var page_task = Task.Factory.StartNew(() =>
            {
                var crawler = new SiteCrawler(cache, page_progress);
                crawler.FindAllPages(Model, page_collection);
            })
            .ContinueWith(parent => IsPageTaskBusy = false, TaskScheduler.FromCurrentSynchronizationContext());

            // Analyze pages for images
            IsImageTaskBusy = true;
            var image_progress = new Progress<string>(x => Images.Add(x));
            var image_task = Task.Factory.StartNew(() =>
            {
                var analyzer = new SiteAnalyzer(cache, image_progress);
                analyzer.FindAllImages(page_collection.GetConsumingEnumerable(), image_collection);
            })
            .ContinueWith(parent => IsImageTaskBusy = false, TaskScheduler.FromCurrentSynchronizationContext());

            // Download images and check for size
            IsDownloadTaskBusy = true;
            var download_progress = new Progress<string>(x => Downloads.Add(x));
            var download_task = Task.Factory.StartNew(() =>
            {
                var loader = new SiteLoader(cache, download_progress);
                loader.LoadAllImages(image_collection.GetConsumingEnumerable());
            })
            .ContinueWith(parent => IsDownloadTaskBusy = false, TaskScheduler.FromCurrentSynchronizationContext());

            Task.WhenAll(page_task, image_task, download_task)
                .ContinueWith(parent => cache.Update(), TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
