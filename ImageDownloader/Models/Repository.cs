using ImageDownloader.Interfaces;
using ImageDownloader.Utils;
using Newtonsoft.Json;
using ReactiveUI;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ImageDownloader.Models
{
    [Export(typeof(IRepository))]
    public class Repository : ReactiveObject, IRepository
    {
        private Project _Current = Project.Empty;
        [DataMember]
        public Project Current
        {
            get { return _Current; }
            set { this.RaiseAndSetIfChanged(ref _Current, value); }
        }

        private ReactiveList<Project> _Projects = new ReactiveList<Project>();
        [DataMember]
        public ReactiveList<Project> Projects
        {
            get { return _Projects; }
            set { this.RaiseAndSetIfChanged(ref _Projects, value); }
        }

        //public Repository()
        //{
        //    Projects = new ReactiveList<Project>
        //    {
        //        new Project {Name = "Test",
        //                     Site = @"http://www.skovboernehave.dk/Album/20131213Lucia/index.html",
        //                     Keywords = new ReactiveList<Keyword> { new Keyword {Text = "thumbs", Type = Keyword.RestrictionType.Exclude}}},
        //        new Project {Name = "Project 1", Site = @"http://www.skovboernehave.dk/Album/20131215Juletra/index.html"},
        //        new Project {Name = "Project 2", Site = @"http://www.skovboernehave.dk/Album/20131215Juletra/index.html"},
        //        new Project {Name = "Skovbørnehaven",
        //                     Site = @"http://www.skovboernehave.dk/"}
                
        //    };

        //    Current = Projects.First();
        //}

        public void Add(Project project)
        {
            Projects.Add(project);
        }

        public void Remove(Project project)
        {
            Projects.Remove(project);
        }

        public void Load()
        {
            var filename = GetFilename();

            if (!File.Exists(filename)) return;

            using (var fs = File.Open(filename, FileMode.Open))
            using (var sw = new StreamReader(fs))
            {
                var json = sw.ReadToEnd();
                var settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                var temp = JsonConvert.DeserializeObject<Repository>(json, settings);

                Projects.Clear();
                Projects.AddRange(temp.Projects);
                Current = temp.Current;
            }
        }

        public void Save()
        {
            var filename = GetFilename();

            using (var fs = File.Open(filename, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                var settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                var json = JsonConvert.SerializeObject(this, Formatting.Indented, settings);
                sw.Write(json);
            }
        }

        private string GetFilename()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(dir, "Projects.txt");
        }
    }
}
