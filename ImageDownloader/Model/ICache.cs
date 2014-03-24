using System.Windows.Media.Imaging;

namespace ImageDownloader.Model
{
    public interface ICache
    {
        void Initialize(string host);
        void Update();
        string Get(string url);
        bool TryGetImage(string url, out BitmapImage image);
        void SaveImage(string url);
        void DiscardImage(string url);
    }
}
