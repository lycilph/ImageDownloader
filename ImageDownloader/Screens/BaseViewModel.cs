using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Model;
using ImageDownloader.Shell;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader.Screens
{
    public class BaseViewModel : ReactiveScreen
    {
        protected readonly Settings settings;
        protected readonly ShellViewModel shell;

        public BaseViewModel Previous { get; set; }
        public BaseViewModel Next { get; set; }

        private IScreen _Option;
        public IScreen Option
        {
            get { return _Option; }
            set { this.RaiseAndSetIfChanged(ref _Option, value); }
        }

        public BaseViewModel(Settings settings, ShellViewModel shell)
        {
            this.settings = settings;
            this.shell = shell;
        }
    }
}
