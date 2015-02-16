using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Controllers;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader.Screens
{
    public class BaseViewModel : ReactiveScreen
    {
        protected readonly ApplicationController controller;

        public BaseViewModel Previous { get; set; }
        public BaseViewModel Next { get; set; }

        private IScreen _Option;
        public IScreen Option
        {
            get { return _Option; }
            set { this.RaiseAndSetIfChanged(ref _Option, value); }
        }

        public BaseViewModel(ApplicationController controller)
        {
            this.controller = controller;
        }
    }
}
