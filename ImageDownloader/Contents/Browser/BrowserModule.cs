using Caliburn.Micro;
using ImageDownloader.Contents.Browser.ViewModels;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Framework.MainMenu.ViewModels;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace ImageDownloader.Contents.Browser
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 4)]
    public class BrowserModule : ModuleBase
    {
        public override void Initialize()
        {
            main_menu.All.First(m => m.Name.ToLower() == "view")
                         .Add(new MenuItem("Browser", Add).WithGlobalShortcut(ModifierKeys.Control, Key.B));
        }

        private void Add()
        {
            var browser = IoC.Get<IBrowser>();
            event_aggregator.PublishOnCurrentThread(ShellMessage.AddContent(browser));
        }
    }
}
