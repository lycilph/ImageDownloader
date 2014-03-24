using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Framework.MainMenu.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows.Input;

namespace ImageDownloader.Contents.Browser
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 4)]
    public class BrowserModule : ModuleBase
    {
        public override void Initialize()
        {
            main_menu.All.First(m => m.Name.ToLower() == "window")
                         .Add(new MenuItem("Browser", ShowBrowser).WithGlobalShortcut(ModifierKeys.Control, Key.B));
        }

        private void ShowBrowser()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.NewBrowser);
        }
    }
}
