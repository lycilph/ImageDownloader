using ImageDownloader.Controllers;

namespace ImageDownloader.Screens.Download
{
    public sealed class DownloadViewModel : BaseViewModel
    {
        public DownloadViewModel(ApplicationController controller) : base(controller)
        {
            DisplayName = "Download";
        }
    }
}
