using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Models
{
    public class Repository
    {
        public ReactiveList<Project> Projects { get; set; }

        public Repository()
        {
            Projects = new ReactiveList<Project>
            {
                new Project {Name = "Project 1", Site = "Site 1", ExecutionCount = 12},
                new Project {Name = "Project 2", Site = "Site 2"},
                new Project {Name = "Project 3", Site = "Site 3"},
            };
        }
    }
}
