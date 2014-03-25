using Caliburn.Micro;
using ImageDownloader.Contents.Browser.ViewModels;
using ImageDownloader.Contents.Job.ViewModels;
using ImageDownloader.Core;
using ImageDownloader.Model;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Collections;

namespace ImageDownloader.Contents.Host.ViewModels
{
    [Export(typeof(IHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HostViewModel : Content, IHost, IConductor
    {
        private IJob job;

        private Lazy<IBrowser> _Browser;
        private IBrowser Browser
        {
            get { return _Browser.Value; }
        }

        public JobModel Model
        {
            get { return job.Model; }
            set { job.Model = value; }
        }

        private IContent _CurrentContent;
        public IContent CurrentContent
        {
            get { return _CurrentContent; }
            set
            {
                if (_CurrentContent != value)
                {
                    this.RaisePropertyChanging();
                    DeactivateItem(_CurrentContent, false);
                    _CurrentContent = value;
                    ActivateItem(_CurrentContent);
                    this.RaisePropertyChanged();
                }
            }
        }

        [ImportingConstructor]
        public HostViewModel(IEventAggregator event_aggregator) : base(event_aggregator)
        {
            job = IoC.Get<IJob>();
            job.IsHosted = true;

            job.WhenAnyValue(x => x.DisplayName)
               .Subscribe(x => DisplayName = x);

            _Browser = new Lazy<IBrowser>(() =>
            {
                var browser = IoC.Get<IBrowser>();
                browser.IsHosted = true;
                return browser;
            });
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CurrentContent = job;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(CurrentContent);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            DeactivateItem(CurrentContent, close);
        }

        public void OpenBrowser()
        {
            Browser.Address = Model.Website;
            CurrentContent = Browser;
        }

        public void CloseBrowser(bool accept_address)
        {
            if (accept_address)
                Model.Website = Browser.Address;
            CurrentContent = job;
        }

        public void ActivateItem(object item)
        {
            var content = item as IContent;
            content.Activate();
        }

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        public void DeactivateItem(object item, bool close)
        {
            var content = item as IContent;
            if (content == null) return;

            content.Deactivate(close);
        }

        public IEnumerable GetChildren()
        {
            return null;
        }
    }
}
