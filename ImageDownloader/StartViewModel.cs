using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using NLog;
using ReactiveUI;

namespace ImageDownloader
{
    public sealed class StartViewModel : StepViewModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Settings settings;
        private readonly ShellViewModel shell;

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set { this.RaiseAndSetIfChanged(ref _Url, value); }
        }

        private string _Filename;
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        public List<string> FavoriteUrls { get { return settings.FavoriteSiteUrls; } }

        public List<string> FavoriteFiles { get { return settings.FavoriteSiteFiles; } }

        private readonly ObservableAsPropertyHelper<bool> _CanCrawlSite;
        public bool CanCrawlSite { get { return _CanCrawlSite.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanLoadSite;
        public bool CanLoadSite { get { return _CanLoadSite.Value; } }

        public StartViewModel(Settings settings, ShellViewModel shell)
        {
            this.settings = settings;
            this.shell = shell;
            DisplayName = "Start";

            _CanCrawlSite = this.WhenAny(x => x.Url, x => !string.IsNullOrWhiteSpace(x.Value))
                                .ToProperty(this, x => x.CanCrawlSite);

            _CanLoadSite = this.WhenAny(x => x.Filename, x => !string.IsNullOrWhiteSpace(x.Value))
                               .ToProperty(this, x => x.CanLoadSite);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Url = FavoriteUrls.FirstOrDefault();
            Filename = FavoriteFiles.FirstOrDefault();
            shell.Text = "Select which site to crawl or load";
        }

        public void CrawlSite()
        {
            if(!FavoriteUrls.Contains(Url))
                settings.FavoriteSiteUrls.Add(Url);
            shell.Selection = new Selection(Url, Selection.SelectionKind.Web);
            shell.Next();
        }

        public void LoadSite()
        {
            if (!File.Exists(Filename))
            {
                shell.Text = Filename + " does not exist!";
                return;
            }

            if (!FavoriteFiles.Contains(Filename))
                settings.FavoriteSiteFiles.Add(Filename);
            shell.Selection = new Selection(Filename, Selection.SelectionKind.File);
            shell.Next();
        }

        public void Capture()
        { }

        public void Browse()
        {
            var open_file_dialog = new OpenFileDialog
            {
                InitialDirectory = settings.DataFolder,
                DefaultExt = ".data",
                Filter = "Site file (.data)|*.data"
            };

            if (open_file_dialog.ShowDialog() == true)
            {
                Filename = open_file_dialog.FileName;
            }
        }
    }
}
