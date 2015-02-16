using ImageDownloader.Controllers;

namespace ImageDownloader.Screens.Site
{
    public sealed class SiteViewModel : BaseViewModel
    {
        public SiteViewModel(ApplicationController controller) : base(controller)
        {
            DisplayName = "Site";
        }
    }
}
