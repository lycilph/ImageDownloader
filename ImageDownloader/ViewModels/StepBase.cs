using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ReactiveUI;

namespace ImageDownloader.ViewModels
{
    public class StepBase : ReactiveScreen, IStep
    {
        private bool _IsEnabled = false;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }
        
        public StepBase(string name)
        {
            DisplayName = name;
        }

        public virtual void Cancel() { }
    }
}
