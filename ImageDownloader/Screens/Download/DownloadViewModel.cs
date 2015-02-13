using ImageDownloader.Model;
using ImageDownloader.Shell;

namespace ImageDownloader.Screens.Download
{
    public sealed class DownloadViewModel : BaseViewModel
    {
        public DownloadViewModel(Settings settings, ShellViewModel shell) : base(settings, shell)
        {
            DisplayName = "Download";
        }
    }
}
