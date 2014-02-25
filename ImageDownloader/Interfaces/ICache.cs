using ImageDownloader.Utils;

namespace ImageDownloader.Interfaces
{
    public interface ICache
    {
        void Initialize(string url);
        void Update();
        void Clear();
        string Get(string url);
        string GetImage(string url);
    }
}
