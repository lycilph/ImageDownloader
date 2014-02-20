using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using ReactiveUI;
using System.ComponentModel.Composition;
using System.Linq;
using System;
using System.Reactive.Linq;
using ImageDownloader.Utils;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IPage))]
    public class ProjectSelectionPageViewModel : ReactiveScreen, IPage
    {
        private static ILog log = LogManager.GetLog(typeof(ProjectSelectionPageViewModel));

        private IEventAggregator event_aggregator;
        private IRepository repository;

        public PageType Page
        {
            get { return PageType.ProjectSelection; }
        }

        private IReactiveDerivedList<ProjectViewModel> _Projects;
        public IReactiveDerivedList<ProjectViewModel> Projects
        {
            get { return _Projects; }
            set { this.RaiseAndSetIfChanged(ref _Projects, value); }
        }

        private ProjectViewModel _SelectedProject;
        public ProjectViewModel SelectedProject
        {
            get { return _SelectedProject; }
            set { this.RaiseAndSetIfChanged(ref _SelectedProject, value); }
        }

        private ObservableAsPropertyHelper<bool> _CanDeleteProject;
        public bool CanDeleteProject
        {
            get { return _CanDeleteProject.Value; }
        }

        private ObservableAsPropertyHelper<bool> _CanEditProject;
        public bool CanEditProject
        {
            get { return _CanEditProject.Value; }
        }

        private ObservableAsPropertyHelper<bool> _CanRunProject;
        public bool CanRunProject
        {
            get { return _CanRunProject.Value; }
        }

        [ImportingConstructor]
        public ProjectSelectionPageViewModel(IRepository repository, IEventAggregator event_aggregator)
        {
            this.repository = repository;
            this.event_aggregator = event_aggregator;

            Projects = repository.Projects.CreateDerivedCollection(p => new ProjectViewModel(p));
            Projects.ItemsAdded.Subscribe(p =>
            {
                SelectedProject = p;
                SelectedProject.IsEditing = true;
            });

            this.ObservableForProperty(x => x.SelectedProject)
                .Subscribe(x => repository.Current = (SelectedProject == null ? Project.Empty : SelectedProject.Model));

            _CanDeleteProject = this.WhenAny(x => x.SelectedProject, x => x.Value != null)
                                    .ToProperty(this, x => x.CanDeleteProject);

            _CanEditProject = this.WhenAny(x => x.SelectedProject, x => x.Value != null)
                                  .ToProperty(this, x => x.CanEditProject);

            _CanRunProject = this.WhenAny(x => x.SelectedProject, x => x.Value != null)
                                 .ToProperty(this, x => x.CanRunProject);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (Projects.Any() && SelectedProject == null)
                SelectedProject = Projects.First();

            // DEBUG
            EditProject();
        }

        public void Edit()
        {
            SelectedProject.IsEditing = true;
        }

        public void AddProject()
        {
            repository.Add(new Project { Name = "New" });
        }

        public void DeleteProject()
        {
            repository.Remove(SelectedProject.Model);
        }

        public void EditProject()
        {
            event_aggregator.PublishOnCurrentThread(PageType.EditProject);
        }

        public void RunProject()
        {
            if (SelectedProject.Model.CanRun())
                event_aggregator.PublishOnCurrentThread(PageType.RunProject);
            else
                event_aggregator.PublishOnCurrentThread(PageType.EditProject);
        }
    }
}
