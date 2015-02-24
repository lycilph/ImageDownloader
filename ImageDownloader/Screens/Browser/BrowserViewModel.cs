using System.ComponentModel.Composition;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Controllers;
using NLog;
using ReactiveUI;

namespace ImageDownloader.Screens.Browser
{
    [Export(typeof(BrowserViewModel))]
    public class BrowserViewModel : ReactiveScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string HomeUrl = "www.google.com";

        private readonly SiteController site_controller;
        
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { this.RaiseAndSetIfChanged(ref _Address, value); }
        }

        [ImportingConstructor]
        public BrowserViewModel(SiteController site_controller)
        {
            this.site_controller = site_controller;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        
            var url = site_controller.Url;
            Address = (string.IsNullOrWhiteSpace(url) ? HomeUrl : url);
        }

        public void Home()
        {
            Address = HomeUrl;
        }
    }
}
