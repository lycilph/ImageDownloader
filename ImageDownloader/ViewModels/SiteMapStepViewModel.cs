using Caliburn.Micro;
using ImageDownloader.Interfaces;
using ImageDownloader.Messages;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 3)]
    public class SiteMapStepViewModel : StepBase
    {
        private IEventAggregator event_aggregator;
        private IProgress<string> progress;
        private CancellationTokenSource cancellation_source;
        private Task task;
        private bool done;
        
        private ReactiveList<string> _Pages = new ReactiveList<string>();
        public ReactiveList<string> Pages
        {
            get { return _Pages; }
            set { this.RaiseAndSetIfChanged(ref _Pages, value); }
        }

        [ImportingConstructor]
        public SiteMapStepViewModel(IEventAggregator event_aggregator) : base("Site Map")
        {
            this.event_aggregator = event_aggregator;
            progress = new Progress<string>(AddPage);
            done = false;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            IsEnabled = true;

            if (!done)
                Start();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
            {
                Pages.Clear();
                IsEnabled = false;
                done = false;
            }

            if (!done)
                Pages.Clear();
        }

        public override void CanClose(Action<bool> callback)
        {
            Cancel();
            callback(true);
        }

        public override async void Cancel()
        {
            if (IsBusy && cancellation_source != null)
            {
                event_aggregator.PublishOnCurrentThread(ShellMessage.Disabled);
                //cancellation_source.Cancel();
                await task;
                event_aggregator.PublishOnCurrentThread(ShellMessage.Enabled);
            }
        }

        private void Start()
        {
            cancellation_source = new CancellationTokenSource();
            var rnd = new Random();

            IsBusy = true;
            task = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 500; i++)
                {
                    Thread.Sleep(rnd.Next(10000, 25000));

                    if (cancellation_source.Token.IsCancellationRequested)
                        return;

                    progress.Report(string.Format("Page {0}", i));
                }
            }).ContinueWith(parent =>
            {
                IsBusy = false;
                if (!cancellation_source.IsCancellationRequested)
                    done = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void AddPage(string page)
        {
            Pages.Add(page);
        }
    }
}
