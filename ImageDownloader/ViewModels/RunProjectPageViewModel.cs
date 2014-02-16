using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IPage))]
    public class RunProjectPageViewModel : ReactiveScreen, IPage
    {
        private IEventAggregator event_aggregator;

        public PageType Page
        {
            get { return PageType.RunProject; }
        }

        [ImportingConstructor]
        public RunProjectPageViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;
        }

        public void Back()
        {
            event_aggregator.PublishOnCurrentThread(PageType.ProjectSelection);
        }
    }
}
