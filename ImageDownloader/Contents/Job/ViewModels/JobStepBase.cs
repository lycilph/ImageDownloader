using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace ImageDownloader.Contents.Job.ViewModels
{
    public class JobStepBase : ReactiveScreen, IJobStep
    {
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
