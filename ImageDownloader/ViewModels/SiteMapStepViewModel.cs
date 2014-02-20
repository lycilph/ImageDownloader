using Caliburn.Micro;
using ImageDownloader.Interfaces;
using ImageDownloader.Messages;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 3)]
    public class SiteMapStepViewModel : StepBase
    {
        private IRepository repository;
        private IEventAggregator event_aggregator;
        private IProgress<ScraperInfo> progress;
        private CancellationTokenSource cancellation_source;
        private IScraper scraper;
        private Task task;
        private bool done;

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

        private ObservableAsPropertyHelper<bool> _CanStart;
        public bool CanStart
        {
            get { return _CanStart.Value; }
        }

        private Tab _CurrentTab;
        public Tab CurrentTab
        {
            get { return _CurrentTab; }
            set { this.RaiseAndSetIfChanged(ref _CurrentTab, value); }
        }

        [ImportingConstructor]
        public SiteMapStepViewModel(IRepository repository, IScraper scraper, IEventAggregator event_aggregator) : base("Site Map")
        {
            this.repository = repository;
            this.scraper = scraper;
            this.event_aggregator = event_aggregator;
            progress = new Progress<ScraperInfo>(Update);
            done = false;

            _CanCancel = this.WhenAnyValue(x => x.IsBusy)
                             .ToProperty(this, x => x.CanCancel);

            _CanStart = this.WhenAny(x => x.IsBusy, x => !x.Value)
                            .ToProperty(this, x => x.CanStart);
        }

        private void UpdateNavigationState()
        {
            var message = (done ? EditMessage.EnablePrevious | EditMessage.EnableNext : EditMessage.EnablePrevious);
            event_aggregator.PublishOnCurrentThread(message);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            IsEnabled = true;
            scraper.Progress = progress;

            UpdateNavigationState();

            if (!done)
                Start();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            scraper.Progress = null;

            if (close)
            {
                IsEnabled = false;
                done = false;
            }
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

        public void DeletePages(IList urls)
        {
            while (urls.Count > 0)
            {
                Pages.Remove((string)urls[0]);
            }
        }

        public void Start()
        {
            cancellation_source = new CancellationTokenSource();

            IsBusy = true;
            done = false;
            Log.Clear();
            UpdateNavigationState();
            CurrentTab = Tab.Log;

            var sw = new Stopwatch();
            sw.Start();

            task = Task.Factory.StartNew(() => scraper.FindAllPages(repository.Current, cancellation_source.Token))
                               .ContinueWith(async parent =>
                               {
                                   // Handle result or failure
                                   if (parent.IsFaulted)
                                   {
                                       var e = parent.Exception as AggregateException;
                                       System.Windows.MessageBox.Show(e.InnerException.Message);
                                   }
                                   else
                                   {
                                       Pages.Clear();
                                       Pages.AddRange(parent.Result.Pages);
                                   }

                                   // Add log messages
                                   sw.Stop();
                                   Log.Add(string.Format("Scraping the site \"{0}\" took {1}", repository.Current.Site, sw.Elapsed));
                                   Log.Add("Done");

                                   // Setup control states
                                   IsBusy = false;
                                   if (!cancellation_source.IsCancellationRequested)
                                       done = true;
                                   UpdateNavigationState();

                                   // Wait 3 sec then change to result
                                   await Task.Delay(3000);
                                   CurrentTab = Tab.Pages;
                               }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Update(ScraperInfo info)
        {
            Log.Add(string.Format("({0}) {1}", info.State.ToString(), info.Item));
        }
    }
}
