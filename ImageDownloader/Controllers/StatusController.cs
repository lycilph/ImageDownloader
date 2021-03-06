﻿using System.ComponentModel.Composition;
using ImageDownloader.Shell;
using ReactiveUI;

namespace ImageDownloader.Controllers
{
    [Export(typeof(StatusController))]
    public class StatusController : ReactiveObject
    {
        private readonly ShellViewModel shell;

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                this.RaiseAndSetIfChanged(ref _IsBusy, value);
                shell.IsBusy = value;
            }
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                this.RaiseAndSetIfChanged(ref _IsEnabled, value);
                shell.IsEnabled = value;
            }
        }

        public string MainStatusText { set { shell.MainStatusText = value; } }
        public string AuxiliaryStatusText { set { shell.AuxiliaryStatusText = value; } }

        [ImportingConstructor]
        public StatusController(ShellViewModel shell)
        {
            this.shell = shell;
        }
    }
}
