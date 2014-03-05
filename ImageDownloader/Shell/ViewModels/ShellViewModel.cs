using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Tools.ViewModels;
using ReactiveUI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<ILayoutItem>.Collection.OneActive, IShell
    {
        private IEventAggregator event_aggregator;

        private IReactiveDerivedList<ITool> _Tools;
        public IReactiveDerivedList<ITool> Tools
        {
            get { return _Tools; }
            set { this.RaiseAndSetIfChanged(ref _Tools, value); }
        }

        private IReactiveDerivedList<IContent> _Content;
        public IReactiveDerivedList<IContent> Content
        {
            get { return _Content; }
            set { this.RaiseAndSetIfChanged(ref _Content, value); }
        }

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<ITool> tools, IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            DisplayName = "Image Downloader";

            Tools = Items.CreateDerivedCollection(i => i as ITool, i => i is ITool);
            Content = Items.CreateDerivedCollection(i => i as IContent, i => i is IContent);

            Items.AddRange(tools);
            Items.AddRange(new List<ILayoutItem> 
            {
                new TestContentViewModel("Project 1"),
                new TestContentViewModel("Project 2"),
            });

            ActivateItem(Items.Last());
        }

        public override void ActivateItem(ILayoutItem item)
        {
            event_aggregator.PublishOnCurrentThread(new LogMessage("Activating " + item.DisplayName));
            base.ActivateItem(item);
        }
    }
}
