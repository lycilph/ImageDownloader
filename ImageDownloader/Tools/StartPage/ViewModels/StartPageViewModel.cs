using Caliburn.Micro;
using HtmlAgilityPack;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Reactive.Linq;

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

        private ReactiveList<FeedViewModel> _Feeds;
        public ReactiveList<FeedViewModel> Feeds
        {
            get { return _Feeds; }
            set { this.RaiseAndSetIfChanged(ref _Feeds, value); }
        }

        private FeedViewModel _CurrentFeed;
        public FeedViewModel CurrentFeed
        {
            get { return _CurrentFeed; }
            set { this.RaiseAndSetIfChanged(ref _CurrentFeed, value); }
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

            this.WhenAnyValue(x => x.CurrentFeed)
                .Where(x => x != null)
                .Subscribe(x => x.Load());

            Feeds = new ReactiveList<FeedViewModel>(GetRssFeeds());
            CurrentFeed = Feeds.First();
        }

        private IEnumerable<FeedViewModel> GetRssFeeds()
        {
            var page = string.Empty;
            using (var client = new WebClient())
            {
                page = client.DownloadString(@"http://edition.cnn.com/services/rss/");
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var nodes = doc.DocumentNode.SelectNodes(@"//link[@href and @type='application/rss+xml']");
            if (nodes == null)
                return new List<FeedViewModel>();

            return new List<FeedViewModel>(nodes.Select(n => new FeedViewModel(n.Attributes["title"].Value, n.Attributes["href"].Value)));
        }

        public void NewJob()
        {
            IsVisible = false;
            event_aggregator.PublishOnCurrentThread(ShellMessage.NewJob);
        }

        public void OpenJob()
        {
        }

        public void SelectFeed(FeedViewModel feed)
        {
            CurrentFeed = feed;
        }
    }
}
