using ImageDownloader.Interfaces;
using ImageDownloader.Utils;
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
                new Project {Name = "Test", Site = @"http://www.skovboernehave.dk", Keywords = new ReactiveList<Keyword> { new Keyword {Text = "thumbs", Type = Keyword.RestrictionType.Exclude}}},
                new Project {Name = "Project 1", Site = @"http://www.skovboernehave.dk/Album/20131215Juletra/index.html"},
                new Project {Name = "Project 2", Site = @"http://www.skovboernehave.dk/Album/20131215Juletra/index.html"},
                new Project {Name = "Skovbørnehaven",
                             Site = @"http://www.skovboernehave.dk/",
                             Keywords = new ReactiveList<Keyword> { new Keyword {Text = "skov", Type = Keyword.RestrictionType.Exclude},
                                                                    new Keyword {Text = "have", Type = Keyword.RestrictionType.Include}},
                             ImagesFound = 12,}
                
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
