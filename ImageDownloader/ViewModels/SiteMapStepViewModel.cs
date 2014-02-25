using Caliburn.Micro;
using ImageDownloader.Interfaces;
using ImageDownloader.Messages;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 3)]
    public class SiteMapStepViewModel : StepBase
    {
        private IRepository repository;
        private IEventAggregator event_aggregator;
        private IWindowManager window_manager;
        private CancellationTokenSource cancellation_source;
        private IWebscraper webscraper;
        private Task task;
        private DispatcherTimer timer;
        private int steps;
        private int time_per_step;

        public enum Tab { Pages, Log };
        
        private ReactiveList<string> _Log = new ReactiveList<string>();
        public ReactiveList<string> Log
        {
            get { return _Log; }
            set { this.RaiseAndSetIfChanged(ref _Log, value); }
        }

        private ReactiveList<string> _Pages = new ReactiveList<string>();
        public ReactiveList<string> Pages
        {
            get { return _Pages; }
            set { this.RaiseAndSetIfChanged(ref _Pages, value); }
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

        private int _Time;
        public int Time
        {
            get { return _Time; }
            set { this.RaiseAndSetIfChanged(ref _Time, value); }
        }

        private int _MaxTime;
        public int MaxTime
        {
            get { return _MaxTime; }
            set { this.RaiseAndSetIfChanged(ref _MaxTime, value); }
        }

        private Tab _CurrentTab;
        public Tab CurrentTab
        {
            get { return _CurrentTab; }
            set { this.RaiseAndSetIfChanged(ref _CurrentTab, value); }
        }

        [ImportingConstructor]
        public SiteMapStepViewModel(IRepository repository, IWebscraper scraper, IEventAggregator event_aggregator, IWindowManager window_manager) : base("Site Map")
        {
            this.repository = repository;
            this.webscraper = scraper;
            this.event_aggregator = event_aggregator;
            this.window_manager = window_manager;

            InitializeTimer();

            _CanCancel = this.WhenAnyValue(x => x.IsBusy)
                             .ToProperty(this, x => x.CanCancel);

            _CanStart = this.WhenAny(x => x.IsBusy, x => !x.Value)
                            .ToProperty(this, x => x.CanStart);

            _CanClear = this.WhenAny(x => x.IsBusy, x => !x.Value)
                            .ToProperty(this, x => x.CanClear);

            Observable.Merge(Observable.FromEventPattern(this, "Activated").IgnoreValue(),
                             this.WhenAnyValue(x => x.IsBusy).IgnoreValue(),
                             Pages.Changed.IgnoreValue())
                      .Subscribe(x => UpdateNavigationState());
        }

        private void InitializeTimer()
        {
            _Time = 0;
            _MaxTime = 2000;
            steps = 100;
            time_per_step = _MaxTime / steps;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(time_per_step);
            timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            Time += time_per_step;
            if (Time >= MaxTime)
            {
                Time = 0;
                CurrentTab = Tab.Pages;
                timer.Stop();
            }
        }

        protected override void UpdateNavigationState()
        {
            var message = (Pages.Any() && !IsBusy ? EditMessage.EnablePrevious | EditMessage.EnableNext : EditMessage.EnablePrevious);
            event_aggregator.PublishOnCurrentThread(message);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            IsEnabled = true;

            if (!Pages.Any())
                Start();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                IsEnabled = false;

            event_aggregator.PublishOnCurrentThread(new Result(Pages));
        }

        public override async void CanClose(Action<bool> callback)
        {
            await Cancel();

            callback(true);
        }

        public override Task Cancel()
        {
            if (IsBusy && cancellation_source != null)
            {
                event_aggregator.PublishOnCurrentThread(ShellMessage.Disabled);
                cancellation_source.Cancel();
                return task.ContinueWith(parent => event_aggregator.PublishOnCurrentThread(ShellMessage.Enabled), TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
                return base.Cancel();
        }

        public void Clear()
        {
            Pages.Clear();
            Log.Clear();
            CurrentTab = Tab.Pages;
        }

        public void DeletePages(IEnumerable urls)
        {
            using (var suppressor = Pages.SuppressChangeNotifications())
            {
                Pages.RemoveAll(urls.OfType<string>());
            }
        }

        public void Start()
        {
            cancellation_source = new CancellationTokenSource();

            IsBusy = true;
            Log.Clear();
            Pages.Clear();
            CurrentTab = Tab.Log;

            var sw = new Stopwatch();
            sw.Start();

            var progress = new Progress<Info>(Update);
            task = Task.Factory.StartNew(() => webscraper.FindAllPages(repository.Current, progress, cancellation_source.Token), cancellation_source.Token)
                               .ContinueWith(async parent =>
                               {
                                   // Handle result or failure
                                   if (parent.IsFaulted)
                                   {
                                       var e = parent.Exception as AggregateException;
                                       await DialogService.ShowMetroMessageBox("Error", e.InnerException.Message);
                                   }
                                   else
                                   {
                                       if (parent.Result.Items.Any())
                                           Pages.AddRange(parent.Result.Items);
                                   }

                                   // Add log messages
                                   sw.Stop();
                                   Log.Add(string.Format("Scraping the site \"{0}\" took {1}", repository.Current.Site, sw.Elapsed));
                                   Log.Add("Done");

                                   if (cancellation_source.IsCancellationRequested)
                                       CurrentTab = Tab.Pages;
                                   else
                                       timer.Start(); // Switch to pages tab after a predefined time

                                   // Setup control states
                                   IsBusy = false;
                               }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Update(Info info)
        {
            Log.Add(string.Format("({0}) {1}", info.State.ToString(), info.Item));
        }
    }
}
