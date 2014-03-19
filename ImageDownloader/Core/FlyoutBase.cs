using MahApps.Metro.Controls;
using ReactiveUI;

namespace ImageDownloader.Core
{
    public class FlyoutBase : ReactiveObject, IFlyout
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
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
    
        public FlyoutBase(string name, Position position)
        {
            _DisplayName = DisplayName;
            _Position = position;
            _IsOpen = false;
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }
    }
}
