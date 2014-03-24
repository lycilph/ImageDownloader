using Caliburn.Micro.ReactiveUI;
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

        private bool _IsHosted = false;
        public bool IsHosted
        {
            get { return _IsHosted; }
            set { this.RaiseAndSetIfChanged(ref _IsHosted, value); }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            IsEnabled = true;
        }
    }
}
