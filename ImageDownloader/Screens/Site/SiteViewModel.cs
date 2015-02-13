using ImageDownloader.Model;
using ImageDownloader.Shell;

namespace ImageDownloader.Screens.Site
{
    public sealed class SiteViewModel : BaseViewModel
    {
        public SiteViewModel(Settings settings, ShellViewModel shell) : base(settings, shell)
        {
            DisplayName = "Site";
        }
    }
}
