using ImageDownloader.Core;

namespace ImageDownloader.Tools.Output.ViewModels
{
    public interface IOutput : ITool
    {
        void Write(string text);
        void Clear();
    }
}
