using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Framework.Core.Shell;
using NLog;
using NLog.Config;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace ImageDownloader
{
    [Export(typeof(IShell))]
    public sealed class ShellViewModel : ConductorShell<StepViewModel>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Settings settings;

        public Selection Selection { get; set; }

        private List<StepViewModel> _Screens;
        public List<StepViewModel> Screens
        {
            get { return _Screens; }
            set { this.RaiseAndSetIfChanged(ref _Screens, value); }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanNext;
        public bool CanNext { get { return _CanNext.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanPrevious;
        public bool CanPrevious { get { return _CanPrevious.Value; } }

        public ShellViewModel()
        {
            DisplayName = "ImageDownloader";
            settings = Settings.Load();
            SetupStatusbarLogging();

            Screens = new List<StepViewModel>
            {
                new StartViewModel(settings, this),
                new SiteViewModel(settings, this),
                new DownloadViewModel()
            };

            _CanNext = this.WhenAny(x => x.ActiveItem, x => x.IsBusy, (item, busy) => item.Value != Screens.Last() && item.Value != Screens.First() && !busy.Value)
                           .ToProperty(this, x => x.CanNext);

            _CanPrevious = this.WhenAny(x => x.ActiveItem, x => x.IsBusy, (item, busy) => item.Value != Screens.First() && !busy.Value)
                               .ToProperty(this, x => x.CanPrevious);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Show(Screens.First());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                settings.Save();
        }

        private void SetupStatusbarLogging()
        {
            var log_target = new WpfLogTarget { Progress = new Progress<string>(s => Text = s) };
            var config = LogManager.Configuration;
            config.AddTarget("s", log_target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, log_target));
            LogManager.Configuration = config;
        }

        private void Show(StepViewModel screen)
        {
            logger.Trace("Showing screen " + screen.DisplayName);
            ActivateItem(screen);
        }

        public void Next()
        {
            var index = Screens.IndexOf(ActiveItem);
            Show(Screens[index+1]);
        }

        public void Previous()
        {
            var index = Screens.IndexOf(ActiveItem);
            Show(Screens[index-1]);
        }
    }
}
