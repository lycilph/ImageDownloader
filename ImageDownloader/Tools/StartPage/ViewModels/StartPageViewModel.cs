using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.ServiceModel.Syndication;
using System.Xml;
using ReactiveUI;

namespace ImageDownloader.Tools.StartPage.ViewModels
{
    [Export(typeof(ITool))]
    [Export(typeof(IStartPage))]
    public class StartPageViewModel : Tool, IStartPage
    {
        private IEventAggregator event_aggregator;

        public override PaneLocation DefaultLocation
        {
            get { return PaneLocation.Content; }
        }

        public override double DefaultSize
        {
            get { return 0; }
        }

        public override bool CanAutoHide
        {
            get { return false; }
        }

        private ReactiveList<RssItemViewModel> _RssItems;
        public ReactiveList<RssItemViewModel> RssItems
        {
            get { return _RssItems; }
            set { this.RaiseAndSetIfChanged(ref _RssItems, value); }
        }

        [ImportingConstructor]
        public StartPageViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = "Start Page";
            IsVisible = true;

            this.event_aggregator = event_aggregator;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            SyndicationFeed feed = null;
            using (var reader = XmlReader.Create("http://rss.cnn.com/rss/edition.rss"))
            {
                feed = SyndicationFeed.Load(reader);
            }

            RssItems = new ReactiveList<RssItemViewModel>(feed.Items.Take(10).Select(i => new RssItemViewModel(i.Title.Text, i.Summary.Text)));
        }

        public void NewJob()
        {
            IsVisible = false;
            event_aggregator.PublishOnCurrentThread(ShellMessage.NewJob);
        }

        public void OpenJob()
        {
            throw new NotImplementedException();
        }
    }
}
