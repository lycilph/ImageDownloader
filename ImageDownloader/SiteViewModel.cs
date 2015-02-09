using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Core;
using Microsoft.Win32;
using ReactiveUI;

namespace ImageDownloader
{
    public sealed class SiteViewModel : BaseViewModel
    {
        private Site site;

        private Node _CurrentNode;
        public Node CurrentNode
        {
            get { return _CurrentNode; }
            set { this.RaiseAndSetIfChanged(ref _CurrentNode, value); }
        }

        private ReactiveList<Node> _Nodes;
        public ReactiveList<Node> Nodes
        {
            get { return _Nodes; }
            set { this.RaiseAndSetIfChanged(ref _Nodes, value); }
        }

        private Node _CurrentSelectedNode;
        public Node CurrentSelectedNode
        {
            get { return _CurrentSelectedNode; }
            set { this.RaiseAndSetIfChanged(ref _CurrentSelectedNode, value); }
        }

        private ReactiveList<Node> _SelectedNodes = new ReactiveList<Node>();
        public ReactiveList<Node> SelectedNodes
        {
            get { return _SelectedNodes; }
            set { this.RaiseAndSetIfChanged(ref _SelectedNodes, value); }
        }

        private int _CurrentFocus;
        public int CurrentFocus
        {
            get { return _CurrentFocus; }
            set { this.RaiseAndSetIfChanged(ref _CurrentFocus, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanNext;
        public override bool CanNext { get { return _CanNext.Value; } }

        public SiteViewModel(Settings settings, ShellViewModel shell) : base(settings, shell)
        {
            DisplayName = "Site";
            Option = new SiteOptionViewModel(this, shell);

            SelectedNodes.CountChanged.Subscribe(x =>
            {
                shell.MainStatusText = (SelectedNodes.Count > 0 ? "Selected files: " + SelectedNodes.Count : string.Empty);
                shell.AuxiliaryStatusText = string.Empty;
            });

            _CanNext = SelectedNodes.CountChanged.Select(c => c > 0).ToProperty(this, x => x.CanNext);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            shell.IsBusy = true;
            SelectedNodes.Clear();

            switch (shell.Selection.Kind)
            {
                case Selection.SelectionKind.Web:
                case Selection.SelectionKind.WebCapture:
                    await CrawlSite(shell.Selection.Name);
                    break;
                case Selection.SelectionKind.File:
                    await LoadSite(shell.Selection.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            shell.IsBusy = false;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            shell.Selection.Files = SelectedNodes.Select(n => n.Text).ToList();
        }

        private async Task CrawlSite(string url)
        {
            var start_time = DateTime.Now;
            var timer = new DispatcherTimer();
            timer.Tick += (o, a) => shell.AuxiliaryStatusText = Math.Round((DateTime.Now - start_time).TotalSeconds, 1).ToString("N1") + " sec(s)";
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();

            var provider_status = string.Empty;
            await Task.Factory.StartNew(() =>
            {
                using (var page_provider = GetPageProvider(url))
                using (var crawler = new SiteCrawler(page_provider))
                {
                    site = crawler.Crawl(url);
                    provider_status = page_provider.Status();
                }
            });
            CreateSiteMap();
            shell.MainStatusText = provider_status;

            timer.Stop();
        }

        private async Task LoadSite(string filename)
        {
            site = await Task.Factory.StartNew(() => JsonExtensions.ReadFromFileAndUnzip<Site>(filename));
            CreateSiteMap();
            shell.MainStatusText = "Loaded site " + filename;
        }

        private void CreateSiteMap()
        {
            Nodes = new ReactiveList<Node>
            {
                new Node(SiteAnalyzer.CreateSiteMap(site), string.Empty, null, Node.NodeKind.Page, SelectedNodes)
            };
        }

        private IPageProvider GetPageProvider(string url)
        {
            if (!settings.UseCache)
                return new WebPageProvider();

            var uri = new Uri(url);
            var cache_name = uri.Host + uri.PathAndQuery;
            var cache_filename = cache_name.ToLowerInvariant().MakeFilenameSafe() + ".cache";
            var cache_path = Path.Combine(settings.DataFolder, cache_filename);
            return new CachedPageProvider(cache_path);
        }

        public void Save()
        {
            var save_file_dialog = new SaveFileDialog
            {
                InitialDirectory = settings.DataFolder,
                DefaultExt = ".site",
                Filter = "Site file (.site)|*.site"
            };

            if (save_file_dialog.ShowDialog() != true) 
                return;

            JsonExtensions.ZipAndWriteToFile(save_file_dialog.FileName, site);
            shell.MainStatusText = string.Format("Saved {0} to {1}", shell.Selection.Name, save_file_dialog.FileName);
        }

        public void Delete()
        {
            if (CurrentSelectedNode != null && CurrentFocus == 1)
            {
                CurrentSelectedNode.IsChecked = false;
            }
        }
    }
}
