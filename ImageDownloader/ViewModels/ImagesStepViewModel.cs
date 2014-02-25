using ImageDownloader.Interfaces;
using System.ComponentModel.Composition;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Threading;
using Caliburn.Micro;
using ImageDownloader.Utils;
using System.Threading;
using System.Threading.Tasks;
using ImageDownloader.Messages;
using System.Collections;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 4)]
    public class ImagesStepViewModel : StepBase, IHandle<Result>
    {
        private IRepository repository;
        private IEventAggregator event_aggregator;
        private IWebscraper webscraper;
        private CancellationTokenSource cancellation_source;
        private Result pages_result;
        private Task task;

        private ReactiveList<string> _Images = new ReactiveList<string>();
        public ReactiveList<string> Images
        {
            get { return _Images; }
            set { this.RaiseAndSetIfChanged(ref _Images, value); }
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

        [ImportingConstructor]
        public ImagesStepViewModel(IRepository repository, IWebscraper scraper, IEventAggregator event_aggregator) : base("Images")
        {
            this.repository = repository;
            this.webscraper = scraper;
            this.event_aggregator = event_aggregator;
            
            _CanCancel = this.WhenAnyValue(x => x.IsBusy)
                             .ToProperty(this, x => x.CanCancel);

            _CanStart = this.WhenAny(x => x.IsBusy, x => !x.Value)
                            .ToProperty(this, x => x.CanStart);

            _CanClear = this.WhenAny(x => x.IsBusy, x => !x.Value)
                            .ToProperty(this, x => x.CanClear);

            Observable.Merge(Observable.FromEventPattern(this, "Activated").IgnoreValue(),
                             this.WhenAnyValue(x => x.IsBusy).IgnoreValue(),
                             Images.Changed.IgnoreValue())
                      .Subscribe(x => UpdateNavigationState());

            event_aggregator.Subscribe(this);
        }

        protected override void UpdateNavigationState()
        {
            var message = (Images.Any() && !IsBusy ? EditMessage.EnablePrevious | EditMessage.EnableDownload : EditMessage.EnablePrevious);
            event_aggregator.PublishOnCurrentThread(message);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            IsEnabled = true;

            if (!Images.Any() && (pages_result != null && pages_result.Items.Any()))
                Start();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                IsEnabled = false;

            event_aggregator.PublishOnCurrentThread(new Result(Images));
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
            Images.Clear();
        }

        public void DeleteImages(IEnumerable urls)
        {
            using (var suppressor = Images.SuppressChangeNotifications())
            {
                Images.RemoveAll(urls.OfType<string>());
            }
        }

        public void Start()
        {
            cancellation_source = new CancellationTokenSource();

            IsBusy = true;
            Images.Clear();

            var progress = new Progress<Info>(Update);
            task = Task.Factory.StartNew(() => webscraper.FindAllImages(repository.Current, pages_result.Items, progress, cancellation_source.Token), cancellation_source.Token)
                               .ContinueWith(async parent =>
                               {
                                   // Handle failure
                                   if (parent.IsFaulted)
                                   {
                                       var e = parent.Exception as AggregateException;
                                       await DialogService.ShowMetroMessageBox("Error", e.InnerException.Message);
                                   }

                                   // Setup control states
                                   IsBusy = false;
                               }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Update(Info info)
        {
            Images.Add(info.Item);
        }

        public void Handle(Result result)
        {
            pages_result = result;
        }
    }
}
