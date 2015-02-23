using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ImageDownloader.Controllers;
using NLog;
using Panda.ApplicationCore.Shell;
using ReactiveUI;
using LogManager = NLog.LogManager;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader.Shell
{
    [Export(typeof(IShell))]
    [Export(typeof(ShellViewModel))]
    public sealed class ShellViewModel : ConductorShell<IScreen>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly SiteController site_controller;
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

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        [ImportingConstructor]
        public ShellViewModel(SiteController site_controller)
        {
            DisplayName = "ImageDownloader";
            this.site_controller = site_controller;
        }

        protected override void ChangeActiveItem(IScreen new_item, bool close_previous)
        {
            logger.Trace("Changing to screen: " + new_item.DisplayName);
            MainStatusText = string.Empty;
            AuxiliaryStatusText = string.Empty;
            base.ChangeActiveItem(new_item, close_previous);
        }

        public override async void CanClose(Action<bool> callback)
        {
            IsEnabled = false;
            Mouse.OverrideCursor = Cursors.Wait;
            await site_controller.Cleanup();
            callback(true);
        }

        public void Back()
        {
            screens.Pop();
            ActivateItem(screens.Peek());
        }

        public void Show(IScreen screen)
        {
            screens.Push(screen);
            ActivateItem(screen);
        }
    }
}
