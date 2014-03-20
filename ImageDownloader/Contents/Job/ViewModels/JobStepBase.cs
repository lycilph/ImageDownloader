﻿using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Model;
using ReactiveUI;

namespace ImageDownloader.Contents.Job.ViewModels
{
    public class JobStepBase : ReactiveScreen, IJobStep
    {
        private JobModel _Model;
        public JobModel Model
        {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(ref _Model, value); }
        }

        private bool _IsEnabled = false;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            IsEnabled = true;
        }
    }
}
