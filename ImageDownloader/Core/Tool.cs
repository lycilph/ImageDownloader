using ImageDownLoader.Core;
using ReactiveUI;
using System.Windows.Input;

namespace ImageDownloader.Core
{
    public abstract class Tool : LayoutItem, ITool
    {
        public abstract PaneLocation DefaultLocation { get;  }
        public abstract double DefaultSize { get; }
        public abstract bool CanAutoHide { get; }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { this.RaiseAndSetIfChanged(ref _IsVisible, value); }
        }

        private ICommand _CloseCommand;
        public ICommand CloseCommand
        {
            get { return _CloseCommand; }
            set { this.RaiseAndSetIfChanged(ref _CloseCommand, value); }
        }

        public Tool()
        {
            CloseCommand = new RelayCommand(obj => Close());
        }

        private void Close()
        {
            IsVisible = false;
        }
    }
}
