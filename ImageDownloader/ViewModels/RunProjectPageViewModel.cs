using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Collections.Concurrent;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IPage))]
    public class RunProjectPageViewModel : ReactiveScreen, IPage
    {
        private IEventAggregator event_aggregator;
        private CancellationTokenSource cts;

        private ReactiveList<string> _Log = new ReactiveList<string>();
        public ReactiveList<string> Log
        {
            get { return _Log; }
            set { this.RaiseAndSetIfChanged(ref _Log, value); }
        }

        public PageType Page
        {
            get { return PageType.RunProject; }
        }

        [ImportingConstructor]
        public RunProjectPageViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
            var bc = new BlockingCollection<int>();

            var disp = Observable.Generate(0,
                                           x => true,
                                           i => i + 1,
                                           i => i,
                                           i => TimeSpan.FromMilliseconds(250))
                                 .Subscribe(x => bc.Add(x));

            cts = new CancellationTokenSource();
            cts.Token.Register(() =>
            {
                disp.Dispose();
                bc.CompleteAdding();
            });

            IProgress<string> prog = new Progress<string>(str => Log.Add(str));

            Task.Factory.StartNew(() =>
            {
                foreach (var i in bc.GetConsumingEnumerable())
                    prog.Report("Consumer 1 - Item " + i);
                prog.Report("Consumer 1 done");
            });

            Task.Factory.StartNew(() =>
            {
                foreach (var i in bc.GetConsumingEnumerable())
                    prog.Report("Consumer 2 - Item " + i);
                prog.Report("Consumer 2 done");
            });
        }

        public void Stop()
        {
            cts.Cancel();
        }

        public void Edit() { }

        public void Back()
        {
            event_aggregator.PublishOnCurrentThread(PageType.ProjectSelection);
        }
    }
}
