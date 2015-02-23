using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Controllers;
using NLog;
using Panda.ApplicationCore;
using Panda.ApplicationCore.Extensions;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace ImageDownloader.Screens.Main
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : ReactiveConductor<StepScreenBase>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Stack<StepScreenBase> screens = new Stack<StepScreenBase>();
        private readonly List<IDisposable> subscriptions = new List<IDisposable>();
        private readonly StatusController status_controller;

        private ReactiveList<StepScreenBase> _Steps;
        public ReactiveList<StepScreenBase> Steps
        {
            get { return _Steps; }
            set { this.RaiseAndSetIfChanged(ref _Steps, value); }
        }

        private bool _CanNext;
        public bool CanNext
        {
            get { return _CanNext; }
            set { this.RaiseAndSetIfChanged(ref _CanNext, value); }
        }

        private bool _CanPrevious;
        public bool CanPrevious
        {
            get { return _CanPrevious; }
            set { this.RaiseAndSetIfChanged(ref _CanPrevious, value); }
        }

        [ImportingConstructor]
        public MainViewModel([ImportMany] IEnumerable<Lazy<StepScreenBase, IExportOrderMetadata>> steps, StatusController status_controller)
        {
            this.status_controller = status_controller;
            Steps = steps.OrderBy(s => s.Metadata.Order)
                         .Select(s => s.Value)
                         .ToReactiveList();
        }

        protected override void ChangeActiveItem(StepScreenBase new_item, bool close_previous)
        {
            logger.Trace("Changing to screen: " + new_item.DisplayName);

            status_controller.MainStatusText = string.Empty;
            status_controller.AuxiliaryStatusText = string.Empty;

            subscriptions.Apply(s => s.Dispose());
            subscriptions.Clear();

            var next_observable = new_item.WhenAnyValue(x => x.CanNext);
            var previous_observable = new_item.WhenAnyValue(x => x.CanPrevious);
            var busy_observable = status_controller.WhenAnyValue(x => x.IsBusy);

            subscriptions.Add(next_observable.CombineLatest(busy_observable, (next, busy) => next && !busy)
                                             .Subscribe(x => CanNext = x));
            subscriptions.Add(previous_observable.CombineLatest(busy_observable, (previous, busy) => previous && !busy)
                                                 .Subscribe(x => CanPrevious = x));

            base.ChangeActiveItem(new_item, close_previous);
        }

        public void ResetAndShow(StepScreenBase screen)
        {
            screens.Clear();
            Show(screen);
        }

        public void Back()
        {
            screens.Pop();
            ActivateItem(screens.Peek());
        }

        public void Show(StepScreenBase screen)
        {
            screens.Push(screen);
            ActivateItem(screen);
        }

        public void Next()
        {
            var index = Steps.IndexOf(ActiveItem);
            Show(Steps[index+1]);
        }

        public void Previous()
        {
            Back();
        }
    }
}
