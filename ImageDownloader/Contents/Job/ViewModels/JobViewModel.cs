using Caliburn.Micro;
using Core;
using ImageDownloader.Core;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.Contents.Job.ViewModels
{
    [Export(typeof(IJob))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class JobViewModel : Content, IJob, IConductor
    {
        private ReactiveList<IJobStep> _Steps;
        public ReactiveList<IJobStep> Steps
        {
            get { return _Steps; }
            set { this.RaiseAndSetIfChanged(ref _Steps, value); }
        }

        private IJobStep _CurrentStep;
        public IJobStep CurrentStep
        {
            get { return _CurrentStep; }
            set { this.RaiseAndSetIfChanged(ref _CurrentStep, value); }
        }

        private ObservableAsPropertyHelper<bool> _CanGoBack;
        public bool CanGoBack
        {
            get { return _CanGoBack.Value; }
        }

        private ObservableAsPropertyHelper<bool> _CanGoForward;
        public bool CanGoForward
        {
            get { return _CanGoForward.Value; }
        }

        [ImportingConstructor]
        public JobViewModel([ImportMany] IEnumerable<Lazy<IJobStep, OrderMetadata>> steps, IEventAggregator event_aggregator) : base(event_aggregator)
        {
            DisplayName = "Job";
            Steps = new ReactiveList<IJobStep>(steps.OrderBy(s => s.Metadata.Order).Select(s => s.Value));
            ActivateItem(Steps.First());

            _CanGoBack = this.WhenAny(x => x.CurrentStep, x => x.Value != Steps.First())
                             .ToProperty(this, x => x.CanGoBack);

            _CanGoForward = this.WhenAny(x => x.CurrentStep, x => x.Value != Steps.Last())
                                .ToProperty(this, x => x.CanGoForward);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Steps.Apply(s => DeactivateItem(s, close));
        }

        public void GoBack()
        {
            var index = Steps.IndexOf(CurrentStep);
            ActivateItem(Steps[index - 1]);
        }

        public void GoForward()
        {
            var index = Steps.IndexOf(CurrentStep);
            ActivateItem(Steps[index + 1]);
        }

        public void ActivateItem(object item)
        {
            var step = item as IJobStep;
            CurrentStep = step;
            CurrentStep.Activate();
        }

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        public void DeactivateItem(object item, bool close)
        {
            var step = item as IJobStep;
            step.Deactivate(close);
        }

        public IEnumerable GetChildren()
        {
            return Steps;
        }
    }
}
