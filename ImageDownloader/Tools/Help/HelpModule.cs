using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ImageDownloader.Tools.Help.ViewModels;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace ImageDownloader.Tools.Help
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 1)]
    public class HelpModule : ModuleBase
    {
        public override void Initialize()
        {
            main_menu.All.First(m => m.Name.ToLower() == "help")
                         .Add(new MenuItem("_Help", ShowHelp).WithGlobalShortcut(ModifierKeys.None, Key.F1));
        }

        private void ShowHelp()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ToggleTool(typeof(IHelp)));
        }
    }
}
