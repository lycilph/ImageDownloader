using ImageDownloader.Models;
using ReactiveUI;

namespace ImageDownloader.Interfaces
{
    public interface IRepository
    {
        Project Current { get; set; }
        ReactiveList<Project> Projects { get; }

        void Add(Project project);
        void Remove(Project project);

        void Load();
        void Save();
    }
}
