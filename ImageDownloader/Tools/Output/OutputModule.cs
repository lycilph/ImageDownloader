using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ImageDownloader.Tools.Output.ViewModels;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace ImageDownloader.Tools.Output
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 2)]
    public class OutputModule : ModuleBase
    {
        public override void Initialize()
        {
            main_menu.All.First(m => m.Name.ToLower() == "view")
                         .Add(new MenuItem("_Output", ShowOutput).WithGlobalShortcut(ModifierKeys.Control, Key.O));
        }

        private void ShowOutput()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ToggleTool(typeof(IOutput)));
        }
    }
}
