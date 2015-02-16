using ImageDownloader.Model;
using ImageDownloader.Screens.Main;
using ImageDownloader.Shell;

namespace ImageDownloader.Controllers
{
    public class ApplicationController
    {
        //private readonly BrowserViewModel browser_view_model;

        public Settings Settings { get; set; }
        public Selection Selection { get; set; }
        public ShellViewModel Shell { get; set; }
        public MainViewModel Main { get; set; }

        public ApplicationController(ShellViewModel shell)
        {
            Shell = shell;
            Settings = Settings.Load();
            Main = new MainViewModel(this);
        }

        public void Activate()
        {
            Shell.Show(Main);
        }

        public void Deactivate()
        {
            Settings.Save();
        }

        public void ShowBrowser()
        {
            //Show(browser_view_model);
        }
    }
}
