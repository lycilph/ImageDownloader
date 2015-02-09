using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader
{
    public class StepViewModel : ReactiveScreen
    {
        protected readonly Settings settings;
        protected readonly ShellViewModel shell;

        public virtual bool CanNext { get { return true; } }

        private IScreen _Option;
        public IScreen Option
        {
            get { return _Option; }
            set { this.RaiseAndSetIfChanged(ref _Option, value); }
        }

        public StepViewModel(Settings settings, ShellViewModel shell)
        {
            this.settings = settings;
            this.shell = shell;
        }
    }
}
