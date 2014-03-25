using ImageDownloader.Contents.Host.ViewModels;

namespace ImageDownloader.Model
{
    public interface IJobFactory
    {
        IHost Create();
    }
}
