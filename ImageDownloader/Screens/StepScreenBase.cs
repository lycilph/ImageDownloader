using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader.Screens
{
    public abstract class StepScreenBase : ReactiveScreen
    {
        public abstract bool CanNext { get; protected set; }
        public abstract bool CanPrevious { get; protected set; }

        private IScreen _Option;
        public IScreen Option
        {
            get { return _Option; }
            set { this.RaiseAndSetIfChanged(ref _Option, value); }
        }
    }
}
