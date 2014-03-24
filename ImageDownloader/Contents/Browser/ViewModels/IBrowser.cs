using ImageDownloader.Core;

namespace ImageDownloader.Contents.Browser.ViewModels
{
    interface IBrowser : IContent
    {
        string Address { get; set; }
        bool IsHosted { get; set; }
    }
}
