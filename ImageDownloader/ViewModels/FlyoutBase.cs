using MahApps.Metro.Controls;
using ReactiveUI;

namespace ImageDownloader.ViewModels
{
    public class FlyoutBase : ReactiveObject
    {
        private string _Header;
        public string Header
        {
            get { return _Header; }
            set { this.RaiseAndSetIfChanged(ref _Header, value); }
        }

        private bool _IsOpen;
        public bool IsOpen
        {
            get { return _IsOpen; }
            set { this.RaiseAndSetIfChanged(ref _IsOpen, value); }
        }

        private Position _Position;
        public Position Position
        {
            get { return _Position; }
            set { this.RaiseAndSetIfChanged(ref _Position, value); }
        }

        private bool _ShowInTitlebar;
        public bool ShowInTitlebar
        {
            get { return _ShowInTitlebar; }
            set { this.RaiseAndSetIfChanged(ref _ShowInTitlebar, value); }
        }

        public FlyoutBase(string header, Position position, bool show_in_titlebar)
        {
            _Header = header;
            _Position = position;
            _IsOpen = false;
            _ShowInTitlebar = show_in_titlebar;
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }
    }
}
