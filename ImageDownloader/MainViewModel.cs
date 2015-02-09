using System;
using System.Linq;
using System.Collections.Generic;
using Caliburn.Micro.ReactiveUI;
using NLog;
using ReactiveUI;

namespace ImageDownloader
{
    public class MainViewModel : ReactiveConductor<BaseViewModel>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly Settings settings;
        protected readonly ShellViewModel shell;

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
            this.settings = settings;
            this.shell = shell;

            Screens = new List<BaseViewModel>
            {
                new StartViewModel(settings, shell),
                new SiteViewModel(settings, shell),
                new DownloadViewModel(settings, shell)
            };

            shell.WhenAnyValue(x => x.IsBusy).Subscribe(x => IsBusy = x);

            _CanNext = this.WhenAny(x => x.ActiveItem,
                                    x => x.ActiveItem.CanNext,
                                    x => x.IsBusy,
                                    (item, item_next, busy) => item.Value != Screens.Last() && item.Value != Screens.First() && item_next.Value && !busy.Value)
                           .ToProperty(this, x => x.CanNext);

            _CanPrevious = this.WhenAny(x => x.ActiveItem,
                                        x => x.IsBusy,
                                        (item, busy) => item.Value != Screens.First() && !busy.Value)
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
            ActivateItem(Screens.First());
        }

        public void Next()
        {
            var index = Screens.IndexOf(ActiveItem);
            ActivateItem(Screens[index + 1]);
        }

        public void Previous()
        {
            var index = Screens.IndexOf(ActiveItem);
            ActivateItem(Screens[index - 1]);
        }
    }
}
