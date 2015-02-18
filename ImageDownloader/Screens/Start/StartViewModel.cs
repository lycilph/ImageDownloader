using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageDownloader.Controllers;
using Microsoft.Win32;
using NLog;
using ReactiveUI;

namespace ImageDownloader.Screens.Start
{
    public sealed class StartViewModel : BaseViewModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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

        public List<string> FavoriteUrls { get { return controller.Settings.FavoriteSiteUrls; } }

        public List<string> FavoriteFiles { get { return controller.Settings.FavoriteSiteFiles; } }

        private readonly ObservableAsPropertyHelper<bool> _CanCrawlSite;
        public bool CanCrawlSite { get { return _CanCrawlSite.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanLoadSite;
        public bool CanLoadSite { get { return _CanLoadSite.Value; } }

        public StartViewModel(ApplicationController controller) : base(controller)
        {
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
            controller.MainStatusText = "Select which site to crawl or load";
        }

        public void CrawlSite()
        {
            controller.CrawlSite(Url);
        }

        public void LoadSite()
        {
            if (!File.Exists(Filename))
            {
                controller.MainStatusText = Filename + " does not exist!";
                return;
            }
            controller.LoadSite(Filename);
        }

        public void Capture()
        {
            controller.ShowBrowser(Url);
        }

        public void Browse()
        {
            var open_file_dialog = new OpenFileDialog
            {
                InitialDirectory = controller.Settings.DataFolder,
                DefaultExt = ".site",
                Filter = "Site file (.site)|*.site"
            };

            if (open_file_dialog.ShowDialog() == true)
            {
                Filename = open_file_dialog.FileName;
            }
        }
    }
}
