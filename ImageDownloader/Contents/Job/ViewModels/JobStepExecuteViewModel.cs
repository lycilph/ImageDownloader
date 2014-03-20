using System.ComponentModel.Composition;
using ReactiveUI;
using System;
using ImageDownloader.Model;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ImageDownloader.Contents.Job.ViewModels
{
    [Export(typeof(IJobStep))]
    [ExportMetadata("Order", 2)]
    public class JobStepExecuteViewModel : JobStepBase
    {
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

        public JobStepExecuteViewModel()
        {
            DisplayName = "Execute";

            this.WhenAnyDynamic(new string[] { "Model", "Website" }, x => (string)x.Value)
                .Subscribe(x => IsEnabled = !string.IsNullOrWhiteSpace(x));
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Pages.Clear();
            Images.Clear();
            Downloads.Clear();

            var page_collection = new BlockingCollection<string>();
            var image_collection = new BlockingCollection<string>();

            // Crawl site for pages
            var page_progress = new Progress<string>(x => Pages.Add(x));
            Task.Factory.StartNew(() =>
            {
                var page_cache = new Cache();
                var crawler = new SiteCrawler(page_cache, page_progress);
                crawler.FindAllPages(Model, page_collection);
            });

            // Analyze pages for images
            var image_progress = new Progress<string>(x => Images.Add(x));
            Task.Factory.StartNew(() =>
            {
                var image_cache = new Cache();
                var analyzer = new SiteAnalyzer(image_cache, image_progress);
                analyzer.FindAllImages(page_collection.GetConsumingEnumerable(), image_collection);
            });

            // Download images and check for size
            var download_progress = new Progress<string>(x => Downloads.Add(x));
            Task.Factory.StartNew(() =>
            {
                var downloads_cache = new Cache();
                var loader = new SiteLoader(downloads_cache, download_progress);
                loader.LoadAllImages(image_collection.GetConsumingEnumerable());
            });
        }
    }
}
