using ReactiveUI;

namespace ImageDownloader.Core
{
    public abstract class Tool : LayoutItem, ITool
    {
        public abstract PaneLocation DefaultLocation { get;  }
        public abstract double DefaultWidth { get; }
        public abstract double DefaultHeight { get; }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { this.RaiseAndSetIfChanged(ref _IsVisible, value); }
        }
    }
}
