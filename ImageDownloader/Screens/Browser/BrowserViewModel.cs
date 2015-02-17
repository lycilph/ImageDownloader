using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Controllers;
using ReactiveUI;

namespace ImageDownloader.Screens.Browser
{
    public class BrowserViewModel : ReactiveScreen
    {
        private const string HomeUrl = "www.google.com";

        private readonly ApplicationController controller;

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { this.RaiseAndSetIfChanged(ref _Address, value); }
        }

        public BrowserViewModel(ApplicationController controller)
        {
            this.controller = controller;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var url = controller.SiteInformation.Url;
            Address = (string.IsNullOrWhiteSpace(url) ? HomeUrl : url);
        }

        public void Cancel()
        {
            controller.Back();
        }

        public void Capture()
        {
            controller.Back();
            controller.CrawlSite(Address);
        }

        public void Home()
        {
            Address = HomeUrl;
        }
    }
}
