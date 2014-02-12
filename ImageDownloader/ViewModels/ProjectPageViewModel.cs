using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(ProjectPageViewModel))]
    public class ProjectPageViewModel : ReactiveConductor<IStep>.Collection.OneActive, IPage, IHandle<Project>
    {
        private Project _Project;
        public Project Project
        {
            get { return _Project; }
            set { this.RaiseAndSetIfChanged(ref _Project, value); }
        }

        public bool CanPrevious
        {
            get { return ActiveItem.CanGotoPrevious; }
        }

        public bool CanNext
        {
            get { return ActiveItem.CanGotoNext; }
        }

        [ImportingConstructor]
        public ProjectPageViewModel(IEventAggregator event_aggregator,
                                    [ImportMany] IEnumerable<Lazy<IStep, OrderMetadata>> steps)
        {
            event_aggregator.Subscribe(this);

            this.WhenAny(m => m.ActiveItem, m => m.Value)
                .Where(m => m != null)
                .Subscribe(m =>
                {
                    raisePropertyChanged("CanPrevious");
                    raisePropertyChanged("CanNext");
                });

            Items.AddRange(steps.OrderBy(Lazy => Lazy.Metadata.Order).Select(Lazy => Lazy.Value));
            if (Items.Any())
                ActivateItem(Items.First());
        }

        public void Previous()
        {
            var index = Items.IndexOf(ActiveItem);
            if (index - 1 >= 0)
                ActivateItem(Items[index - 1]);
        }

        public void Next()
        {
            var index = Items.IndexOf(ActiveItem);
            if (index + 1 < Items.Count())
                ActivateItem(Items[index + 1]);
        }

        public void Handle(Project project)
        {
            Project = project;
        }
    }
}
