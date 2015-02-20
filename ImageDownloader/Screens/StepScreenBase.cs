using Caliburn.Micro.ReactiveUI;

namespace ImageDownloader.Screens
{
    public abstract class StepScreenBase : ReactiveScreen
    {
        public abstract bool CanNext { get; protected set; }
        public abstract bool CanPrevious { get; protected set; }
    }
}
