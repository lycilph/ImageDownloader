using ImageDownloader.Core;
using ImageDownloader.Model;

namespace ImageDownloader.Contents.Host.ViewModels
{
    public interface IHost : IContent
    {
        JobModel Model { get; set; }
    }
}
