﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using AutoMapper;
using ImageDownloader.Controllers;
using ImageDownloader.Data;
using NLog;
using Panda.ApplicationCore;
using ReactiveUI;

namespace ImageDownloader.Screens.Start
{
    [Export(typeof(StepScreenBase))]
    [Export(typeof(StartViewModel))]
    [ExportOrder(1)]
    public sealed class StartViewModel : StepScreenBase
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
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Mapper.Map(settings, this);

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
        }
    }
}
