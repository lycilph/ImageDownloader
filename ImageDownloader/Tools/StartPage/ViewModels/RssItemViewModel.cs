using ReactiveUI;

namespace ImageDownloader.Tools.StartPage.ViewModels
{
    public class RssItemViewModel : ReactiveObject
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public RssItemViewModel(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
