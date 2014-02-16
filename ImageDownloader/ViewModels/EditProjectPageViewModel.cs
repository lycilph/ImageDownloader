using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ImageDownloader.Messages;
using ImageDownloader.Models;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IPage))]
    public class EditProjectPageViewModel : ReactiveConductor<IStep>.Collection.OneActive, IPage, IHandle<EditMessage>
    {
        private IEventAggregator event_aggregator;
        private List<IStep> steps;

        public PageType Page
        {
            get { return PageType.EditProject; }
        }

        private bool _CanGotoPrevious;
        public bool CanGotoPrevious
        {
            get { return _CanGotoPrevious; }
            set { this.RaiseAndSetIfChanged(ref _CanGotoPrevious, value); }
        }

        private bool _CanGotoNext;
        public bool CanGotoNext
        {
            get { return _CanGotoNext; }
            set { this.RaiseAndSetIfChanged(ref _CanGotoNext, value); }
        }

        private bool _CanDownload;
        public bool CanDownload
        {
            get { return _CanDownload; }
            set { this.RaiseAndSetIfChanged(ref _CanDownload, value); }
        }

        [ImportingConstructor]
        public EditProjectPageViewModel(IEventAggregator event_aggregator,
                                        [ImportMany] IEnumerable<Lazy<IStep, OrderMetadata>> unsorted_steps)
        {
            this.event_aggregator = event_aggregator;
            event_aggregator.Subscribe(this);

            steps = new List<IStep>(unsorted_steps.OrderBy(Lazy => Lazy.Metadata.Order).Select(Lazy => Lazy.Value));
        }

        protected override void ChangeActiveItem(IStep newItem, bool closePrevious)
        {
            //if (ActiveItem != null && ActiveItem.IsBusy)
            //{
            //    event_aggregator.PublishOnCurrentThread(ShellMessage.Disabled);
            //    ActiveItem.Cancel();
            //    await ActiveItem.BusyTask;
            //    event_aggregator.PublishOnCurrentThread(ShellMessage.Enabled);
            //}

            base.ChangeActiveItem(newItem, closePrevious);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Items.AddRange(steps);
            if (Items.Any())
                ActivateItem(Items.First());
        }

        public void GoBack()
        {
            event_aggregator.PublishOnCurrentThread(PageType.ProjectSelection);
        }

        public void GotoPrevious()
        {
            var index = Items.IndexOf(ActiveItem);
            if (index - 1 >= 0)
                ActivateItem(Items[index - 1]);
        }

        public void GotoNext()
        {
            var index = Items.IndexOf(ActiveItem);
            if (index + 1 < Items.Count())
                ActivateItem(Items[index + 1]);
        }

        public void Download()
        {
            event_aggregator.PublishOnCurrentThread(PageType.RunProject);
        }

        public void Handle(EditMessage message)
        {
            CanGotoPrevious = ((message & EditMessage.EnablePrevious) == EditMessage.EnablePrevious);
            CanGotoNext = ((message & EditMessage.EnableNext) == EditMessage.EnableNext);
            CanDownload = ((message & EditMessage.EnableDownload) == EditMessage.EnableDownload);
        }
    }
}
