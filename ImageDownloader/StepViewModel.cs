using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader
{
    public class StepViewModel : ReactiveScreen
    {
        private IScreen _Option;
        public IScreen Option
        {
            get { return _Option; }
            set { this.RaiseAndSetIfChanged(ref _Option, value); }
        }
    }
}
