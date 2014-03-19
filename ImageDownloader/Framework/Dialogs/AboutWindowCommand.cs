using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Framework.Services;
using System.ComponentModel.Composition;

namespace ImageDownloader.Framework.Commands
{
    [Export(typeof(IWindowCommand))]
    [ExportMetadata("Order", 2)]
    public class AboutWindowCommand : WindowCommandBase
    {
        private IWindowManager window_manager;

        [ImportingConstructor]
        public AboutWindowCommand(IWindowManager window_manager)
        {
            DisplayName = "About";
            this.window_manager = window_manager;
        }

        public override void Execute()
        {
            window_manager.ShowAboutDialog();
        }
    }
}
