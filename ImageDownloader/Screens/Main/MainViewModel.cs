using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Model;
using ImageDownloader.Screens.Crawl;
using ImageDownloader.Screens.Download;
using ImageDownloader.Screens.Site;
using ImageDownloader.Screens.Start;
using ImageDownloader.Shell;
using NLog;
using ReactiveUI;

namespace ImageDownloader.Screens.Main
{
    public class MainViewModel : ReactiveConductor<BaseViewModel>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly BaseViewModel start;
        private readonly BaseViewModel crawl;
        private readonly BaseViewModel site;

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        private List<BaseViewModel> _Screens;
        public List<BaseViewModel> Screens
        {
            get { return _Screens; }
            set { this.RaiseAndSetIfChanged(ref _Screens, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanNext;
        public bool CanNext { get { return _CanNext.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanPrevious;
        public bool CanPrevious { get { return _CanPrevious.Value; } }

        public MainViewModel(Settings settings, ShellViewModel shell)
        {
            start = new StartViewModel(settings, shell, this) {Next = null, Previous = null};
            crawl = new CrawlViewModel(settings, shell, this) {Previous = start};
            site = new SiteViewModel(settings, shell) {Previous = start};
            var download = new DownloadViewModel(settings, shell) {Next = null, Previous = site};

            crawl.Next = site;
            site.Next = download;

            Screens = new List<BaseViewModel> {start, crawl, site, download};
            
            shell.WhenAnyValue(x => x.IsBusy).Subscribe(x => IsBusy = x);

            _CanNext = this.WhenAny(x => x.ActiveItem,
                                    x => x.IsBusy,
                                    (item, busy) => item.Value.Next != null && !busy.Value)
                                    .ToProperty(this, x => x.CanNext);

            _CanPrevious = this.WhenAny(x => x.ActiveItem,
                                        x => x.IsBusy,
                                        (item, busy) => item.Value.Previous != null && !busy.Value)
                                        .ToProperty(this, x => x.CanPrevious);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Home();
        }

        protected override void ChangeActiveItem(BaseViewModel new_item, bool close_previous)
        {
            base.ChangeActiveItem(new_item, close_previous);
            logger.Trace("Changing to step: " + new_item.DisplayName);
        }

        public void Home()
        {
            ActivateItem(start);
        }

        public void Crawl()
        {
            ActivateItem(crawl);
        }

        public void Site()
        {
            ActivateItem(site);
        }

        public void Next()
        {
            ActivateItem(ActiveItem.Next);
        }

        public void Previous()
        {
            ActivateItem(ActiveItem.Previous);
        }
    }
}
