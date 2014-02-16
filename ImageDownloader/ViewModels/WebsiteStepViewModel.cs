using Caliburn.Micro;
using ImageDownloader.Interfaces;
using ImageDownloader.Messages;
using ImageDownloader.Models;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.ComponentModel.Composition;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 1)]
    public class WebsiteStepViewModel : StepBase
    {
        private IRepository repository;
        private IEventAggregator event_aggregator;

        private string _Site;
        public string Site
        {
            get { return _Site; }
            set { this.RaiseAndSetIfChanged(ref _Site, value); }
        }

        private string _Keyword = string.Empty;
        public string Keyword
        {
            get { return _Keyword; }
            set { this.RaiseAndSetIfChanged(ref _Keyword, value); }
        }

        private Keyword.RestrictionType _RestrictionType = Models.Keyword.RestrictionType.Include;
        public Keyword.RestrictionType RestrictionType
        {
            get { return _RestrictionType; }
            set { this.RaiseAndSetIfChanged(ref _RestrictionType, value); }
        }

        private IReactiveDerivedList<KeywordViewModel> _Keywords;
        public IReactiveDerivedList<KeywordViewModel> Keywords
        {
            get { return _Keywords; }
            set { this.RaiseAndSetIfChanged(ref _Keywords, value); }
        }

        private ObservableAsPropertyHelper<bool> _CanAddKeyword;
        public bool CanAddKeyword
        {
            get { return _CanAddKeyword.Value; }
        }

        [ImportingConstructor]
        public WebsiteStepViewModel(IRepository repository, IEventAggregator event_aggregator) : base("Website")
        {
            this.repository = repository;
            this.event_aggregator = event_aggregator;
            IsEnabled = true;

            this.ObservableForProperty(x => x.Site)
                .Subscribe(x => Update());

            _CanAddKeyword = this.ObservableForProperty(x => x.Keyword)
                                 .Select(x => !string.IsNullOrWhiteSpace(x.Value))
                                 .ToProperty(this, x => x.CanAddKeyword);
        }

        private void Update()
        {
            var message = (string.IsNullOrWhiteSpace(Site) ? EditMessage.AllDisabled : EditMessage.EnableNext);
            event_aggregator.PublishOnCurrentThread(message);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Site = repository.Current.Site;

            Keywords = repository.Current.Keywords.CreateDerivedCollection(k => new KeywordViewModel(k));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            repository.Current.Site = Site;
        }

        public void AddKeywordShortcut()
        {
            if (string.IsNullOrWhiteSpace(Keyword))
                return;

            AddKeyword();
        }

        public void AddKeyword()
        {
            repository.Current.Keywords.Add(new Keyword { Text = Keyword, Type = RestrictionType });
            Keyword = string.Empty;
        }

        public void DeleteKeyword(KeywordViewModel vm)
        {
            if (vm == null)
                return;

            repository.Current.Keywords.Remove(vm.Model);
        }
    }
}
