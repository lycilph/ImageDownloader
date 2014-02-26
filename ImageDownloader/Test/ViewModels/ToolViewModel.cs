using ReactiveUI;

namespace ImageDownloader.Test.ViewModels
{
    public class ToolViewModel : ReactiveObject
    {
        private string _Name = "Default name";
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { this.RaiseAndSetIfChanged(ref _IsVisible, value); }
        }

        public ToolViewModel(string name)
        {
            _Name = name;
        }
    }
}
