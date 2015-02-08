using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader
{
    public class StepViewModel : ReactiveScreen
    {
        private IScreen _Misc;
        public IScreen Misc
        {
            get { return _Misc; }
            set { this.RaiseAndSetIfChanged(ref _Misc, value); }
        }
    }
}
