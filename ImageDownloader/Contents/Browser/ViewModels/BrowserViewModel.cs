using Caliburn.Micro;
using ImageDownloader.Core;
using ReactiveUI;
using System.ComponentModel.Composition;

namespace ImageDownloader.Contents.Browser.ViewModels
{
    [Export(typeof(IBrowser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BrowserViewModel : Content, IBrowser
    {
        private string home_url = "http://www.google.com";
        private string help_url = "help://help.html";
        
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { this.RaiseAndSetIfChanged(ref _Address, value); }
        }

        private bool _IsHosted = false;
        public bool IsHosted
        {
            get { return _IsHosted; }
            set { this.RaiseAndSetIfChanged(ref _IsHosted, value); }
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
