using Caliburn.Micro;
using ImageDownloader.Model;

namespace ImageDownloader.Contents.Job.ViewModels
{
    public interface IJobStep : IScreen
    {
        JobModel Model { get; set; }
        bool IsEnabled { get; set; }
    }
}
