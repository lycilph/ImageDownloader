using System.Collections.Generic;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Controllers;
using ImageDownloader.Screens.Download;
using ImageDownloader.Screens.Processing;
using ImageDownloader.Screens.Site;
using ImageDownloader.Screens.Start;
using NLog;
using ReactiveUI;

namespace ImageDownloader.Screens.Main
{
    public class MainViewModel : ReactiveConductor<BaseViewModel>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ApplicationController controller;
        private BaseViewModel start;
        private BaseViewModel process;
        private BaseViewModel site;
        private BaseViewModel download;

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

        public MainViewModel(ApplicationController controller)
        {
            this.controller = controller;
            CreateScreens();

            _CanNext = this.WhenAny(x => x.ActiveItem,
                                    x => x.controller.IsBusy,
                                    (item, busy) => item.Value.Next != null && !busy.Value)
                                    .ToProperty(this, x => x.CanNext);

            _CanPrevious = this.WhenAny(x => x.ActiveItem,
                                        x => x.controller.IsBusy,
                                        (item, busy) => item.Value.Previous != null && !busy.Value)
                                        .ToProperty(this, x => x.CanPrevious);
        }

        private void CreateScreens()
        {
            start = new StartViewModel(controller);
            process = new ProcessingViewModel(controller);
            site = new SiteViewModel(controller);
            download = new DownloadViewModel(controller);

            process.Connect(start, site);
            site.Connect(start, download);
            download.Connect(site, null);

            Screens = new List<BaseViewModel> {start, process, site, download};
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ActivateItem(start);
        }

        protected override void ChangeActiveItem(BaseViewModel new_item, bool close_previous)
        {
            logger.Trace("Changing to step: " + new_item.DisplayName);
            controller.MainStatusText = string.Empty;
            controller.AuxiliaryStatusText = string.Empty;
            base.ChangeActiveItem(new_item, close_previous);
        }

        public void ShowCrawl()
        {
            ActivateItem(process);
        }

        public void ShowSite()
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
