using ImageDownloader.Models;

namespace ImageDownloader.ViewModels
{
    public class ProjectViewModel : ViewModelBase<Project>
    {
        public ProjectViewModel(Project project) : base(project) {}
    }
}
