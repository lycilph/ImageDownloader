using ReactiveUI;

namespace ImageDownloader.Model
{
    public class JobModel : ReactiveObject
    {
        private string _Website = string.Empty;
        public string Website
        {
            get { return _Website; }
            set { this.RaiseAndSetIfChanged(ref _Website, value); }
        }
    }
}
