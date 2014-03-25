using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ImageDownloader.Tools.StartPage.ViewModels;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.Tools.StartPage
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 1)]
    public class StartPageModule : ModuleBase
    {
        public override void Initialize()
        {
            main_menu.All.First(m => m.Name.ToLower() == "view")
                         .Add(new MenuItem("_Start Page", ShowStartPage));
        }

        private void ShowStartPage()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ToggleTool(typeof(IStartPage)));
        }
    }
}
