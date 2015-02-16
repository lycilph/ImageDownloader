using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageDownloader.Controllers;
using ImageDownloader.Model;
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

            if (controller.Selection != null && controller.Selection.Kind == Selection.SelectionKind.CapturedUrl)
            {
                logger.Debug("Captured url detected: " + controller.Selection.Text);
                CrawlCapturedSite();
            }

            Url = FavoriteUrls.FirstOrDefault();
            Filename = FavoriteFiles.FirstOrDefault();
            controller.Shell.MainStatusText = "Select which site to crawl or load";
        }

        private void CrawlCapturedSite()
        {
            if (!FavoriteUrls.Contains(controller.Selection.Text))
                controller.Settings.FavoriteSiteUrls.Insert(0, controller.Selection.Text);
            controller.Main.Crawl();
        }

        public void CrawlSite()
        {
            if(!FavoriteUrls.Contains(Url))
                controller.Settings.FavoriteSiteUrls.Insert(0, Url);
            controller.Selection = new Selection(Url, Selection.SelectionKind.Url);
            controller.Main.Crawl();
        }

        public void LoadSite()
        {
            if (!File.Exists(Filename))
            {
                controller.Shell.MainStatusText = Filename + " does not exist!";
                return;
            }

            if (!FavoriteFiles.Contains(Filename))
                controller.Settings.FavoriteSiteFiles.Insert(0, Filename);
            controller.Selection = new Selection(Filename, Selection.SelectionKind.File);
            controller.Main.Site();
        }

        public void Capture()
        {
            controller.Selection = new Selection(Url, Selection.SelectionKind.Url);
            controller.ShowBrowser();
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
