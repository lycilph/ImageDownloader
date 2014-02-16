using System.Threading.Tasks;

namespace ImageDownloader.Interfaces
{
    public interface IStep
    {
        bool IsEnabled { get; }
        bool IsBusy { get; }
        void Cancel();
    }
}
