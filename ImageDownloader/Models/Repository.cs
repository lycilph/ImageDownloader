using ImageDownloader.Interfaces;
using ReactiveUI;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.Models
{
    [Export(typeof(IRepository))]
    public class Repository : ReactiveObject, IRepository
    {
        private Project _Current;
        public Project Current
        {
            get { return _Current; }
            set { this.RaiseAndSetIfChanged(ref _Current, value); }
        }

        private ReactiveList<Project> _Projects;
        public ReactiveList<Project> Projects
        {
            get { return _Projects; }
            set { this.RaiseAndSetIfChanged(ref _Projects, value); }
        }

        public Repository()
        {
            Projects = new ReactiveList<Project>
            {
                new Project {Name = "Project 1", Site = "Site 1", ImagesFound = 12},
                new Project {Name = "Project 2"},
                new Project {Name = "Project 3"},
            };

            Current = Projects.First();
        }

        public void Add(Project project)
        {
            Projects.Add(project);
        }

        public void Remove(Project project)
        {
            Projects.Remove(project);
        }
    }
}
