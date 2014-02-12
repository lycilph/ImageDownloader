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
    public class ProjectPageViewModel : ReactiveConductor<IStep>.Collection.OneActive, IPage, IPartImportsSatisfiedNotification, IHandle<Project>
    {
        [Import]
        private IEventAggregator EventAggregator { get; set; }

        [ImportMany]
        private IEnumerable<Lazy<IStep, OrderMetadata>> UnsortedSteps { get; set; }

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

        public ProjectPageViewModel()
        {
            this.WhenAny(m => m.ActiveItem, m => m.Value)
                .Where(m => m != null)
                .Subscribe(m =>
                {
                    raisePropertyChanged("CanPrevious");
                    raisePropertyChanged("CanNext");
                });
        }

        public void Previous()
        {
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

        public void OnImportsSatisfied()
        {
            EventAggregator.Subscribe(this);

            Items.AddRange(UnsortedSteps.OrderBy(Lazy => Lazy.Metadata.Order).Select(Lazy => Lazy.Value));
            if (Items.Any())
                ActivateItem(Items.First());
        }
    }
}
