using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using AutoMapper;
using ImageDownloader.Controllers;
using ImageDownloader.Data;
using Microsoft.Win32;
using NLog;
using Panda.ApplicationCore;
using Panda.ApplicationCore.Validation;
using ReactiveUI;
using WebCrawler.Extensions;

namespace ImageDownloader.Screens.Start
{
    [Export(typeof(StepScreen))]
    [Export(typeof(StartViewModel))]
    [ExportOrder(1)]
    public sealed class StartViewModel : StepScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Settings settings;
        private readonly SiteController site_controller;
        private readonly NavigationController navigation_controller;

        private string _CurrentFavoriteUrl;
        public string CurrentFavoriteUrl
        {
            get { return _CurrentFavoriteUrl; }
            set { this.RaiseAndSetIfChanged(ref _CurrentFavoriteUrl, value); }
        }

        private string _CurrentFavoriteFile;
        public string CurrentFavoriteFile
        {
            get { return _CurrentFavoriteFile; }
            set { this.RaiseAndSetIfChanged(ref _CurrentFavoriteFile, value); }
        }

        private ReactiveList<string> _FavoriteUrls = new ReactiveList<string>();
        public ReactiveList<string> FavoriteUrls
        {
            get { return _FavoriteUrls; }
            set { this.RaiseAndSetIfChanged(ref _FavoriteUrls, value); }
        }

        private ReactiveList<string> _FavoriteFiles = new ReactiveList<string>();
        public ReactiveList<string> FavoriteFiles
        {
            get { return _FavoriteFiles; }
            set { this.RaiseAndSetIfChanged(ref _FavoriteFiles, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanCrawlSite;
        public bool CanCrawlSite { get { return _CanCrawlSite.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanLoadSite;
        public bool CanLoadSite { get { return _CanLoadSite.Value; } }

        public override bool CanNext
        {
            get { return false; }
            protected set { throw new NotSupportedException(); }
        }

        public override bool CanPrevious
        {
            get { return false; }
            protected set { throw new NotSupportedException(); }
        }

        [ImportingConstructor]
        public StartViewModel(Settings settings, SiteController site_controller, NavigationController navigation_controller)
        {
            DisplayName = "Start";
            this.settings = settings;
            this.site_controller = site_controller;
            this.navigation_controller = navigation_controller;

            _CanCrawlSite = this.WhenAny(x => x.CurrentFavoriteUrl, 
                                         x => !string.IsNullOrWhiteSpace(x.Value) && x.Value.IsWellFormedUrl())
                               .ToProperty(this, x => x.CanCrawlSite);

            _CanLoadSite = this.WhenAny(x => x.CurrentFavoriteFile, x => !string.IsNullOrWhiteSpace(x.Value))
                               .ToProperty(this, x => x.CanLoadSite);

            this.Validate(x => x.CurrentFavoriteUrl, x => !string.IsNullOrWhiteSpace(x) && !x.IsWellFormedUrl(), "Invalid url");
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Mapper.Map(settings, this);

            await site_controller.Cleanup();

            if (string.IsNullOrWhiteSpace(CurrentFavoriteUrl) && FavoriteUrls.Any())
                CurrentFavoriteUrl = FavoriteUrls.First();

            if (string.IsNullOrWhiteSpace(CurrentFavoriteFile) && FavoriteFiles.Any())
                CurrentFavoriteFile = FavoriteFiles.First();
        }

        public void CrawlSiteOnEnter(Key key)
        {
            if (key == Key.Enter && !string.IsNullOrWhiteSpace(CurrentFavoriteUrl))
                CrawlSite();
        }

        public void CrawlSite()
        {
            logger.Trace("Crawling site " + CurrentFavoriteUrl);
            site_controller.Initialize(CurrentFavoriteUrl);
            navigation_controller.ShowOptions();
        }

        public void LoadSiteOnEnter(Key key)
        {
            if (key == Key.Enter && !string.IsNullOrWhiteSpace(CurrentFavoriteFile))
                LoadSite();
        }

        public void LoadSite()
        {
            logger.Trace("Loading site " + CurrentFavoriteFile);
            site_controller.Load(CurrentFavoriteFile);
            navigation_controller.ShowSitemap();
        }

        public void Browse()
        {
            var open_file_dialog = new OpenFileDialog
            {
                InitialDirectory = settings.DataFolder,
                DefaultExt = ".site",
                Filter = "Site file (.site)|*.site"
            };

            if (open_file_dialog.ShowDialog() == true)
            {
                CurrentFavoriteFile = open_file_dialog.FileName;
                LoadSite();
            }
        }
    }
}
