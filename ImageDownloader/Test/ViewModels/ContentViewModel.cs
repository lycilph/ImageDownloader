using ReactiveUI;

namespace ImageDownloader.Test.ViewModels
{
    public class ContentViewModel : ReactiveObject
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        public ContentViewModel(string name)
        {
            _Name = name;
        }
    }
}
