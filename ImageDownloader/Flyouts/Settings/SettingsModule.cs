using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Flyouts.Settings.ViewModels;
using ImageDownloader.Framework.MainMenu.ViewModels;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.Flyouts.Settings
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 3)]
    public class SettingsModule : ModuleBase
    {
        public override void Initialize()
        {
            main_menu.All.First(m => m.Name.ToLower() == "view")
                         .Add(new MenuItem("Settings", ShowSettings));
        }

        private void ShowSettings()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ToggleFlyout(typeof(ISettings)));
        }
    }
}
