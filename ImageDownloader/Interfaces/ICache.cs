using ImageDownloader.Utils;

namespace ImageDownloader.Interfaces
{
    public interface ICache
    {
        void Initialize(string url);
        void Update();
        string Get(string url);
    }
}
