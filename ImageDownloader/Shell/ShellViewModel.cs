using System.Collections.Generic;
using System.ComponentModel.Composition;
using ImageDownloader.Model;
using ImageDownloader.Screens;
using ImageDownloader.Screens.Main;
using NLog;
using Panda.ApplicationCore.Shell;
using ReactiveUI;
using LogManager = NLog.LogManager;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader.Shell
{
    [Export(typeof(IShell))]
    public sealed class ShellViewModel : ConductorShell<IScreen>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Settings settings;
        private readonly Stack<IScreen> screens = new Stack<IScreen>();
        private readonly MainViewModel main_view_model;
        //private readonly BrowserViewModel browser_view_model;

        public Selection Selection { get; set; }

        //private MainViewModel _Main;
        //public MainViewModel Main
        //{
        //    get { return _Main; }
        //    private set { this.RaiseAndSetIfChanged(ref _Main, value); }
        //}

        private string _MainStatusText;
        public string MainStatusText
        {
            get { return _MainStatusText; }
            set { this.RaiseAndSetIfChanged(ref _MainStatusText, value); }
        }

        private string _AuxiliaryStatusText;
        public string AuxiliaryStatusText
        {
            get { return _AuxiliaryStatusText; }
            set { this.RaiseAndSetIfChanged(ref _AuxiliaryStatusText, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        public ShellViewModel()
        {
            DisplayName = "ImageDownloader";
            settings = Settings.Load();

            main_view_model = new MainViewModel(settings, this);
            //browser_view_model = new BrowserViewModel(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Show(main_view_model);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                settings.Save();
        }

        public void Back()
        {
            screens.Pop();
            ActivateItem(screens.Peek());
        }

        public void Show(IScreen view_model)
        {
            logger.Trace("Showing screen " + view_model.DisplayName);

            MainStatusText = string.Empty;
            AuxiliaryStatusText = string.Empty;

            screens.Push(view_model);
            ActivateItem(view_model);
        }

        public void ShowBrowser()
        {
            //Show(browser_view_model);
        }
    }
}
