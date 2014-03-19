using Caliburn.Micro;

namespace ImageDownloader.Contents.Job.ViewModels
{
    public interface IJobStep : IScreen
    {
        bool IsEnabled { get; set; }
    }
}
