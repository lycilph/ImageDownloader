using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Flyouts.Settings.ViewModels;
using System.ComponentModel.Composition;

namespace ImageDownloader.Flyouts.Settings
{
    [Export(typeof(IWindowCommand))]
    [ExportMetadata("Order", 1)]
    public class SettingsWindowCommand : WindowCommandBase
    {
        private IEventAggregator event_aggregator;

        [ImportingConstructor]
        public SettingsWindowCommand(IEventAggregator event_aggregator)
        {
            DisplayName = "Settings";
            this.event_aggregator = event_aggregator;
        }

        public override void Execute()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ToggleFlyout(typeof(ISettings)));
        }
    }
}
