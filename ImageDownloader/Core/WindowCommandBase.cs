using ReactiveUI;

namespace ImageDownloader.Core
{
    public class WindowCommandBase : ReactiveObject, IWindowCommand
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        public virtual void Execute() { }
    }
}
