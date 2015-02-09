using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Microsoft.Win32;
using ReactiveUI;

namespace ImageDownloader
{
    public sealed class SiteViewModel : StepViewModel
    {
        private readonly Settings settings;
        private readonly ShellViewModel shell;

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

        public SiteViewModel(Settings settings, ShellViewModel shell)
        {
            this.settings = settings;
            this.shell = shell;

            DisplayName = "Site";
            Option = new SiteOptionViewModel(this, shell);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
            shell.IsBusy = true;

            if (shell.Selection.Kind == Selection.SelectionKind.Web)
                await CrawlSite(shell.Selection.Name);
            else
                await LoadSite(shell.Selection.Name);

            shell.IsBusy = false;
        }

        private async Task CrawlSite(string url)
        {
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
            shell.Text = provider_status;
        }

        private async Task LoadSite(string filename)
        {
            site = await Task.Factory.StartNew(() => JsonExtensions.ReadFromFileAndUnzip<Site>(filename));
            CreateSiteMap();
            shell.Text = "Loaded site " + filename;
        }

        private void CreateSiteMap()
        {
            Nodes = new ReactiveList<Node>
            {
                new Node(SiteAnalyzer.CreateSiteMap(site), null)
            };
            Nodes.Apply(n => n.SelectionChanged += UpdateSelectionCount);
        }

        private void UpdateSelectionCount(object sender, EventArgs args)
        {
            var count = Nodes.Sum(n => n.GetSelectedFilesCount());
            shell.Text = (count > 0 ? "Selected files: " + count : string.Empty);
        }

        private IPageProvider GetPageProvider(string url)
        {
            if (!settings.UseCache)
                return new WebPageProvider();

            var uri = new Uri(url);
            var cache_name = uri.Host + uri.PathAndQuery;
            var cache_filename = cache_name.ToLowerInvariant().MakeFilenameSafe() + "_cache.data";
            var cache_path = Path.Combine(settings.DataFolder, cache_filename);
            return new CachedPageProvider(cache_path);
        }

        public void Save()
        {
            var save_file_dialog = new SaveFileDialog
            {
                InitialDirectory = settings.DataFolder,
                DefaultExt = ".data",
                Filter = "Site file (.data)|*.data"
            };

            if (save_file_dialog.ShowDialog() != true) 
                return;

            JsonExtensions.ZipAndWriteToFile(save_file_dialog.FileName, site);
            shell.Text = string.Format("Saved {0} to {1}", shell.Selection.Name, save_file_dialog.FileName);
        }
    }
}
