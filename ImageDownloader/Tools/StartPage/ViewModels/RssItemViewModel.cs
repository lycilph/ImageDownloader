using ReactiveUI;

namespace ImageDownloader.Tools.StartPage.ViewModels
{
    public class RssItemViewModel : ReactiveObject
    {
        public string Title { get; set; }
        public string Summary { get; set; }

        public RssItemViewModel(string title, string summary)
        {
            Title = title;
            Summary = summary;
        }
    }
}
