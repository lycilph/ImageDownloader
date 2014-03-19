using Caliburn.Micro;

namespace ImageDownloader.Core
{
    public interface IWindowCommand : IHaveDisplayName
    {
        void Execute();
    }
}
