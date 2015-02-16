using ImageDownloader.Controllers;

namespace ImageDownloader.Screens.Download
{
    public sealed class DownloadViewModel : BaseViewModel
    {
        public DownloadViewModel(ApplicationController controller, SiteController site_controller) : base(controller, site_controller)
        {
            DisplayName = "Download";
        }
    }
}
