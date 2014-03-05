using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Modules.ViewModels
{
    [Export(typeof(ILayoutItem))]
    public class HelloWorldContentViewModel : Content
    {
        private IEventAggregator event_aggregator;

        [ImportingConstructor]
        public HelloWorldContentViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            DisplayName = "Content";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            event_aggregator.PublishOnUIThread(new OutputMessage("Activating content"));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            event_aggregator.PublishOnUIThread(new OutputMessage("Deactivating content"));
        }
    }
}
