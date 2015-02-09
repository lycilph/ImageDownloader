using ReactiveUI;

namespace ImageDownloader
{
    public class DownloadItemViewModel : ReactiveObject
    {
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private bool _Done;
        public bool Done
        {
            get { return _Done; }
            set { this.RaiseAndSetIfChanged(ref _Done, value); }
        }

        public DownloadItemViewModel(string text)
        {
            Text = text;
            Done = false;
        }
    }
}
