using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ImageDownloader.Messages;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IPage))]
    public class RunProjectPageViewModel : ReactiveScreen, IPage
    {
        private IRepository repository;
        private IEventAggregator event_aggregator;
        private CancellationTokenSource cancellation_source;
        private Task task;

        private ReactiveList<string> _Log = new ReactiveList<string>();
        public ReactiveList<string> Log
        {
            get { return _Log; }
            set { this.RaiseAndSetIfChanged(ref _Log, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        private ObservableAsPropertyHelper<bool> _CanCancel;
        public bool CanCancel
        {
            get { return _CanCancel.Value; }
        }

        private ObservableAsPropertyHelper<bool> _CanClear;
        public bool CanClear
        {
            get { return _CanClear.Value; }
        }

        private ObservableAsPropertyHelper<bool> _CanStart;
        public bool CanStart
        {
            get { return _CanStart.Value; }
        }

        private ObservableAsPropertyHelper<bool> _CanDownload;
        public bool CanDownload
        {
            get { return _CanDownload.Value; }
        }

        public PageType Page
        {
            get { return PageType.RunProject; }
        }

        [ImportingConstructor]
        public RunProjectPageViewModel(IRepository repository, IEventAggregator event_aggregator)
        {
            this.repository = repository;
            this.event_aggregator = event_aggregator;

            _CanCancel = this.WhenAnyValue(x => x.IsBusy)
                             .ToProperty(this, x => x.CanCancel);

            _CanStart = this.WhenAny(x => x.IsBusy, x => !x.Value)
                            .ToProperty(this, x => x.CanStart);

            _CanClear = this.WhenAny(x => x.IsBusy, x => !x.Value)
                            .ToProperty(this, x => x.CanClear);

            _CanDownload = Observable.Merge(this.WhenAny(x => x.IsBusy, x => !x.Value),
                                            Log.IsEmptyChanged.Select(x => x == false))
                                     .ToProperty(this, x => x.CanDownload);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Start();
        }

        public void Start()
        {
            Log.Clear();
            IsBusy = true;

            cancellation_source = new CancellationTokenSource();
            var bc = new BlockingCollection<string>();

            var throttled_log = new ReactiveList<string>();
            var throttled_log_subscription = throttled_log.ItemsAdded.Throttle(TimeSpan.FromMilliseconds(5)).ObserveOnDispatcher().Subscribe(x => Log.Add(x));
            var prog = new Progress<Info>(x => throttled_log.Add(x.Item));

            var sw = new Stopwatch();
            sw.Start();

            // Create and start producer task
            var producer = IoC.Get<IWebscraper>();
            Task.Factory.StartNew(() =>
            {
                producer.FindAllPages(repository.Current, null, cancellation_source.Token, bc);
                bc.CompleteAdding();
            }, cancellation_source.Token);

            // Create and start consumer task
            var consumer = IoC.Get<IWebscraper>();
            task = Task.Factory.StartNew(() =>
            {
                return consumer.FindAllImages(repository.Current, bc.GetConsumingEnumerable(), prog, cancellation_source.Token);
            }, cancellation_source.Token)
            .ContinueWith(parent =>
            {
                sw.Stop();
                throttled_log_subscription.Dispose();
                IsBusy = false;

                if (parent.Status == TaskStatus.RanToCompletion)
                {
                    event_aggregator.PublishOnCurrentThread(parent.Result);

                    using (var suppressor = Log.SuppressChangeNotifications())
                    {
                        Log.Clear();
                        Log.AddRange(parent.Result.Items);
                    }
                    Log.Add("Images found " + parent.Result.Items.Count());
                    Log.Add("Time elapsed " + sw.Elapsed);
                }
                else
                {
                    Log.Add("Cancelled");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Clear()
        {
            Log.Clear();
        }

        public async void Cancel()
        {
            if (IsBusy && cancellation_source != null)
            {
                event_aggregator.PublishOnCurrentThread(ShellMessage.Disabled);
                cancellation_source.Cancel();
                await task.ContinueWith(parent => event_aggregator.PublishOnCurrentThread(ShellMessage.Enabled), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public void Download()
        {
            event_aggregator.PublishOnCurrentThread(PageType.ShowResults);
        }

        public void Edit() { }

        public void Back()
        {
            event_aggregator.PublishOnCurrentThread(PageType.ProjectSelection);
        }
    }
}