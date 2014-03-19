using Caliburn.Micro;
using ImageDownloader.Core;
using ReactiveUI;
using System.ComponentModel.Composition;

namespace ImageDownloader.Contents.Browser.ViewModels
{
    [Export(typeof(IContent))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BrowserViewModel : Content
    {
        private string home_url = "http://www.google.com";
        private string help_url = "help://help.html";
        
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { this.RaiseAndSetIfChanged(ref _Address, value); }
        }

        [ImportingConstructor]
        public BrowserViewModel(IEventAggregator event_aggregator) : base(event_aggregator)
        {
            DisplayName = "Browser";
            Home();
        }

        public void Home()
        {
            Address = home_url;
        }

        public void Help()
        {
            Address = help_url;
        }
    }
}
