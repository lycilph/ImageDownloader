using ImageDownloader.Models;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Interfaces
{
    public interface IRepository
    {
        Project Current { get; set; }
        ReactiveList<Project> Projects { get; }

        void Add(Project project);
        void Remove(Project project);
    }
}
