using Caliburn.Micro;
using ImageDownloader.Interfaces;
using ImageDownloader.Messages;
using ReactiveUI;
using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 2)]
    public class CriteriaStepViewModel : StepBase
    {
        private IRepository repository;
        private IEventAggregator event_aggregator;

        private int? _MinWidth;
        public int? MinWidth
        {
            get { return _MinWidth; }
            set { this.RaiseAndSetIfChanged(ref _MinWidth, value); }
        }

        private int? _MaxWidth;
        public int? MaxWidth
        {
            get { return _MaxWidth; }
            set { this.RaiseAndSetIfChanged(ref _MaxWidth, value); }
        }

        private int? _MinHeight;
        public int? MinHeight
        {
            get { return _MinHeight; }
            set { this.RaiseAndSetIfChanged(ref _MinHeight, value); }
        }

        private int? _MaxHeight;
        public int? MaxHeight
        {
            get { return _MaxHeight; }
            set { this.RaiseAndSetIfChanged(ref _MaxHeight, value); }
        }

        private string _Extension;
        public string Extension
        {
            get { return _Extension; }
            set { this.RaiseAndSetIfChanged(ref _Extension, value); }
        }

        private IReactiveDerivedList<string> _Extensions;
        public IReactiveDerivedList<string> Extensions
        {
            get { return _Extensions; }
            set { this.RaiseAndSetIfChanged(ref _Extensions, value); }
        }

        private ObservableAsPropertyHelper<bool> _CanAddExtension;
        public bool CanAddExtension
        {
            get { return _CanAddExtension.Value; }
        }

        [ImportingConstructor]
        public CriteriaStepViewModel(IRepository repository, IEventAggregator event_aggregator) : base("Criteria")
        {
            this.repository = repository;
            this.event_aggregator = event_aggregator;

            Observable.FromEventPattern(this, "Activated")
                      .Subscribe(x => UpdateNavigationState());

            _CanAddExtension = this.WhenAny(x => x.Extension, x => !string.IsNullOrWhiteSpace(x.Value))
                                   .ToProperty(this, x => x.CanAddExtension);
        }

        protected override void UpdateNavigationState()
        {
            event_aggregator.PublishOnCurrentThread(EditMessage.EnablePrevious | EditMessage.EnableNext);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            IsEnabled = true;

            MinWidth = repository.Current.MinWidth;
            MaxWidth = repository.Current.MaxWidth;
            MinHeight = repository.Current.MinHeight;
            MaxHeight = repository.Current.MaxHeight;
            Extensions = repository.Current.Extensions.CreateDerivedCollection(e => e);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                IsEnabled = false;

            repository.Current.MinWidth = MinWidth;
            repository.Current.MaxWidth = MaxWidth;
            repository.Current.MinHeight = MinHeight;
            repository.Current.MaxHeight = MaxHeight;
        }

        public void AddExtensionShortcut()
        {
            if (string.IsNullOrWhiteSpace(Extension))
                return;

            AddExtension();
        }

        public void AddExtension()
        {
            repository.Current.Extensions.Add(Extension);
            Extension = string.Empty;
        }

        public void DeleteExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return;

            repository.Current.Extensions.Remove(extension);
        }
    }
}
