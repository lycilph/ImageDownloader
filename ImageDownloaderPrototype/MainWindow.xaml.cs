using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Core;
using ImageDownloader;
using NLog;
using NLog.Config;

namespace ImageDownloaderPrototype
{
    public partial class MainWindow
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private const string cache_filename = @"C:\Private\Projects\ImageDownloader\Data\cache.data";

        public string SiteUrl
        {
            get { return (string) GetValue(SiteUrlProperty); }
            set { SetValue(SiteUrlProperty, value); }
        }
        public static readonly DependencyProperty SiteUrlProperty =
            DependencyProperty.Register("SiteUrl", typeof (string), typeof (MainWindow), new PropertyMetadata(string.Empty));

        public bool IsReady
        {
            get { return (bool) GetValue(IsReadyProperty); }
            set { SetValue(IsReadyProperty, value); }
        }
        public static readonly DependencyProperty IsReadyProperty =
            DependencyProperty.Register("IsReady", typeof (bool), typeof (MainWindow), new PropertyMetadata(true));

        public string StatusText
        {
            get { return (string) GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }
        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof (string), typeof (MainWindow), new PropertyMetadata(string.Empty));

        public ObservableCollection<NodeViewModel> Nodes
        {
            get { return (ObservableCollection<NodeViewModel>)GetValue(NodesProperty); }
            set { SetValue(NodesProperty, value); }
        }
        public static readonly DependencyProperty NodesProperty =
            DependencyProperty.Register("Nodes", typeof (ObservableCollection<NodeViewModel>), typeof (MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            SetupStatusbarLogging();

            SiteUrl = "http://www.skovboernehave.dk";
        }

        private void SetupStatusbarLogging()
        {
            var log_target = new WpfLogTarget {Progress = new Progress<string>(s => StatusText = s)};
            var config = LogManager.Configuration;
            config.AddTarget("s", log_target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, log_target));
            LogManager.Configuration = config;
        }

        private async void OnCrawlClick(object sender, RoutedEventArgs e)
        {
            IsReady = false;
            var sw = Stopwatch.StartNew();

            var url = SiteUrl;
            var cache_message = string.Empty;
            Site site = null;
            await Task.Factory.StartNew(() =>
            {
                using (var page_provider = new WebPageProvider())
                using (var cache = new CachedPageProvider(cache_filename, page_provider))
                using (var crawler = new SiteCrawler(cache))
                {
                    site = crawler.Crawl(url);
                    cache_message = string.Format("Cache: hits {0}, misses {1}", cache.CacheHit, cache.CacheMiss);
                }
            });

            Nodes = new ObservableCollection<NodeViewModel>
            {
                new NodeViewModel(SiteAnalyzer.CreateSiteMap(site), null)
            };
            Nodes.Apply(n => n.SelectionChanged += (o, args) => UpdateSelectionCount());

            sw.Stop();
            log.Trace("Done: " + Math.Round(sw.Elapsed.TotalSeconds, 1) + " sec - " + cache_message);
            IsReady = true;
        }

        private async void OnLoadClick(object sender, RoutedEventArgs e)
        {
            const string filename = @"C:\Private\Projects\ImageDownloader\Data\site.data";

            var site = await Task.Factory.StartNew(() => JsonExtensions.ReadFromFileAndUnzip<Site>(filename));
            Nodes = new ObservableCollection<NodeViewModel>
            {
                new NodeViewModel(SiteAnalyzer.CreateSiteMap(site), null)
            };
            Nodes.Apply(n => n.SelectionChanged += (o, args) => UpdateSelectionCount());
            log.Trace("");
        }

        private void UpdateSelectionCount()
        {
            var count = Nodes.Sum(n => n.GetSelectedFilesCount());
            if (count > 0)
                log.Trace("Selected files: " + count);
            else
                log.Trace("");
        }

        private void ListBox_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var item = ListBox.SelectedItem;
            }
        }
    }
}
