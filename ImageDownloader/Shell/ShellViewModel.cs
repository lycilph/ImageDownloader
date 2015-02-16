using System.Collections.Generic;
using System.ComponentModel.Composition;
using ImageDownloader.Controllers;
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

        private readonly ApplicationController controller;
        private readonly Stack<IScreen> screens = new Stack<IScreen>();

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
            controller = new ApplicationController(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            controller.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                controller.Deactivate();
        }

        protected override void ChangeActiveItem(IScreen new_item, bool close_previous)
        {
            logger.Trace("Changing to screen: " + new_item.DisplayName);
            MainStatusText = string.Empty;
            AuxiliaryStatusText = string.Empty;
            base.ChangeActiveItem(new_item, close_previous);
        }

        public void Back()
        {
            screens.Pop();
            ActivateItem(screens.Peek());
        }

        public void Show(IScreen view_model)
        {
            screens.Push(view_model);
            ActivateItem(view_model);
        }
    }
}
