using System.ComponentModel.Composition;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Controllers;
using ReactiveUI;

namespace ImageDownloader.Screens.Download.Option
{
    [Export(typeof(DownloadOptionViewModel))]
    public class DownloadOptionViewModel : ReactiveScreen
    {
        private readonly NavigationController navigation_controller;

        private readonly ObservableAsPropertyHelper<bool> _CanHome;
        public bool CanHome { get { return _CanHome.Value; } }

        [ImportingConstructor]
        public DownloadOptionViewModel(NavigationController navigation_controller, StatusController status_controller)
        {
            this.navigation_controller = navigation_controller;

            _CanHome = status_controller.WhenAny(x => x.IsBusy, x => !x.Value)
                                        .ToProperty(this, x => x.CanHome);
        }

        public void Home()
        {
            navigation_controller.ShowStart();
        }
    }
}
