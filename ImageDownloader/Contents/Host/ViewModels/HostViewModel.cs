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
        private JobModel model;
        private IJob job;
        private IBrowser browser;

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
            DisplayName = "Host";

            model = new JobModel { Website = @"file:///C:/Private/GitHub/ImageDownloader/TestSite/index.html" };

            job = IoC.Get<IJob>();
            job.Model = model;
            job.IsHosted = true;

            browser = IoC.Get<IBrowser>();
            browser.IsHosted = true;
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
            browser.Address = model.Website;
            CurrentContent = browser;
        }

        public void CloseBrowser(bool accept_address)
        {
            if (accept_address)
                model.Website = browser.Address;
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
