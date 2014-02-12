using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using ReactiveUI;
using System.ComponentModel.Composition;
using System.Linq;
using System;
using ImageDownloader.Utils;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(ProjectSelectionPageViewModel))]
    public class ProjectSelectionPageViewModel : ReactiveScreen, IPage
    {
        private static ILog log = LogManager.GetLog(typeof(ProjectSelectionPageViewModel));

        private Repository repository = new Repository();

        [Import]
        private IEventAggregator EventAggregator { get; set; }

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

        public bool CanDeleteProject
        {
            get { return SelectedProject != null; }
        }

        public bool CanEditProject
        {
            get { return SelectedProject != null; }
        }

        public bool CanSelectProject
        {
            get { return SelectedProject != null; }
        }

        public ProjectSelectionPageViewModel()
        {
            Projects = repository.Projects.CreateDerivedCollection(p => new ProjectViewModel(p));
            Projects.ItemsAdded.Subscribe(p =>
            {
                SelectedProject = p;
                SelectedProject.IsEditing = true;
            });

            this.ObservableForProperty(m => m.SelectedProject)
                .Subscribe(m =>
                {
                    raisePropertyChanged("CanDeleteProject");
                    raisePropertyChanged("CanEditProject");
                    raisePropertyChanged("CanSelectProject");
                    EventAggregator.PublishOnCurrentThread(SelectedProject.Model);
                });
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (Projects.Any() && SelectedProject == null)
                SelectedProject = Projects.First();            
        }

        public void AddProject()
        {
            repository.Projects.Add(new Project { Name = "New" });
        }

        public void DeleteProject()
        {
            repository.Projects.Remove(SelectedProject.Model);
        }

        public void EditProject()
        {
            SelectedProject.IsEditing = true;
        }

        public void SelectProject()
        {
            EventAggregator.PublishOnCurrentThread(NavigationMessage.NavigateToProjectPage());
        }
    }
}
