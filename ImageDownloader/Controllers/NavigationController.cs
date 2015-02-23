using System.ComponentModel.Composition;
using Caliburn.Micro;
using ImageDownloader.Screens.Main;
using ImageDownloader.Screens.Options;
using ImageDownloader.Screens.Sitemap;
using ImageDownloader.Screens.Start;
using ImageDownloader.Shell;

namespace ImageDownloader.Controllers
{
    [Export(typeof(NavigationController))]
    public class NavigationController
    {
        private readonly ShellViewModel shell;
        private MainViewModel main;

        [ImportingConstructor]
        public NavigationController(ShellViewModel shell)
        {
            this.shell = shell;
        }

        public void Initialize()
        {
            main = IoC.Get<MainViewModel>(); // This is done here to avoid circular dependencies in the constructor
            var start = IoC.Get<StartViewModel>();

            shell.Show(main);
            main.Show(start);
        }

        public void ShowStart()
        {
            main.ResetAndShow(IoC.Get<StartViewModel>());
        }

        public void ShowOptions()
        {
            main.Show(IoC.Get<OptionsViewModel>());
        }

        public void ShowSitemap()
        {
            main.Show(IoC.Get<SitemapViewModel>());
        }
    }
}
