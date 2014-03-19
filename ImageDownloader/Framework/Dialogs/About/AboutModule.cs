using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ImageDownloader.Framework.Services;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.Framework.Dialogs.About
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 2)]
    public class AboutModule : ModuleBase
    {
        [Import]
        private IWindowManager window_manager;

        public override void Initialize()
        {
            main_menu.All.First(m => m.Name.ToLower() == "help")
                         .Add(new MenuItemSeparator(),
                              new MenuItem("_About", ShowAbout));
        }

        private void ShowAbout()
        {
            window_manager.ShowAboutDialog();
        }
    }
}
