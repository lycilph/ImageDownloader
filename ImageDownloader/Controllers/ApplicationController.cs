using System.Threading;
using ImageDownloader.Model;
using ImageDownloader.Screens.Browser;
using ImageDownloader.Screens.Main;
using ImageDownloader.Shell;
using ReactiveUI;

namespace ImageDownloader.Controllers
{
    public class ApplicationController : ReactiveObject
    {
        private readonly ShellViewModel shell;
        private readonly MainViewModel main;
        private readonly BrowserViewModel browser;

        public Settings Settings { get; private set; }

        public SiteController SiteController { get; private set; }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                this.RaiseAndSetIfChanged(ref _IsBusy, value);
                shell.IsBusy = value;
            }
        }

        public string MainStatusText { set { shell.MainStatusText = value; } }
        public string AuxiliaryStatusText { set { shell.AuxiliaryStatusText = value; } }

        public ApplicationController(ShellViewModel shell)
        {
            this.shell = shell;
            Settings = Settings.Load();
            SiteController = new SiteController(this);
            main = new MainViewModel(this);
            browser = new BrowserViewModel(this);

            EnsureMinThreadCount();
        }

        private static void EnsureMinThreadCount()
        {
            int min_worker, min_ioc;
            ThreadPool.GetMinThreads(out min_worker, out min_ioc);
            if (min_worker < Settings.MaxTotalThreadCount)
                ThreadPool.SetMinThreads(Settings.MaxTotalThreadCount, min_ioc);
        }

        public void Activate()
        {
            shell.Show(main);
        }

        public void Deactivate()
        {
            Settings.Save();
        }

        public void Back()
        {
            shell.Back();
        }

        public void Next()
        {
            main.Next();
        }

        public void CrawlSite(string url)
        {
            SiteController.Url = url;
            main.ShowCrawl();
        }

        public void LoadSite(string filename)
        {
            SiteController.Load(filename);
            main.ShowSite();
        }

        public void ShowBrowser(string url)
        {
            SiteController.Url = url;
            shell.Show(browser);
        }
    }
}
