using ImageDownloader.Core;
using ImageDownloader.Model;

namespace ImageDownloader.Contents.Job.ViewModels
{
    public interface IJob : IContent
    {
        JobModel Model { get; set; }
        bool IsHosted { get; set; }
    }
}
