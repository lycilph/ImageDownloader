using ImageDownloader.Controllers;
using ReactiveUI;

namespace ImageDownloader.Screens.Download
{
    public class DownloadOptionViewModel : BaseViewModel
    {
        private readonly ObservableAsPropertyHelper<bool> _CanHome;
        public bool CanHome { get { return _CanHome.Value; } }

        public DownloadOptionViewModel(ApplicationController controller) : base(controller)
        {
            _CanHome = controller.WhenAny(x => x.IsBusy, x => !x.Value)
                                 .ToProperty(this, x => x.CanHome);
        }

        public void Home()
        {
            controller.Home();
        }
    }
}
