using ReactiveUI;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ImageDownloader.Tools.StartPage.ViewModels
{
    public class FeedViewModel : ReactiveObject
    {
        private bool loaded = false;

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        private string _Link;
        public string Link
        {
            get { return _Link; }
            set { this.RaiseAndSetIfChanged(ref _Link, value); }
        }

        private ReactiveList<RssItemViewModel> _Items;
        public ReactiveList<RssItemViewModel> Items
        {
            get { return _Items; }
            set { this.RaiseAndSetIfChanged(ref _Items, value); }
        }

        public FeedViewModel(string title, string link)
        {
            _Title = title;
            _Link = link;
        }

        public void Load()
        {
            if (loaded) return;

            SyndicationFeed feed = null;
            using (var reader = XmlReader.Create(Link))
            {
                feed = SyndicationFeed.Load(reader);
            }

            Description = feed.Description.Text;
            Items = new ReactiveList<RssItemViewModel>(feed.Items.Take(10).Select(i => new RssItemViewModel(i.Title.Text, i.Summary.Text)));
            loaded = true;
        }
    }
}
