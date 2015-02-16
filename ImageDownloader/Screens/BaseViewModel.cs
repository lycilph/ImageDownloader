using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Controllers;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader.Screens
{
    public class BaseViewModel : ReactiveScreen
    {
        protected readonly ApplicationController controller;
        protected readonly SiteController site_controller;

        public BaseViewModel Previous { get; set; }
        public BaseViewModel Next { get; set; }

        private IScreen _Option;
        public IScreen Option
        {
            get { return _Option; }
            set { this.RaiseAndSetIfChanged(ref _Option, value); }
        }

        public BaseViewModel(ApplicationController controller, SiteController site_controller)
        {
            this.controller = controller;
            this.site_controller = site_controller;
        }

        public void Connect(BaseViewModel previous, BaseViewModel next)
        {
            Previous = previous;
            Next = next;
        }
    }
}
