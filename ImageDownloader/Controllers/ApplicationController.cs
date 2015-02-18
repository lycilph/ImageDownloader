using ImageDownloader.Model;
using ImageDownloader.Screens.Browser;
using ImageDownloader.Screens.Main;
using ImageDownloader.Shell;
using ReactiveUI;
using WebCrawler.Sitemap;

namespace ImageDownloader.Controllers
{
    public class ApplicationController : ReactiveObject
    {
        private readonly ShellViewModel shell;
        private readonly MainViewModel main;
        private readonly BrowserViewModel browser;

        public Settings Settings { get; private set; }

        public SiteInformation SiteInformation { get; set; } // This is data shared between view models

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
            SiteInformation = new SiteInformation();
            main = new MainViewModel(this);
            browser = new BrowserViewModel(this);
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

        public void Home()
        {
            main.ShowStart();
        }

        public void CrawlSite(string url)
        {
            SiteInformation.Url = url;
            if (!Settings.FavoriteSiteUrls.Contains(url))
                Settings.FavoriteSiteUrls.Insert(0, url);
            main.ShowOption();
        }

        public void LoadSite(string filename)
        {
            SiteInformation.Sitemap = SitemapNode.Load(filename);
            if (!Settings.FavoriteSiteFiles.Contains(filename))
                Settings.FavoriteSiteFiles.Insert(0, filename);
            main.ShowSite();
        }

        public void ShowBrowser(string url)
        {
            SiteInformation.Url = url;
            shell.Show(browser);
        }
    }
}
