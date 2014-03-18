using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ImageDownloader.Framework.Services;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ImageDownloader.Framework
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 2)]
    public class AboutModule : ModuleBase
    {
        #pragma warning disable 0649 // disable warning "Fields is not assigned to..."
        [Import]
        private IWindowManager window_manager;
        #pragma warning restore 0649

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
