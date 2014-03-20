using Caliburn.Micro;
using Core;
using ImageDownloader.Core;
using ImageDownloader.Model;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;

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

        private IJobStep _PreviousStep;
        public IJobStep PreviousStep
        {
            get { return _PreviousStep; }
            set { this.RaiseAndSetIfChanged(ref _PreviousStep, value); }
        }

        private IJobStep _CurrentStep;
        public IJobStep CurrentStep
        {
            get { return _CurrentStep; }
            set
            {
                if (_CurrentStep != value)
                {
                    this.RaisePropertyChanging();
                    DeactivateItem(_CurrentStep, false);
                    _CurrentStep = value;
                    ActivateItem(_CurrentStep);
                    this.RaisePropertyChanged();
                }
            }
        }

        private IJobStep _NextStep;
        public IJobStep NextStep
        {
            get { return _NextStep; }
            set { this.RaiseAndSetIfChanged(ref _NextStep, value); }
        }

        private JobModel _Model;
        public JobModel Model
        {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(ref _Model, value); }
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
            CurrentStep = Steps.First();

            this.WhenAnyValue(x => x.Model)
                .Subscribe(x => Steps.Apply(s => s.Model = x));

            var previous_step = this.WhenAny(x => x.PreviousStep, x => x.Value != null);
            var previous_step_enabled = this.WhenAnyDynamic(new string[] { "PreviousStep", "IsEnabled" }, x => x.Value != null && (bool)x.Value);
            _CanGoBack = Observable.Merge(previous_step, previous_step_enabled)
                                   .ToProperty(this, x => x.CanGoBack);

            var next_step = this.WhenAny(x => x.NextStep, x => x.Value != null);
            var next_step_enabled = this.WhenAnyDynamic(new string[] { "NextStep", "IsEnabled" }, x => x.Value != null && (bool)x.Value);
            _CanGoForward = Observable.Merge(next_step, next_step_enabled)
                                      .ToProperty(this, x => x.CanGoForward);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Steps.Apply(s => DeactivateItem(s, close));
        }

        public void GoBack()
        {
            CurrentStep = PreviousStep;
        }

        public void GoForward()
        {
            CurrentStep = NextStep;
        }

        public void ActivateItem(object item)
        {
            var step = item as IJobStep;
            if (step == null) return;

            CurrentStep = step;
            CurrentStep.Activate();

            if (CurrentStep == Steps.First())
            {
                PreviousStep = null;
            }
            else
            {
                var index = Steps.IndexOf(CurrentStep);
                PreviousStep = Steps[index - 1];
            }

            if (CurrentStep == Steps.Last())
            {
                NextStep = null;
            }
            else
            {
                var index = Steps.IndexOf(CurrentStep);
                NextStep = Steps[index + 1];
            }
        }

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        public void DeactivateItem(object item, bool close)
        {
            var step = item as IJobStep;
            if (step == null) return;

            step.Deactivate(close);
        }

        public IEnumerable GetChildren()
        {
            return Steps;
        }
    }
}
