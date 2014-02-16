using Caliburn.Micro;
using ImageDownloader.Interfaces;
using System.ComponentModel.Composition;
using ReactiveUI;
using ImageDownloader.Messages;
using System.Reactive.Linq;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 2)]
    public class CriteriaStepViewModel : StepBase
    {
        private IRepository repository;
        private IEventAggregator event_aggregator;

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

            _CanAddExtension = this.ObservableForProperty(x => x.Extension)
                                   .Select(x => !string.IsNullOrWhiteSpace(x.Value))
                                   .ToProperty(this, x => x.CanAddExtension);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            IsEnabled = true;
            event_aggregator.PublishOnCurrentThread(EditMessage.EnablePrevious | EditMessage.EnableNext);

            Extensions = repository.Current.Extensions.CreateDerivedCollection(e => e);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                IsEnabled = false;
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
